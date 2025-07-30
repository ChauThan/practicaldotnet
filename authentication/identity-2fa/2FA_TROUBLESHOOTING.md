# 2FA Troubleshooting Guide

## Common Issues and Solutions

### 1. "Invalid code" Error

This error typically occurs due to several reasons:

#### **Time Synchronization Issues**
- **Problem**: TOTP codes are time-based. If your server time or device time is out of sync, codes will be invalid.
- **Solution**: 
  - Ensure your server system clock is accurate
  - Check your mobile device's time is correct
  - TOTP codes are usually valid for 30 seconds

#### **Missing Authenticator Key Setup**
- **Problem**: The authenticator app wasn't properly configured with the shared key
- **Solution**: 
  1. Call `/api/Auth/2fa/enable` to get the shared key and QR code URI
  2. Properly scan the QR code or manually enter the key in your authenticator app
  3. Ensure you're using the correct account in your authenticator app

#### **Incorrect Code Format**
- **Problem**: Using spaces, dashes, or incorrect number of digits
- **Solution**: Use exactly 6 digits without any spaces or special characters

#### **Code Already Used**
- **Problem**: TOTP codes can typically only be used once within their validity window
- **Solution**: Wait for the next code to generate (usually 30 seconds)

### 2. Testing the 2FA Flow

#### **Step 1: Enable 2FA**
```http
POST https://localhost:7001/api/Auth/2fa/enable
Content-Type: application/json

{
  "userId": "YOUR_USER_ID_HERE"
}
```

**Expected Response:**
```json
{
  "sharedKey": "ABCDEFGHIJKLMNOP",
  "authenticatorUri": "otpauth://totp/App:user@example.com?secret=ABCDEFGHIJKLMNOP&issuer=App"
}
```

#### **Step 2: Set up Authenticator App**
1. Use Google Authenticator, Microsoft Authenticator, or any TOTP app
2. Scan the QR code generated from the `authenticatorUri` OR
3. Manually enter the `sharedKey` in your app

#### **Step 3: Verify the Setup**
```http
POST https://localhost:7001/api/Auth/2fa/verify
Content-Type: application/json

{
  "userId": "YOUR_USER_ID_HERE",
  "code": "123456"
}
```

Replace `123456` with the current 6-digit code from your authenticator app.

### 3. Implementation Changes Made

The following changes were made to fix the original issue:

1. **Added `VerifyTwoFactorTokenAsync` method** to properly verify TOTP codes
2. **Removed dependency on sign-in state** - the original code expected the user to be in a 2FA sign-in state
3. **Added validation** to prevent enabling 2FA if already enabled
4. **Improved error messages** for better debugging

### 4. Debugging Tips

#### **Check User Exists**
Ensure the `userId` you're using exists in the database.

#### **Check Authenticator Key**
After calling enable 2FA, verify that the user has an authenticator key:
```csharp
var key = await userService.GetAuthenticatorKeyAsync(user);
// key should not be null or empty
```

#### **Test with Manual Time**
For testing, you can create a method that accepts a specific time to generate codes for debugging.

#### **Enable Logging**
Add logging to see what's happening:
```csharp
public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
{
    _logger.LogInformation("Verifying 2FA for user {UserId} with code {Code}", request.UserId, request.Code);
    
    var user = await userService.FindByIdAsync(request.UserId);
    if (user == null)
    {
        _logger.LogWarning("User {UserId} not found", request.UserId);
        return new Response { Succeeded = false, Message = "User not found." };
    }

    var key = await userService.GetAuthenticatorKeyAsync(user);
    _logger.LogInformation("User {UserId} has authenticator key: {HasKey}", request.UserId, !string.IsNullOrEmpty(key));
    
    // ... rest of the method
}
```

### 5. Common Authenticator Apps for Testing

- **Google Authenticator** (iOS/Android)
- **Microsoft Authenticator** (iOS/Android)
- **Authy** (iOS/Android/Desktop)
- **1Password** (if you have a subscription)
- **Bitwarden** (free tier available)

### 6. Manual Testing with Online Tools

For debugging purposes, you can use online TOTP generators:
1. Go to a TOTP generator website
2. Enter your shared key
3. Generate codes to test with

**Note**: Only use this for development/testing, never in production!

## Next Steps

1. Test the updated code with the new implementation
2. Use the provided HTTP requests to test the complete flow
3. Check server and device time synchronization
4. Verify the authenticator app is set up correctly with the shared key
