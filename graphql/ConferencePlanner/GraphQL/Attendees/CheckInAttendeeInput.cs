﻿using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL;

public class CheckInAttendeeInput
{
    [ID(nameof(Session))]
    public int SessionId { get; set; }

    [ID(nameof(Attendee))]
    public int AttendeeId { get; set; }
}