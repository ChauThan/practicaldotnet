
# Authentication & Identity with Two-Factor Authentication

## 📝 Recent Changes

- Added endpoints for two-factor authentication (2FA) management and login:
  - `/api/Auth/2fa/enable` to enable 2FA
  - `/api/Auth/2fa/verify` to verify 2FA setup
  - `/api/Auth/2fa/disable` to disable 2FA
  - `/api/Auth/2fa/recovery-codes` to generate recovery codes
  - `/api/Auth/login-2fa` to complete login with 2FA
- Updated `/api/Auth/login` to return a `requiresTwoFactor` flag and user ID if 2FA is required, instead of issuing a token immediately.
- Ensured that JWT tokens are only issued after successful 2FA verification.

This project demonstrates a clean architecture implementation for authentication and identity management using ASP.NET Core Identity, JWT tokens, and **Two-Factor Authentication (2FA)**.

## 🔐 Two-Factor Authentication Features

### Complete 2FA Implementation
- **TOTP Support**: Uses Time-based One-Time Passwords (Google Authenticator, Authy, etc.)
- **Setup Flow**: Enable, verify, and disable 2FA for users
- **Login Flow**: Two-step login process for 2FA-enabled users
- **Recovery Codes**: Generate backup codes for account recovery
- **Security**: JWT tokens only issued after complete authentication

### 2FA API Endpoints

#### Setup Endpoints
- `POST /api/Auth/2fa/enable` - Get QR code and secret key for setup
- `POST /api/Auth/2fa/verify` - Verify and complete 2FA setup
- `POST /api/Auth/2fa/disable` - Disable 2FA for a user
- `POST /api/Auth/2fa/recovery-codes` - Generate recovery codes

#### Login Endpoints
- `POST /api/Auth/login` - Initial login (returns `requiresTwoFactor: true` if 2FA enabled)
- `POST /api/Auth/login-2fa` - Complete 2FA login with authenticator code

## 🚀 2FA Login Flow

### Without 2FA (Standard Flow)
```
User → POST /api/Auth/login → JWT Token (immediate)
```

### With 2FA (Enhanced Security Flow)
```
User → POST /api/Auth/login → RequiresTwoFactor=true (no token)
     → POST /api/Auth/login-2fa → JWT Token (after verification)
```

### Step-by-Step Process

1. **Initial Login**
   ```http
   POST /api/Auth/login
   {
     "email": "user@example.com",
     "password": "UserPassword123!"
   }
   ```
   
   **Response (2FA Required)**:
   ```json
   {
     "succeeded": false,
     "message": "Two-factor authentication required.",
     "userId": "12345678-1234-1234-1234-123456789012",
     "userName": "user@example.com",
     "token": null,
     "requiresTwoFactor": true
   }
   ```

2. **Complete 2FA Login**
   ```http
   POST /api/Auth/login-2fa
   {
     "userId": "12345678-1234-1234-1234-123456789012",
     "code": "123456",
     "rememberMe": false,
     "rememberClient": false
   }
   ```
   
   **Response (Success)**:
   ```json
   {
     "succeeded": true,
     "message": "Login successful.",
     "userId": "12345678-1234-1234-1234-123456789012",
     "userName": "user@example.com",
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }
   ```

## 📋 Testing 2FA

### Quick Start Testing Guide

1. **Start the application**
   ```bash
   dotnet run --project App.Api
   ```

2. **Register a new user**
   ```http
   POST https://localhost:7001/api/Auth/register
   {
     "email": "testuser@example.com",
     "password": "Password123!",
     "firstName": "Test",
     "lastName": "User"
   }
   ```

3. **Enable 2FA**
   ```http
   POST https://localhost:7001/api/Auth/2fa/enable
   {
     "userId": "your-user-id-here"
   }
   ```
   Response includes QR code URI and secret key for authenticator app.

4. **Setup authenticator app** (Google Authenticator, Authy, etc.) using the secret key

5. **Verify 2FA setup**
   ```http
   POST https://localhost:7001/api/Auth/2fa/verify
   {
     "userId": "your-user-id-here",
     "code": "123456"
   }
   ```

6. **Test 2FA login**
   ```http
   POST https://localhost:7001/api/Auth/login
   {
     "email": "testuser@example.com",
     "password": "Password123!"
   }
   ```
   Should return `requiresTwoFactor: true`

7. **Complete login with 2FA**
   ```http
   POST https://localhost:7001/api/Auth/login-2fa
   {
     "userId": "your-user-id-here",
     "code": "current-6-digit-code",
     "rememberMe": false,
     "rememberClient": false
   }
   ```
   Should return JWT token

## 🔒 Security Features

### Enhanced Security
- **No Token Leakage**: Initial login with 2FA requirement doesn't expose JWT tokens
- **Defense in Depth**: JWT tokens only issued after complete authentication (password + 2FA)
- **Proper Validation**: All 2FA operations validate user state and 2FA enablement
- **Standard Compliance**: Uses ASP.NET Core Identity's proven TOTP implementation

### Backward Compatibility
✅ **Fully backward compatible**
- Existing users without 2FA continue to work as before
- Existing endpoints maintain their behavior
- No breaking changes to existing functionality

## 🏗️ Clean Architecture

This project follows Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────────┐
│                        App.Api (Presentation Layer)            │
│  Controllers, Program.cs, Configuration                        │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                   App.Application (Application Layer)          │
│  Features (CQRS), Abstractions, MediatR                       │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                     App.Domain (Domain Layer)                  │
│  Entities, Value Objects, Business Rules                       │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                App.Infrastructure (Infrastructure Layer)       │
│  Database, Services, External APIs                             │
└─────────────────────────────────────────────────────────────────┘
```

## 🛠️ Technologies Used

- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core** - ORM for data access
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer Tokens** - Stateless authentication
- **TOTP (Time-based OTP)** - Two-factor authentication
- **MediatR** - CQRS pattern implementation
- **SQL Server** - Database provider

## 📚 Additional Documentation

### 2FA Specific Documentation
- **[2FA_LOGIN_FLOW.md](2FA_LOGIN_FLOW.md)** - Detailed 2FA implementation and API responses
- **[2FA_IMPLEMENTATION_SUMMARY.md](2FA_IMPLEMENTATION_SUMMARY.md)** - Technical implementation summary
- **[2FA_TROUBLESHOOTING.md](2FA_TROUBLESHOOTING.md)** - Common issues and solutions

### General Project Documentation  
- **[PROJECT_DOCUMENTATION.md](PROJECT_DOCUMENTATION.md)** - Complete architecture details
- **[SEED_DATA.md](SEED_DATA.md)** - Default users and roles information

### Quick Testing
- **[App.Api/requests.http](App.Api/requests.http)** - Complete HTTP request examples for testing

## 🚀 Getting Started

1. **Clone the repository**
2. **Update connection string** in `appsettings.json`
3. **Run migrations**: `dotnet ef database update`
4. **Start the application**: `dotnet run --project App.Api`
5. **Access API documentation** at `/scalar/v1`
6. **Test 2FA using the HTTP requests** in `App.Api/requests.http`

## 🔑 Default Credentials

### Admin User (No 2FA by default)
- **Email:** `admin@app.com`
- **Password:** `Admin123!`
- **Role:** Admin

⚠️ **Security Note**: Change default credentials in production!

---

**Ready to test 2FA?** Start with the HTTP requests in `App.Api/requests.http` for a complete walkthrough!
