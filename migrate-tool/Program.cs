using Microsoft.Data.Sqlite;

var dbPath = args.Length > 0 ? args[0] : "lims.db";
var connString = $"Data Source={dbPath}";

using var conn = new SqliteConnection(connString);
conn.Open();

var columnsToAdd = new[]
{
    ("phone", "TEXT"),
    ("recipient_id", "TEXT"),
    ("recipient_name", "TEXT"),
    ("supervisor_approval_status", "TEXT"),
    ("supervisor_approval_remark", "TEXT"),
    ("supervisor_approval_date", "TEXT"),
    ("admin_approval_status", "TEXT"),
    ("admin_approval_remark", "TEXT"),
    ("admin_approval_date", "TEXT"),
    ("return_checker_id", "TEXT"),
    ("return_check_date", "TEXT"),
    ("usage_location", "TEXT"),
    ("approver_id", "TEXT"),
    ("actual_borrow_date", "TEXT"),
    ("actual_return_date", "TEXT"),
    ("is_renewed", "INTEGER DEFAULT 0"),
    ("renew_approval_status", "TEXT"),
    ("renewed_return_date", "TEXT"),
    ("return_condition", "TEXT"),
    ("return_remarks", "TEXT"),
};

foreach (var (colName, colType) in columnsToAdd)
{
    using var cmd = conn.CreateCommand();
    cmd.CommandText = $"ALTER TABLE equipment_borrow_records ADD COLUMN {colName} {colType}";
    try
    {
        cmd.ExecuteNonQuery();
        Console.WriteLine($"[OK] Added: {colName}");
    }
    catch (SqliteException ex)
    {
        Console.WriteLine($"[SKIP] {colName}: {ex.Message}");
    }
}

// Also ensure the recipients table has the navigation property
using (var cmd2 = conn.CreateCommand())
{
    cmd2.CommandText = "ALTER TABLE equipment_borrow_records ADD COLUMN recipient_id TEXT";
    try { cmd2.ExecuteNonQuery(); Console.WriteLine("[OK] Added: recipient_id"); }
    catch (SqliteException ex) { Console.WriteLine($"[SKIP] recipient_id: {ex.Message}"); }
}

Console.WriteLine("Done!");
