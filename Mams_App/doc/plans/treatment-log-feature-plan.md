# Treatment Log Feature Plan (Journal des Traitements)

## Overview

Add the ability to manage beehive treatments via a treatment log. This feature allows the user to record treatments applied to beehives, listing the treatment product, dosage per hive, and the automatically calculated total dose for the beehive.

**Context:** A beehive (rucher) contains multiple hives (ruches). Treatments are applied per hive, and the total dose is calculated based on the number of hives treated.

---

## Data Model

### New Database Table: `treatments`

| Column                | Type           | Constraint                                  | Description                               |
|-----------------------|----------------|---------------------------------------------|-------------------------------------------|
| `treatment_id`        | INT            | PRIMARY KEY AUTO_INCREMENT                  | Unique identifier                         |
| `treatment_date`      | DATE           | NOT NULL                                    | Date the treatment was applied            |
| `treatment_hive_count`| INT            | NOT NULL                                    | Number of hives treated                   |
| `treatment_dose_per_hive` | DECIMAL(9,2) | NOT NULL                                  | Dose per hive in ml (entered by user)     |
| `fk_beehive_id`       | INT            | NOT NULL, FK → `beehives(beehive_id)`       | The beehive (rucher) treated              |
| `fk_product_id`       | INT            | NOT NULL, FK → `products(product_id)`       | The treatment product used                |

