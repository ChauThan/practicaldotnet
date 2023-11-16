using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TodoClient;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditEntry(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddAuditEntry(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditEntry(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        context.ChangeTracker.DetectChanges();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                continue;
            }

            var builder = new AuditBuilder(entry, Guid.Empty);
            context.Set<Audit>().Add(builder.ToAudit());
        }
    }

    private class AuditBuilder
    {
        private readonly EntityEntry _entityEntry;
        private readonly Guid _userId;
        private readonly Dictionary<string, object?> _keyValues = new();
        private readonly Dictionary<string, object?> _oldValues = new();
        private readonly Dictionary<string, object?> _newValues = new();
        private AuditType _auditType = AuditType.None;
        private readonly List<string> _changedColumns = new();

        public AuditBuilder(EntityEntry entityEntry, Guid userId)
        {
            _entityEntry = entityEntry;
            _userId = userId;
        }

        public Audit ToAudit()
        {
            foreach (var property in _entityEntry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    _keyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (_entityEntry.State)
                {
                    case EntityState.Added:
                        _auditType = AuditType.Create;
                        _newValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        _auditType = AuditType.Delete;
                        _oldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            _auditType = AuditType.Update;
                            _changedColumns.Add(propertyName);
                            _oldValues[propertyName] = property.OriginalValue;
                            _newValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
            var audit = new Audit
            {
                UserId = _userId,
                Type = _auditType,
                EntityName = _entityEntry.Entity.GetType().Name,
                AuditedAt = DateTime.Now,
                PrimaryKey = _keyValues.Count == 0 ? null : JsonSerializer.Serialize(_keyValues),
                OldValues = _oldValues.Count == 0 ? null : JsonSerializer.Serialize(_oldValues),
                NewValues = _newValues.Count == 0 ? null : JsonSerializer.Serialize(_newValues),
                AffectedColumns = _changedColumns.Count == 0 ? null : JsonSerializer.Serialize(_changedColumns)
            };

            return audit;
        }
    }
}
