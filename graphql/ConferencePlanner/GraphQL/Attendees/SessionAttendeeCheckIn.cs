﻿using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL;

public class SessionAttendeeCheckIn
{
    public SessionAttendeeCheckIn(int attendeeId, int sessionId)
    {
        AttendeeId = attendeeId;
        SessionId = sessionId;
    }

    [ID(nameof(Attendee))]
    public int AttendeeId { get; }

    [ID(nameof(Session))]
    public int SessionId { get; }

    public async Task<int> CheckInCountAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken) =>
        await context.Sessions
            .Where(session => session.Id == SessionId)
            .SelectMany(session => session.SessionAttendees)
            .CountAsync(cancellationToken);

    public Task<Attendee> GetAttendeeAsync(
        AttendeeByIdDataLoader attendeeById,
        CancellationToken cancellationToken) =>
        attendeeById.LoadAsync(AttendeeId, cancellationToken);

    public Task<Session> GetSessionAsync(
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken) =>
        sessionById.LoadAsync(SessionId, cancellationToken);
}
