# Mams Release Guide

This document explains how to create and publish releases for the Mams application via GitHub Releases.

---

## How It Works

The release process is **fully automated** using GitHub Actions. Here's the workflow:

```
???????????????????????????????????????????????????????????????????????????????
?                           RELEASE WORKFLOW                                  ?
???????????????????????????????????????????????????????????????????????????????
?                                                                             ?
?   1. Developer creates a Git tag (e.g., v1.0.0)                             ?
?                         ?                                                   ?
?                         ?                                                   ?
?   2. Developer pushes the tag to GitHub                                     ?
?                         ?                                                   ?
?                         ?                                                   ?
?   3. GitHub Actions workflow is triggered automatically                     ?
?                         ?                                                   ?
?                         ?                                                   ?
?   4. Workflow builds the app (self-contained, win-x64)                      ?
?                         ?                                                   ?
?                         ?                                                   ?
?   5. Workflow creates ZIP archive                                           ?
?                         ?                                                   ?
?                         ?                                                   ?
?   6. Workflow publishes a new GitHub Release with the ZIP attached         ?
?                                                                             ?
???????????????????????????????????????????????????????????????????????????????
```

The application is published as a **self-contained .NET application** - no .NET runtime installation required on the user's machine.

---

## Creating a Release (Command Line)

### Step-by-Step Process

#### 1. Make sure your code is ready

```bash
# Check your current status
git status

# Make sure all changes are committed
git add .
git commit -m "Prepare release v1.0.0"

# Push your changes to the remote repository
git push origin work-in-progess
```

#### 2. Create and push a tag

```bash
# Create an annotated tag (recommended)
git tag -a v1.0.0 -m "Release version 1.0.0"

# Push the tag to GitHub
git push origin v1.0.0
```

#### 3. Monitor the release

1. Go to your GitHub repository: https://github.com/C0D3X-25/Mams
2. Click on the **Actions** tab
3. Watch the "Build and Release" workflow run
4. Once complete, check the **Releases** section for your new release

### Quick Command Reference

| Action | Command |
|--------|---------|
| Create a lightweight tag | `git tag v1.0.0` |
| Create an annotated tag | `git tag -a v1.0.0 -m "Release message"` |
| Push a specific tag | `git push origin v1.0.0` |
| Push all tags | `git push origin --tags` |
| List all tags | `git tag` |
| Show tag details | `git show v1.0.0` |

---

## Fixing Mistakes with Tags

### Delete a Tag (Before Pushing)

If you created a tag locally but haven't pushed it yet:

```bash
# Delete the local tag
git tag -d v1.0.0
```

### Delete a Tag (After Pushing)

If you already pushed the tag to GitHub:

```bash
# Step 1: Delete the tag on GitHub (remote)
git push origin --delete v1.0.0

# Step 2: Delete the local tag
git tag -d v1.0.0
```

### Replace an Existing Tag

If you need to move a tag to a different commit:

```bash
# Step 1: Delete the remote tag
git push origin --delete v1.0.0

# Step 2: Delete the local tag
git tag -d v1.0.0

# Step 3: Create a new tag on the correct commit
git tag -a v1.0.0 -m "Release version 1.0.0"

# Step 4: Push the new tag
git push origin v1.0.0
```

### Delete the GitHub Release

If a release was created from a bad tag:

1. Go to https://github.com/C0D3X-25/Mams/releases
2. Find the release you want to delete
3. Click on the release title
4. Click **Delete** (trash icon) in the top right
5. Confirm deletion

Then delete the tag using the commands above and recreate it properly.

---

## Alternative: Manual Workflow Dispatch

You can also trigger a release manually without creating a tag:

1. Go to your GitHub repository
2. Click on **Actions** tab
3. Select **Build and Release** workflow
4. Click **Run workflow**
5. Enter the version number (e.g., `1.0.0`)
6. Click **Run workflow**

---

## Local Build (For Testing)

To build the application locally before creating a release:

```powershell
dotnet publish Mams_App/Mams_App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=false `
  -p:UseAppHost=true `
  -p:Version=1.0.0
```

Output location: `Mams_App/bin/Release/net9.0-windows/win-x64/publish/`

---

## What Gets Distributed

- Main application executable (self-contained, no .NET required)
- All required resources and dependencies
- Ready to run after extraction

---

## Installation Instructions for End Users

1. Download `Mams_vX.X.X_Portable.zip` from the latest GitHub Release
2. Extract to any folder
3. Run `Mams_App.exe`
4. If Windows SmartScreen appears:
   - Click "More info"
   - Click "Run anyway"

---

## Requirements for End Users

- Windows 10 or later (64-bit)
- No additional software required (runtime is bundled)

---

## Versioning Convention

Use [Semantic Versioning](https://semver.org/):

- **MAJOR.MINOR.PATCH** (e.g., `v1.2.3`)
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

Examples:
- `v1.0.0` - Initial release
- `v1.1.0` - Added new feature
- `v1.1.1` - Bug fix
- `v2.0.0` - Breaking change

---

## Legacy Files

The `MamsSetup.iss` file is the old Inno Setup script. It's kept for reference but is no longer used.
