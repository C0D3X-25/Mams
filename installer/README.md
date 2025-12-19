# Mams Installer

This folder contains the Inno Setup script to create a Windows installer (`setup.exe`) for the Mams application.

## How It Works

The installer is automatically built via GitHub Actions when you:
1. Push a tag starting with `v` (e.g., `v1.0.0`)
2. Manually trigger the workflow with a version number

## Creating a Release

### Option 1: Using Git Tags (Recommended)

```bash
# Tag your release
git tag v1.0.0

# Push the tag to GitHub
git push origin v1.0.0
```

This will automatically:
1. Build the application
2. Create the installer (`MamsSetup_v1.0.0.exe`)
3. Create a portable ZIP file
4. Publish a GitHub release with both files

### Option 2: Manual Workflow Dispatch

1. Go to your GitHub repository
2. Click on "Actions" tab
3. Select "Build and Release" workflow
4. Click "Run workflow"
5. Enter the version number (e.g., `1.0.0`)
6. Click "Run workflow"

## Local Build (Optional)

To build the installer locally:

1. Install [Inno Setup](https://jrsoftware.org/isdl.php)

2. Publish the application:
   ```bash
   dotnet publish Mams_App/Mams_App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
   ```

3. Build the installer:
   ```bash
   "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DAppVersion="1.0.0" installer/MamsSetup.iss
   ```

The installer will be created in `installer/Output/`.

## What Gets Installed

- Main application executable (self-contained, no .NET required on user's machine)
- All required resources and dependencies
- Start Menu shortcuts
- Optional Desktop shortcut
- Uninstaller

## Requirements for End Users

- Windows 10 or later (64-bit)
- No additional software required (runtime is bundled)
