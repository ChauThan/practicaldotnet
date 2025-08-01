# i-hosted-service

This project demonstrates how to implement a background job in .NET using `IHostedService` and `System.Threading.Timer`. The service periodically executes a task at a fixed interval (every 5 seconds by default) and logs the execution count. It is a practical example for learning how to run recurring background work in ASP.NET Core or generic host applications.

## How It Works
- The `TimedHostedService` class implements `IHostedService` and `IDisposable`.
- When the service starts, it creates a timer that calls the `DoWork` method every 5 seconds.
- The `DoWork` method increments a counter and logs the current count.
- When the service stops, the timer is disabled to prevent further executions.
- The timer is disposed of when the service is disposed.

## Important Concern
**What happens if we do not stop the timer in `StopAsync`?**

If you do not call `_timer?.Change(Timeout.Infinite, 0);` in the `StopAsync` method, the timer will continue to run and execute the background job even after the service is supposed to be stopped. This can lead to background work running unexpectedly, resource leaks, and potential issues during application shutdown. Always ensure the timer is stopped when the service is stopping.

## Usage
1. Build and run the project.
2. Observe the log output to see the recurring background job in action.
3. When the application is stopped, the background job will also stop gracefully.
