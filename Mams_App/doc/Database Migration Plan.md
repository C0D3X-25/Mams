# Database Migration Plan

This document explains how to implement automatic database schema migrations so that the database is updated alongside the application when a new version is released.

---

## Problem

Currently, when the app is updated and the database schema changes (new tables, new columns, modified constraints, etc.), there is **no automatic way** to apply those changes to the user's existing database. The `init.sql` file only runs when the database is created for the first time (`isDatabaseCreated()` returns `false`). Existing users keep their old schema.

---

## Goal

When a user updates the app (via GitHub Releases), the database schema should be **automatically migrated** on the next startup — without losing any existing data.

---

## How It Works (Overview)

```
┌─────────────────────────────────────────────────────────────────────┐
│                     APP STARTUP (LauncherStepManager)               │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│   ... existing steps (update check, MariaDB start, etc.) ...        │
│                         │                                           │
│                         ▼                                           │
│   Step 6: CreateDatabase (already exists)                           │
│     → If DB doesn't exist: run init.sql (as today)                  │
│                         │                                           │
│                         ▼                                           │
│   Step 6b: MigrateDatabase  ← NEW STEP                             │
│     → Read current schema version from `schema_version` table       │
│     → Find all migration scripts with version > current             │
│     → For each pending migration (in order):                        │
│         1. Create automatic backup                                  │
│         2. Execute the SQL migration script                         │
│         3. Update `schema_version` table                            │
│     → If a migration fails: restore backup, report error            │
│                         │                                           │
│                         ▼                                           │
│   Step 7: Finalize → Ready                                          │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Implementation Plan

### 1. Create a `schema_version` Table

Add this table to `init.sql` (at the end, after all other tables). It tracks which migrations have been applied.

```sql
CREATE TABLE IF NOT EXISTS schema_version (
    version_id INT PRIMARY KEY AUTO_INCREMENT,
    version_number VARCHAR(20) NOT NULL,
    description VARCHAR(255),
    applied_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Record the initial schema version
INSERT INTO schema_version (version_number, description) VALUES
('1.2.7', 'Initial schema');
```

> **Note:** The `version_number` here should match the app version that first shipped this schema. For existing users who never had `schema_version`, the migration service will create this table and seed it with the baseline version.

---

### 2. Create Migration SQL Scripts

Create a new folder to hold numbered migration scripts:

```
Mams_App/
  database/
    init.sql                        ← existing (for fresh installs)
    migrations/
      V1.3.0__add_example_column.sql
      V1.4.0__create_new_table.sql
      V2.0.0__rename_column.sql
```

**Naming convention:** `V{version}__{description}.sql`
- `V` prefix is mandatory
- Version uses dots: `1.3.0`
- Double underscore `__` separates version from description
- Description uses underscores for spaces
- Files are executed in **version order** (parsed via `System.Version`)

**Example migration script** (`V1.3.0__add_example_column.sql`):

```sql
-- Migration: V1.3.0
-- Description: Add example column to entities table

ALTER TABLE entities ADD COLUMN entity_notes TEXT AFTER entity_address;
```

**Rules for writing migration scripts:**
- Each script must be **idempotent when possible** (use `IF NOT EXISTS`, `IF EXISTS`, etc.)
- Never delete user data without confirmation
- Always test locally before releasing
- One migration per version — if a version has multiple changes, combine them in one file

---

### 3. Add Migration Scripts to the Build Output

In `Mams_App.csproj`, add an `<ItemGroup>` to copy migration files alongside the app:

```xml
<ItemGroup>
  <Content Include="database\migrations\*.sql">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

This ensures migration files are included in the published ZIP.

---

### 4. Create `SDatabaseMigrationService.cs`

Create a new static service class: `Mams_App/src/databaseOperations/SDatabaseMigrationService.cs`

**Responsibilities:**
- Read the current `schema_version` from the database
- Discover migration scripts from the `database/migrations/` folder
- Execute pending migrations in order
- Create a backup before each migration (using `SDatabaseBackup`)
- Update `schema_version` after each successful migration
- Handle the **baseline case** (existing users who don't have `schema_version` yet)

**Pseudo-code:**

```csharp
public static class SDatabaseMigrationService
{
    public static async Task<bool> MigrateAsync(CancellationToken ct)
    {
        // 1. Ensure schema_version table exists
        //    (CREATE TABLE IF NOT EXISTS schema_version ...)
        //    If empty → insert baseline version (e.g., "1.2.7")

        // 2. Get the latest applied version
        //    SELECT version_number FROM schema_version
        //    ORDER BY applied_at DESC LIMIT 1

        // 3. Discover migration files from disk
        //    Parse version from filename, sort ascending

        // 4. Filter to only pending migrations (version > current)

        // 5. For each pending migration:
        //    a. Create backup via SDatabaseBackup
        //    b. Read and execute the SQL file
        //    c. INSERT INTO schema_version (version_number, description)
        //    d. Log success

        // 6. Return true if all succeeded
    }
}
```

---

### 5. Add a New Launcher Step

In `ELauncherStep.cs`, add a new step:

```csharp
public enum ELauncherStep
{
    // ... existing steps ...
    CreateDatabase,
    MigrateDatabase,    // ← NEW
    Finalize,
    Ready,
    Failed
}
```

In `LauncherStepManager.cs`, add the migration step **after** `CreateDatabase` and **before** `Finalize`:

```csharp
// Step 6: Create database (existing)
// ...

// Step 6b: Migrate database schema
await ExecuteStepAsync(ELauncherStep.MigrateDatabase, cancellationToken);
if (!await SDatabaseMigrationService.MigrateAsync(cancellationToken))
{
    await FailWithErrorAsync(
        "Failed to migrate the database schema.\nPlease try restarting the application.",
        cancellationToken);
    return;
}

// Step 7: Finalize (existing)
```

Add a status message in `GetStatusMessageForStep`:

```csharp
ELauncherStep.MigrateDatabase => Loc.Get("Launcher.MigratingDatabase")
    ?? "Updating database schema...",
```

---

### 6. Handle Existing Users (Baseline)

When the migration service runs for the first time on an existing user's database:

1. The `schema_version` table **does not exist** yet
2. The service creates it
3. It inserts the **baseline version** (the version before any migration scripts were introduced, e.g., `1.2.7`)
4. Then it applies any migrations with version > `1.2.7`

This means **no migration runs** for existing users until the first version that ships a migration file.

---

### 7. Keep `init.sql` Updated

When adding a migration, **also apply the same change to `init.sql`**. This ensures that brand-new installations get the latest schema directly without needing to run through all historical migrations.

Example workflow when adding a new column:

1. Create `database/migrations/V1.3.0__add_entity_notes.sql`:
   ```sql
   ALTER TABLE entities ADD COLUMN IF NOT EXISTS entity_notes TEXT AFTER entity_address;
   ```
2. Update `database/init.sql` to include `entity_notes` in the `CREATE TABLE entities` statement
3. Update the baseline insert at the bottom of `init.sql` to the new version

---

## Developer Workflow (Step-by-Step)

### Adding a Database Change to a New Release

```
1. Write your migration SQL script
       ↓
2. Save it as: database/migrations/V{version}__{description}.sql
       ↓
3. Update init.sql with the same schema change
       ↓
4. Update version.json with the new version number
       ↓
5. Test locally:
   a. Test fresh install (delete mariadb/data folder → init.sql runs)
   b. Test migration (keep existing data → migration runs)
       ↓
6. Commit, tag, and push (follow the existing release process)
       ↓
7. GitHub Actions builds and publishes the release
       ↓
8. User updates → app starts → migration runs automatically
```

---

## Safety Measures

| Measure | Description |
|---------|-------------|
| **Automatic backup** | A full database backup is created before each migration via `SDatabaseBackup` |
| **Version tracking** | `schema_version` table records exactly which migrations were applied and when |
| **Ordered execution** | Migrations run in strict version order, never skipped |
| **Idempotent scripts** | Use `IF NOT EXISTS` / `IF EXISTS` guards in SQL |
| **Failure handling** | If a migration fails, the app reports the error and stops — the backup can be restored from Settings |
| **No data loss** | Migrations only add/modify schema — never drop tables or columns without explicit intent |

---

## File Summary

| File | Action | Purpose |
|------|--------|---------|
| `database/init.sql` | **Modify** | Add `schema_version` table, keep schema up-to-date for fresh installs |
| `database/migrations/*.sql` | **Create** (as needed) | Individual migration scripts per version |
| `Mams_App.csproj` | **Modify** | Add `<Content>` item to copy migration files to output |
| `src/databaseOperations/SDatabaseMigrationService.cs` | **Create** | Service that discovers and runs pending migrations |
| `src/launcher/ELauncherStep.cs` | **Modify** | Add `MigrateDatabase` step |
| `src/launcher/LauncherStepManager.cs` | **Modify** | Call migration service after `CreateDatabase` step |

---

## Example Timeline

| Version | Change | Migration File | What Happens |
|---------|--------|----------------|--------------|
| v1.2.7 | Current release | *(none — this is the baseline)* | `schema_version` table gets created and seeded |
| v1.3.0 | Add `entity_notes` column | `V1.3.0__add_entity_notes.sql` | Existing users: migration runs. New users: `init.sql` already has it |
| v1.4.0 | Add `fees` table column | `V1.4.0__add_fee_discount.sql` | Users on v1.2.7: both migrations run. Users on v1.3.0: only this one runs |
| v2.0.0 | Major schema change | `V2.0.0__restructure_receipts.sql` | All pending migrations run in order up to v2.0.0 |

---

## Why Not Use a Library (DbUp, FluentMigrator, etc.)?

A lightweight **custom solution** is recommended here because:

- The app already manages MariaDB lifecycle (`SMariaDbPortableService`)
- The app already has backup/restore (`SDatabaseBackup`)
- The app already has a step-based launcher (`LauncherStepManager`)
- Adding a NuGet package adds complexity for a straightforward "run SQL files in order" task
- Full control over error handling and backup integration

If the number of migrations grows significantly in the future, consider adopting **DbUp** (lightweight, SQL-script-based, supports MySQL) as a drop-in replacement.
