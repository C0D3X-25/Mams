# Mams Installer

This folder contains legacy Inno Setup files. The application now uses **ClickOnce deployment** with auto-updates via GitHub Releases.

## ClickOnce Deployment

ClickOnce provides automatic updates - when users launch the app, it checks GitHub Releases for new versions and updates automatically.

### How It Works

```
???????????????????     1. Check for updates    ????????????????????
?   Mams App      ? ?????????????????????????????  GitHub Releases ?
?   (on launch)   ?                             ?                  ?
?                 ????????????????????????????? ?  Latest version  ?
???????????????????     2. Download if newer    ????????????????????
         ?
         ?
   3. Install update
      & restart
```

## Creating a Release

### Option 1: Using Git Tags (Recommended)

```bash
# Tag your release
git tag v1.0.0

# Push the tag to GitHub
git push origin v1.0.0
```

This will automatically:
1. Build the ClickOnce application
2. Create the installer files
3. Create a portable ZIP file
4. Publish a GitHub release with all files

### Option 2: Manual Workflow Dispatch

1. Go to your GitHub repository
2. Click on "Actions" tab
3. Select "Build and Release" workflow
4. Click "Run workflow"
5. Enter the version number (e.g., `1.0.0`)
6. Click "Run workflow"

## Local Build

To build the ClickOnce installer locally:

### Using Visual Studio

1. Right-click on `Mams_App` project
2. Select **Publish**
3. Create a new ClickOnce profile or use existing
4. Click **Publish**

### Using Command Line

```powershell
dotnet publish Mams_App/Mams_App.csproj `
  -c Release `
  -p:PublishProfile=ClickOnceProfile `
  -p:ApplicationVersion=1.0.0.0
```

The ClickOnce files will be created in `Mams_App/publish/`.

## What Gets Installed

- Main application executable (self-contained, no .NET required)
- All required resources and dependencies
- Start Menu shortcut
- Desktop shortcut
- Automatic update capability
- Clean uninstall via Control Panel

## Installation Instructions for End Users

### First-time Installation

1. Download `Mams_App.application` from the latest GitHub Release
2. Double-click to install
3. If Windows SmartScreen appears:
   - Click "More info"
   - Click "Run anyway"
4. The app installs and creates shortcuts

### Updates

Updates are **automatic**! When you launch Mams:
- It checks GitHub for new versions
- If found, downloads and installs automatically
- You'll always have the latest version

### Uninstalling

1. Open **Settings** ? **Apps** ? **Installed Apps**
2. Search for "Mams"
3. Click **Uninstall**

Or use **Control Panel** ? **Programs and Features**

## Requirements for End Users

- Windows 10 or later (64-bit)
- No additional software required (runtime is bundled)
- Internet connection for auto-updates

## Troubleshooting

### "Windows protected your PC" (SmartScreen)
This appears because the app isn't signed with a paid certificate. Click "More info" ? "Run anyway".

### Updates not working
Ensure you have internet access. The app checks `https://github.com/C0D3X-25/Mams/releases/latest/download/` for updates.

## Legacy Files

The `MamsSetup.iss` file is the old Inno Setup script. It's kept for reference but is no longer used.
