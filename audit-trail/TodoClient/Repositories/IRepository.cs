﻿namespace TodoClient;

public interface IRepository<TEntity> where TEntity : Entity
{
    void Add(TEntity entity);

    void Delete(TEntity entity);

    void Update(TEntity entity);

    TEntity? GetById(Guid id);
}