> **Calculated field (not stored):** `treatment_dose_total` = `treatment_hive_count` × `treatment_dose_per_hive` (calculated in C# code, displayed in UI and PDF).

### SQL (to add at the end of `init.sql`)

```sql
CREATE TABLE IF NOT EXISTS treatments (
    treatment_id INT PRIMARY KEY AUTO_INCREMENT,
    treatment_date DATE NOT NULL,
    treatment_hive_count INT NOT NULL,
    treatment_dose_per_hive DECIMAL(9,2) NOT NULL,
    fk_beehive_id INT NOT NULL,
    fk_product_id INT NOT NULL,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id),
    FOREIGN KEY (fk_product_id) REFERENCES products(product_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_treatment_date ON treatments(treatment_date);
```

---

## Architecture — Files to Create & Modify

Following the existing project conventions (naming, patterns, folder structure).

### New Files (folder: `Mams_App/src/treatments/`)

| File                           | Role                        | Pattern to Follow                        |
|--------------------------------|-----------------------------|------------------------------------------|
| `TreatmentItem.cs`            | Data item (POCO)            | `BeehiveItem.cs`                         |
| `TreatmentModel.cs`           | DB operations (CRUD + filter) | `BeehiveModel.cs` + `ResumeModel.cs` (for filtering) |
| `ListTreatmentPage.xaml`      | List view (XAML)            | `ResumePage.xaml` (filter bar + list + totals) |
| `ListTreatmentPage.xaml.cs`   | Code-behind                 | `ResumePage.xaml.cs`                     |
| `ListTreatmentController.cs`  | List controller             | `ResumeController.cs` (filters, sort, PDF from list) |
| `SaveTreatmentPage.xaml`      | Save/Edit view (XAML)       | `SaveBeehivePage.xaml` + `SaveProfitPage.xaml` (for ComboBoxes) |
| `SaveTreatmentPage.xaml.cs`   | Code-behind                 | `SaveBeehivePage.xaml.cs`                |
| `SaveTreatmentController.cs`  | Save controller             | `SaveBeehiveController.cs`               |
| `TreatmentPdfTemplate.cs`     | PDF generation (QuestPDF)   | `InvoiceProfitTemplate.cs`               |

### New Files (folder: `Mams_Test/treatments/`)

| File                           | Role                        |
|--------------------------------|-----------------------------|
| `TreatmentItemTests.cs`       | Unit tests for TreatmentItem |

### Files to Modify

| File                                          | Change                                                         |
|-----------------------------------------------|----------------------------------------------------------------|
| `Mams_App/database/init.sql`                  | Add `treatments` table                                         |
| `Mams_App/resources/localization/fr.json`     | Add all treatment-related localization keys                    |
| `Mams_App/src/navigations/UCMenuController.cs`| Add treatment navigation command + page active tracking        |
| `Mams_App/src/mainWindow/userControls/UCMenu.xaml` | Add treatment menu button                                |
| `Mams_App/src/invoices/SInvoicePdfService.cs` | Add `generateAndOpenTreatmentPdf()` method                    |

---

## Detailed Implementation Steps

### Step 1 — Database Schema

**File:** `Mams_App/database/init.sql`

Add the `treatments` table after the `receipts_products` table (before the `users` table):

```sql
CREATE TABLE IF NOT EXISTS treatments (
    treatment_id INT PRIMARY KEY AUTO_INCREMENT,
    treatment_date DATE NOT NULL,
    treatment_hive_count INT NOT NULL,
    treatment_dose_per_hive DECIMAL(9,2) NOT NULL,
    fk_beehive_id INT NOT NULL,
    fk_product_id INT NOT NULL,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id),
    FOREIGN KEY (fk_product_id) REFERENCES products(product_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_treatment_date ON treatments(treatment_date);
```

---

### Step 2 — Item (POCO)

**File:** `Mams_App/src/treatments/TreatmentItem.cs`

```csharp
public class TreatmentItem : ABaseItem
{
    public int treatment_id { get; set; } = 0;
    public string treatment_date { get; set; } = string.Empty;
    public int treatment_hive_count { get; set; } = 0;
    public decimal treatment_dose_per_hive { get; set; } = 0;
    public int fk_beehive_id { get; set; } = 0;
    public int fk_product_id { get; set; } = 0;

    // Joined fields (not stored in treatments table)
    public string beehive_name { get; set; } = string.Empty;
    public string beehive_number { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public string region_name { get; set; } = string.Empty;

    // Calculated field
    public decimal treatment_dose_total => treatment_hive_count * treatment_dose_per_hive;
}
```

---

### Step 3 — Model (DB operations)

**File:** `Mams_App/src/treatments/TreatmentModel.cs`

Key methods to implement:

| Method                    | Description                                                  |
|---------------------------|--------------------------------------------------------------|
| `saveItem(TreatmentItem)` | INSERT or UPDATE with parameters                            |
| `deleteItem(string id)`   | Hard delete (no archive for treatments)                     |
| `getItemByID(string id)`  | SELECT with JOINs on beehives + products                    |
| `getAllItems()`            | SELECT all with JOINs, ordered by date DESC                 |
| `getFilteredItems(...)`   | Filtered query (by beehive, product, year) for the list page |

SQL pattern for SELECT (with JOINs):
```sql
SELECT t.treatment_id, t.treatment_date, t.treatment_hive_count,
       t.treatment_dose_per_hive, t.fk_beehive_id, t.fk_product_id,
       b.beehive_name, b.beehive_number, p.product_name,
       r.region_name
FROM treatments t
JOIN beehives b ON t.fk_beehive_id = b.beehive_id
JOIN products p ON t.fk_product_id = p.product_id
LEFT JOIN regions r ON b.fk_region_id = r.region_id
ORDER BY t.treatment_date DESC;
```

SQL pattern for filtered query:
```sql
-- Add WHERE clauses based on filter:
WHERE t.fk_beehive_id = @beehiveId        -- if filtering by beehive
WHERE t.fk_product_id = @productId         -- if filtering by product
WHERE b.fk_region_id = @regionId           -- if filtering by region
WHERE YEAR(t.treatment_date) = @year       -- if filtering by year
```

---

### Step 4 — List View (Resume-style)

**File:** `Mams_App/src/treatments/ListTreatmentPage.xaml`

Layout structure (following `ResumePage.xaml` pattern):

```
┌──────────────────────────────────────────────────────┐
│ Header: "Journal des Traitements"                    │
├──────────┬───────────────────────────────────────────┤
│          │ Filter Bar:                               │
│          │ [Search by ▼] [Filter item ▼] [Year ▼] [X]│
│  Menu    │                                           │
│          │ ListView (treatment list):                │
│          │ Date | Rucher | N° | Région | Ruches |   │
│          │ Produit | Dose/ruche | Dose totale        │
│          │                                           │
│          │ Bottom: [PDF] [Delete]    [Edit] [New]   │
├──────────┴───────────────────────────────────────────┤
│ Footer                                               │
└──────────────────────────────────────────────────────┘
```

ListView columns:

| Column Header         | Binding                      | Notes                                |
|-----------------------|------------------------------|--------------------------------------|
| Date                  | `treatment_date`             | Sortable                             |
| Rucher                | `beehive_name`               | Sortable                             |
| N°                    | `beehive_number`             | Sortable                             |
| Région                | `region_name`                | Sortable (joined via beehive)        |
| Ruches traitées       | `treatment_hive_count`       | Sortable                             |
| Produit               | `product_name`               | Sortable                             |
| Dose/ruche (ml)       | `treatment_dose_per_hive`    | Sortable                             |
| Dose totale (ml)      | `treatment_dose_total`       | Calculated, sortable                 |

Buttons:
- **PDF** — Generates a PDF of the current filtered list (via `SInvoicePdfService`)
- **Delete** — Deletes the selected treatment (hard delete)
- **Edit** — Navigates to `SaveTreatmentPage` with selected item ID
- **New** — Navigates to `SaveTreatmentPage` without ID (new entry)

**File:** `Mams_App/src/treatments/ListTreatmentController.cs`

Properties and commands:
- Filter bar (same pattern as `ResumeController`): `m_list_table`, `m_selected_table`, `m_list_filter_item`, `m_selected_filter_item`, `m_list_year`, `m_selected_year`
- List: `m_list_items` (ObservableCollection<TreatmentItem>)
- Selected: `m_selected_item`
- Commands: `m_add_command`, `m_modify_command`, `m_delete_command`, `m_double_click_command`, `m_sort_command`, `m_generate_pdf_command`, `m_clear_search_command`
- Sort: `m_sorted_column`, `m_sort_direction`

Filter tables (simplified vs Resume — only relevant filters):
```csharp
new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = string.Empty },
new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = Loc.Get("Search.Beehive") },
new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = Loc.Get("Search.Product") },
new(){ m_name_in_database = EDatabaseTableName.REGION, m_name_to_display = Loc.Get("Search.Region") }
```

---

### Step 5 — Save View

**File:** `Mams_App/src/treatments/SaveTreatmentPage.xaml`

Layout structure (following `SaveBeehivePage.xaml` + `SaveProfitPage.xaml` for ComboBoxes):

```
┌──────────────────────────────────────────────────────┐
│ Header: "Traitement"                                 │
├──────────┬───────────────────────────────────────────┤
│          │                                           │
│          │  Left Column:           Right Column:     │
│  Menu    │  Date*: [________]      ID: [hidden]     │
│          │  Rucher*: [▼ combo]                       │
│          │  Nb de ruches*: [____]                    │
│          │  Produit*: [▼ combo]                      │
│          │  Dose/ruche (ml)*: [__]                   │
│          │  Dose totale (ml): [=calculated=]         │
│          │                                           │
│          │  [Cancel]                        [Save]   │
├──────────┴───────────────────────────────────────────┤
│ Footer                                               │
└──────────────────────────────────────────────────────┘
```

Fields:
- **Date** (`UCLabelTextBox`) — bound to `treatment_date`, format dd.MM.yyyy
- **Beehive** (ComboBox) — bound to `m_list_beehive` / `m_selected_beehive`, displays `beehive_name` + `beehive_number`
- **Number of hives** (`UCLabelTextBox`) — bound to `treatment_hive_count`
- **Product** (ComboBox) — bound to `m_list_product` / `m_selected_product`, displays `product_name`
- **Dose per hive** (`UCLabelTextBox`) — bound to `treatment_dose_per_hive`
- **Total dose** (read-only Label) — bound to `treatment_dose_total` (calculated: hive_count × dose_per_hive)

**File:** `Mams_App/src/treatments/SaveTreatmentController.cs`

Key properties:
- `m_treatment` (`TreatmentItem`) — the current treatment being edited
- `m_list_beehive` (`ObservableCollection<BeehiveItem>`) — loaded from `BeehiveModel.getActiveBeehives()`
- `m_selected_beehive` (`BeehiveItem`) — selected beehive, updates `m_treatment.fk_beehive_id`
- `m_list_product` (`ObservableCollection<ProductItem>`) — loaded from `ProductModel.getActiveProducts()`
- `m_selected_product` (`ProductItem`) — selected product, updates `m_treatment.fk_product_id`
- `m_treatment_dose_total_ui` (string) — formatted calculated field for display

Key commands:
- `m_save_command` — validates and saves via `TreatmentModel.saveItem()`
- `m_abort_command` — navigates back

Validation (`canSave`):
- Date is not empty
- Beehive is selected (fk_beehive_id > 0)
- Product is selected (fk_product_id > 0)
- Hive count > 0
- Dose per hive > 0

Implements `ICompareState` for unsaved changes detection.

---

### Step 6 — PDF Generation

**File:** `Mams_App/src/treatments/TreatmentPdfTemplate.cs`

Uses QuestPDF (same as `InvoiceProfitTemplate.cs`).

Document structure:
```
┌──────────────────────────────────────┐
│ Header: "Journal des Traitements"    │
│         Date range / filter info     │
├──────────────────────────────────────┤
│                                      │
│ Table:                               │
│ # | Date | Rucher | N° | Région |   │
│   | Ruches | Produit | Dose/ruche │
│   | Dose tot                         │
│                                      │
│ Footer: page X / Y                   │
└──────────────────────────────────────┘
```

**File:** `Mams_App/src/invoices/SInvoicePdfService.cs`

Add a new method:
```csharp
public static void generateAndOpenTreatmentPdf(ObservableCollection<TreatmentItem> items)
```

This method:
1. Creates the `TreatmentPdfTemplate` with the list of treatments
2. Generates the PDF to the same directory as invoices
3. Opens the generated PDF

This is called from the **List page** (not the Save page), triggered by the PDF button.

---

### Step 7 — Navigation

**File:** `Mams_App/src/navigations/UCMenuController.cs`

Add:
```csharp
// New property
private bool _m_is_treatment_page_active;
public bool m_is_treatment_page_active { ... }

// New command
public ICommand m_navigate_list_treatment_command { get; set; }

// In constructor
m_navigate_list_treatment_command = new RelayCommand(navigateToListTreatment);

// In OnPageChanged
m_is_treatment_page_active = pageType == typeof(ListTreatmentPage);

// New method
private void navigateToListTreatment(object? obj)
{
    SPageNavigationController.navigateTo(new ListTreatmentPage(), true);
}
```

**File:** `Mams_App/src/mainWindow/userControls/UCMenu.xaml`

Add a new menu button (after Beehives, before Product Categories). Update Grid.RowDefinitions to add one more row:

```xml
<local:UCPageNavigationMenuButton 
    Content="{loc:Translate Nav.Treatments}"
    Grid.Row="7"
    Command="{Binding m_navigate_list_treatment_command}"
    IsCurrentPage="{Binding m_is_treatment_page_active}"/>
```

> Shift existing rows 7-9 to 8-10.

---

### Step 8 — Localization

**File:** `Mams_App/resources/localization/fr.json`

Keys to add:

```json
"_comment_treatment": "Treatment - Treatment log related labels",
"Treatment.Title": "Traitements",
"Treatment.Date": "Date du traitement",
"Treatment.HiveCount": "Nb de ruches traitées",
"Treatment.DosePerHive": "Dose/ruche (ml)",
"Treatment.DoseTotal": "Dose totale (ml)",
"Treatment.Product": "Produit de traitement",
"Treatment.Beehive": "Rucher",

"Nav.Treatments": "Traitements",
"Page.ListTreatments": "Journal des Traitements",
"Page.SaveTreatment": "Enregistrer un Traitement",
"Button.NewTreatment": "Nouveau Traitement",
"Search.Treatment": "Traitement",

"Column.Date": "Date",
"Column.HiveCount": "Ruches traitées",
"Column.DosePerHive": "Dose/ruche (ml)",
"Column.DoseTotal": "Dose totale (ml)",
"Column.Product": "Produit",
"Column.Beehive": "Rucher",

"Form.Beehive": "Rucher*",
"Form.HiveCount": "Nb de ruches*",
"Form.DosePerHive": "Dose/ruche (ml)*",
"Form.DoseTotal": "Dose totale (ml)",
"Form.TreatmentProduct": "Produit de traitement*",

"Pdf.TreatmentTitle": "Journal des Traitements",
"Pdf.TreatmentFilename": "Traitements_{0}.pdf"
```

---

### Step 9 — Unit Tests

**File:** `Mams_Test/treatments/TreatmentItemTests.cs`

Tests to implement:
- `TreatmentItem_DefaultValues_ShouldBeCorrect` — verify all defaults
- `TreatmentItem_SetProperties_ShouldReturnCorrectValues` — verify property setters
- `TreatmentItem_DoseTotal_ShouldBeCalculated` — verify `treatment_dose_total` = `hive_count × dose_per_hive`
- `TreatmentItem_DoseTotal_WithZeroHives_ShouldBeZero`
- `TreatmentItem_DoseTotal_WithZeroDose_ShouldBeZero`

---

## Implementation Order

| #  | Task                                      | Dependencies |
|----|-------------------------------------------|--------------|
| 1  | `init.sql` — add `treatments` table       | —            |
| 2  | `TreatmentItem.cs`                        | —            |
| 3  | `TreatmentItemTests.cs`                   | Step 2       |
| 4  | `TreatmentModel.cs`                       | Step 1, 2    |
| 5  | `SaveTreatmentController.cs`              | Step 2, 4    |
| 6  | `SaveTreatmentPage.xaml` + `.xaml.cs`     | Step 5       |
| 7  | `ListTreatmentController.cs`              | Step 2, 4    |
| 8  | `ListTreatmentPage.xaml` + `.xaml.cs`     | Step 7       |
| 9  | `TreatmentPdfTemplate.cs`                 | Step 2       |
| 10 | `SInvoicePdfService.cs` — add method      | Step 9       |
| 11 | `UCMenuController.cs` — add navigation    | Step 8       |
| 12 | `UCMenu.xaml` — add menu button           | Step 11      |
| 13 | `fr.json` — add localization keys         | —            |
| 14 | Build & test                              | All          |

---

## Migration Note (Existing Databases)

For users with existing databases, run this ALTER statement manually or via a migration step:

```sql
CREATE TABLE IF NOT EXISTS treatments (
    treatment_id INT PRIMARY KEY AUTO_INCREMENT,
    treatment_date DATE NOT NULL,
    treatment_hive_count INT NOT NULL,
    treatment_dose_per_hive DECIMAL(9,2) NOT NULL,
    fk_beehive_id INT NOT NULL,
    fk_product_id INT NOT NULL,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id),
    FOREIGN KEY (fk_product_id) REFERENCES products(product_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_treatment_date ON treatments(treatment_date);
```
