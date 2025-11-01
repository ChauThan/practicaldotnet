# Electron .NET Monorepo

This project integrates an Electron client with a .NET server in a monorepo structure.

## Prerequisites

- Node.js (for Electron client)
- .NET 9.0 SDK (for server)

## Development

1. Build the server:
   ```bash
   npm run build:server
   ```

2. Start the application:
   ```bash
   npm run dev
   ```

This will build the server and start the Electron client, which will automatically launch the server.

## Building for Production

Build both components:
```bash
npm run build
```

This creates a packaged Electron app with the server executable included.

**Note:** On Windows, electron-builder may require administrator privileges for code signing tools. If you encounter permission errors, run the command prompt as Administrator or disable Windows Developer Mode symbolic link restrictions.

## Project Structure

- `Client/` - Electron frontend application
- `Server/` - ASP.NET Core Web API server
- `package.json` - Root monorepo configuration

## API

The server provides:
- `GET /api/status` - Server status and port information
- `GET /weatherforecast` - Sample weather forecast data