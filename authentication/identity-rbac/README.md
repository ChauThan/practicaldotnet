# Identity RBAC API

## Role-Based Access Control (RBAC) Implementation

This project implements Role-Based Access Control (RBAC) using ASP.NET Core Identity. Users are assigned roles, and API endpoints are protected using role-based authorization. Only users with the required roles can access certain endpoints.

### Key Features
- User and role management with ASP.NET Core Identity
- JWT authentication with roles included in the token
- Role-based authorization using `[Authorize(Roles = "RoleName")]`

## APIs Supporting RBAC

| Endpoint                  | Method | Description                                 | Required Role |
|--------------------------|--------|---------------------------------------------|--------------|
| `/api/roles/all`         | GET    | List all roles                              | Admin        |
| `/api/roles/create`      | POST   | Create a new role                           | Admin        |
| `/api/roles/assign-to-user` | POST | Assign a role to a user                     | Admin        |
| `/api/roles/remove-from-user` | POST | Remove a role from a user                  | Admin        |
| `/api/roles/user-roles/{userId}` | GET | List roles assigned to a user             | Admin        |

Other endpoints (such as authentication) are available for all users.

## How to Use RBAC

1. **Login as Admin**
   - Use the seeded admin user:
     - Email: `admin@example.com`
     - Password: `Admin123!`
   - Authenticate via `/api/auth/login` to receive a JWT token.

2. **Access Protected Endpoints**
   - Use your token to call any RBAC-protected endpoint (see table above).
   - Only users with the `Admin` role can access these endpoints.

3. **Assign Roles to Other Users**
   - Use `/api/roles/assign-to-user` to assign roles to other users.
   - Example payload:
     ```json
     {
       "UserId": "<user-guid>",
       "RoleName": "Admin"
     }
     ```

4. **Check User Roles**
   - Use `/api/roles/user-roles/{userId}` to see which roles a user has.

5. **Remove Roles**
   - Use `/api/roles/remove-from-user` to remove a role from a user.

## Notes
- Only users with the `Admin` role can manage roles and assignments.
- JWT tokens must include the `role` claim for role-based authorization to work.
- The Scalar UI provides a convenient way to test and explore the API.

---

For more details, see the source code and comments in the controllers and feature handlers.
