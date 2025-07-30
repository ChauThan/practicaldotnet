# Two-Factor Authentication (2FA) Login Flow - Technical Details

> 📖 **For a complete overview and quick start guide, see [README.md](README.md)**

This document provides detailed technical information about the 2FA login flow implementation.

## Overview

The 2FA login flow has been implemented with two main endpoints:
1. `/api/Auth/login` - Initial login with username/password
2. `/api/Auth/login-2fa` - Complete login with 2FA code

## Flow Description

### Step 1: Initial Login
**Endpoint**: `POST /api/Auth/login`

**Request**:
```json
{
  "email": "user@example.com",
  "password": "UserPassword123!"
}
```

**Possible Responses**:

#### Success (No 2FA Required)
```json
{
  "succeeded": true,
  "message": "Login successful.",
  "userId": "12345678-1234-1234-1234-123456789012",
  "userName": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "requiresTwoFactor": false
}
```

#### 2FA Required
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

#### Invalid Credentials
```json
{
  "succeeded": false,
  "message": "Invalid credentials.",
  "userId": null,
  "userName": null,
  "token": null,
  "requiresTwoFactor": false
}
```

#### Account Locked
```json
{
  "succeeded": false,
  "message": "User account locked out.",
  "userId": null,
  "userName": null,
  "token": null,
  "requiresTwoFactor": false
}
```

### Step 2: Complete 2FA Login (if required)
**Endpoint**: `POST /api/Auth/login-2fa`

**Request**:
```json
{
  "userId": "12345678-1234-1234-1234-123456789012",
  "code": "123456",
  "rememberMe": false,
  "rememberClient": false
}
```

**Possible Responses**:

#### Success
```json
{
  "succeeded": true,
  "message": "Login successful.",
  "userId": "12345678-1234-1234-1234-123456789012",
  "userName": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Invalid 2FA Code
```json
{
  "succeeded": false,
  "message": "Invalid two-factor code.",
  "userId": null,
  "userName": null,
  "token": null
}
```

#### 2FA Not Enabled
```json
{
  "succeeded": false,
  "message": "Two-factor authentication is not enabled for this user.",
  "userId": null,
  "userName": null,
  "token": null
}
```

## Implementation Details

### Key Changes Made

1. **Enhanced SignInResult Domain Model**
   - Added `RequiresTwoFactor` property to match ASP.NET Core Identity's SignInResult

2. **Updated SignInService**
   - Modified `CheckPasswordSignInAsync` to return the `RequiresTwoFactor` flag
   - Enhanced `TwoFactorAuthenticatorSignInAsync` to include the new property

3. **Modified Login Feature**
   - Added `RequiresTwoFactor` property to the response
   - Updated handler to check for `result.RequiresTwoFactor` and return appropriate response

4. **New LoginWith2FA Feature**
   - Created dedicated handler for completing 2FA login
   - Validates that 2FA is enabled for the user
   - Uses `ISignInService.TwoFactorAuthenticatorSignInAsync` for verification
   - Issues JWT token upon successful 2FA verification

5. **Updated AuthController**
   - Added `/api/Auth/login-2fa` endpoint

6. **Renamed VerifyTwoFactor to VerifyTwoFactorSetup**
   - Made it clear this is for setting up 2FA, not logging in

### Testing Flow

1. **Enable 2FA for a user**:
   ```http
   POST /api/Auth/2fa/enable
   {
     "userId": "user-guid"
   }
   ```

2. **Verify 2FA setup**:
   ```http
   POST /api/Auth/2fa/verify
   {
     "userId": "user-guid",
     "code": "123456"
   }
   ```

3. **Test login with 2FA**:
   ```http
   POST /api/Auth/login
   {
     "email": "user@example.com",
     "password": "password"
   }
   ```
   This should return `requiresTwoFactor: true`

4. **Complete 2FA login**:
   ```http
   POST /api/Auth/login-2fa
   {
     "userId": "user-guid",
     "code": "123456",
     "rememberMe": false,
     "rememberClient": false
   }
   ```
   This should return a JWT token

## Security Considerations

- JWT tokens are only issued after both password verification AND 2FA verification
- The initial login response with `requiresTwoFactor: true` does NOT include a token
- UserID is included in the 2FA required response to facilitate the second step
- The 2FA verification endpoint validates that 2FA is actually enabled for the user
- All validation uses ASP.NET Core Identity's built-in 2FA mechanisms

## Error Handling

The system handles various error scenarios:
- Invalid credentials in initial login
- Account lockout
- 2FA not enabled when trying to use 2FA login
- Invalid 2FA codes
- User not found scenarios
