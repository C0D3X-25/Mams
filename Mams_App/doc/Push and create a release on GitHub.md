# Mams Installer

This folder contains legacy Inno Setup files. The application is now distributed as a **self-contained ZIP** via GitHub Releases.

## Distribution Method

The application is published as a **self-contained .NET application** - no .NET runtime installation required on the user's machine.

### How It Works

```
???????????????????     Download ZIP      ????????????????????
?   User          ? ???????????????????????  GitHub Releases ?
?                 ?                       ?                  ?
???????????????????                       ????????????????????
         ?
         ?
   Extract & Run
   Mams_App.exe
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
1. Build the application (self-contained, win-x64)
2. Create ZIP archives
3. Publish a GitHub release with the files

### Option 2: Manual Workflow Dispatch

1. Go to your GitHub repository
2. Click on "Actions" tab
3. Select "Build and Release" workflow
4. Click "Run workflow"
5. Enter the version number (e.g., `1.0.0`)
6. Click "Run workflow"

## Local Build

To build the application locally:

```powershell
dotnet publish Mams_App/Mams_App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=false `
  -p:UseAppHost=true `
  -p:Version=1.0.0
```

The files will be created in `Mams_App/bin/Release/net10.0-windows/win-x64/publish/`.

## What Gets Distributed

- Main application executable (self-contained, no .NET required)
- All required resources and dependencies
- Ready to run after extraction

## Installation Instructions for End Users

1. Download `Mams_vX.X.X_Portable.zip` from the latest GitHub Release
2. Extract to any folder
3. Run `Mams_App.exe`
4. If Windows SmartScreen appears:
   - Click "More info"
   - Click "Run anyway"

## Requirements for End Users

- Windows 10 or later (64-bit)
- No additional software required (runtime is bundled)

## Legacy Files

The `MamsSetup.iss` file is the old Inno Setup script. It's kept for reference but is no longer used.
