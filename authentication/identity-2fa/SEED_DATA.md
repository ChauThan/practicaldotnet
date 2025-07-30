# Seed Data Documentation

## Overview
The application now includes automatic seed data that creates default roles and an admin user on application startup.

## Seeded Data

### Roles
The following roles are automatically created:
- **Admin** - Full administrative access
- **User** - Regular user access
- **Manager** - Management level access

### Default Admin User
A default admin user is created with the following credentials:

**Email:** `admin@app.com`
**Password:** `Admin123!`
**Roles:** Admin

### User Details
- **First Name:** System
- **Last Name:** Administrator
- **Email Confirmed:** Yes
- **Created Date:** Current UTC time when seeded

## How It Works

### Seeding Process
1. The `SeedService` runs automatically on application startup
2. Database migrations are applied first
3. Roles are checked and created if they don't exist
4. Admin user is checked and created if it doesn't exist
5. Admin role is assigned to the admin user

### Implementation Details
- **Location:** `App.Infrastructure/Services/SeedService.cs`
- **Configuration:** `App.Infrastructure/ServiceExtensions/InfrastructureServiceExtensions.cs`
- **Execution:** Called from `Program.cs` during application startup

### Safety Features
- Idempotent operations - won't create duplicates
- Checks for existing roles and users before creation
- Comprehensive logging for troubleshooting
- Error handling with detailed error messages

## Testing the Admin User

You can test the admin user by:

1. **Using the API endpoints:**
   - POST `/auth/login` with the admin credentials
   - Use the returned JWT token for authenticated requests

2. **Example login request:**
   ```json
   {
     "email": "admin@app.com",
     "password": "Admin123!"
   }
   ```

3. **Verify admin role:**
   - Check that the user has the "Admin" role in the JWT token claims
   - Test admin-only endpoints (if any are implemented)

## Security Considerations

⚠️ **Important:** For production environments:

1. **Change the default password immediately**
2. **Use a secure, generated password**
3. **Consider disabling automatic seeding in production**
4. **Implement proper password policies**
5. **Enable email confirmation if required**

## Configuration

The seeding behavior can be modified in:
- `SeedService.cs` - Change roles, user details, or add more seed data
- `InfrastructureServiceExtensions.cs` - Modify when seeding occurs
- `Program.cs` - Enable/disable seeding

## Troubleshooting

Check the application logs for seeding status:
- Look for "SeedService" log entries
- Verify database connectivity
- Ensure migrations have been applied
- Check for any permission issues

The seeding process will log:
- When roles are created or already exist
- When the admin user is created or already exists
- Any errors that occur during the process
