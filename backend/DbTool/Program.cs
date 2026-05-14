using Microsoft.Data.Sqlite;

var dbPath = args.Length > 0 ? args[0] : "lims.db";
Console.WriteLine($"Database: {dbPath}");

using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();
conn.EnableExtensions(true);

void Exec(string sql, string desc)
{
    Console.Write($"\n[SQL] {desc}... ");
    try
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        Console.Write("OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}

var steps = new List<(string sql, string desc)>
{
    // 1. labs 新增字段
    ("ALTER TABLE labs ADD COLUMN seat_count INTEGER NOT NULL DEFAULT 0", "labs: seat_count"),
    ("ALTER TABLE labs ADD COLUMN floor_no INTEGER", "labs: floor_no"),
    ("ALTER TABLE labs ADD COLUMN area REAL", "labs: area"),
    ("ALTER TABLE labs ADD COLUMN room_type TEXT", "labs: room_type"),
    ("ALTER TABLE labs ADD COLUMN photo TEXT", "labs: photo"),
    ("ALTER TABLE labs ADD COLUMN is_available INTEGER NOT NULL DEFAULT 1", "labs: is_available"),
    ("ALTER TABLE labs ADD COLUMN experiment_location_code TEXT", "labs: experiment_location_code"),

    // 2. schedule_entries
    (@"CREATE TABLE schedule_entries (
        id TEXT NOT NULL PRIMARY KEY,
        semester_id TEXT NOT NULL,
        lab_id TEXT NULL,
        week_number INTEGER NOT NULL,
        day_of_week INTEGER NOT NULL,
        period_number INTEGER NOT NULL,
        source TEXT NULL,
        status TEXT NULL,
        reservation_id TEXT NULL,
        teaching_application_id TEXT NULL,
        experiment_task_id TEXT NULL,
        teaching_task_id TEXT NULL,
        course_name TEXT NULL,
        project_name TEXT NULL,
        course_id TEXT NULL,
        teacher_id TEXT NULL,
        teacher_name TEXT NULL,
        class_id TEXT NULL,
        class_name TEXT NULL,
        major_id TEXT NULL,
        major_name TEXT NULL,
        student_count INTEGER NULL,
        remark TEXT NULL,
        has_conflict INTEGER NOT NULL,
        conflict_info TEXT NULL,
        created_at TEXT NOT NULL,
        created_by TEXT NULL,
        updated_at TEXT NOT NULL,
        updated_by TEXT NULL
    )", "CREATE schedule_entries"),

    // 3. reservations
    (@"CREATE TABLE reservations (
        id TEXT NOT NULL PRIMARY KEY,
        semester_id TEXT NOT NULL,
        lab_id TEXT NOT NULL,
        use_date TEXT NOT NULL,
        day_of_week INTEGER NOT NULL,
        period_numbers TEXT NULL,
        week_number INTEGER NOT NULL,
        expected_duration_hours REAL NULL,
        project_name TEXT NOT NULL,
        project_category TEXT NULL,
        remark TEXT NULL,
        applicant_id TEXT NOT NULL,
        applicant_name TEXT NOT NULL,
        applicant_phone TEXT NOT NULL,
        project_leader_id TEXT NULL,
        project_leader_name TEXT NULL,
        project_leader_phone TEXT NULL,
        member_grade TEXT NULL,
        member_class_id TEXT NULL,
        member_class_name TEXT NULL,
        member_count INTEGER NULL,
        status TEXT NOT NULL,
        approval_comment TEXT NULL,
        approved_by TEXT NULL,
        approved_at TEXT NULL,
        is_cancelled INTEGER NOT NULL DEFAULT 0,
        cancel_reason TEXT NULL,
        cancelled_by TEXT NULL,
        cancelled_at TEXT NULL,
        created_at TEXT NOT NULL,
        created_by TEXT NULL,
        updated_at TEXT NOT NULL,
        updated_by TEXT NULL
    )", "CREATE reservations"),

    // 4. schedule_statistics
    (@"CREATE TABLE schedule_statistics (
        id TEXT NOT NULL PRIMARY KEY,
        semester_id TEXT NOT NULL,
        building_id TEXT NULL,
        lab_id TEXT NULL,
        week_number INTEGER NOT NULL,
        total_slots INTEGER NOT NULL DEFAULT 0,
        used_slots INTEGER NOT NULL DEFAULT 0,
        reservation_slots INTEGER NOT NULL DEFAULT 0,
        occupancy_rate REAL NOT NULL DEFAULT 0,
        total_student_count INTEGER NOT NULL DEFAULT 0,
        generated_at TEXT NOT NULL
    )", "CREATE schedule_statistics"),

    // 5. usage_registrations
    (@"CREATE TABLE usage_registrations (
        id TEXT NOT NULL PRIMARY KEY,
        semester_id TEXT NOT NULL,
        lab_id TEXT NULL,
        lab_name TEXT NOT NULL,
        use_date TEXT NOT NULL,
        week_number INTEGER NOT NULL,
        day_of_week INTEGER NOT NULL,
        period_number INTEGER NOT NULL,
        source TEXT NULL,
        schedule_entry_id TEXT NULL,
        reservation_id TEXT NULL,
        teaching_application_id TEXT NULL,
        experiment_task_id TEXT NULL,
        teaching_task_id TEXT NULL,
        course_name TEXT NULL,
        project_name TEXT NULL,
        experiment_item_name TEXT NULL,
        experiment_item_type TEXT NULL,
        planned_hours INTEGER NOT NULL,
        actual_hours REAL NOT NULL,
        class_name TEXT NULL,
        expected_student_count INTEGER NULL,
        actual_student_count INTEGER NULL,
        attendance_record TEXT NULL,
        teaching_condition TEXT NULL,
        equipment_condition TEXT NULL,
        status TEXT NOT NULL,
        reminded_at TEXT NULL,
        filled_by_id TEXT NOT NULL,
        filled_by_name TEXT NOT NULL,
        filled_at TEXT NOT NULL,
        created_at TEXT NOT NULL,
        created_by TEXT NULL,
        updated_at TEXT NOT NULL,
        updated_by TEXT NULL
    )", "CREATE usage_registrations"),

    // 6. 删除 ven_*
    ("DROP TABLE IF EXISTS ven_rooms", "DROP ven_rooms"),
    ("DROP TABLE IF EXISTS ven_buildings", "DROP ven_buildings"),
};

int i = 1;
foreach (var (sql, desc) in steps)
{
    Console.Write($"\n[{i++}/{steps.Count}] {desc}... ");
    try
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        Console.Write("OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"  SQL: {sql.Substring(0, Math.Min(80, sql.Length))}...");
    }
}

// 验证结果
Console.WriteLine("\n\n=== 验证结果 ===");
using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
    using var r = cmd.ExecuteReader();
    Console.WriteLine("Tables:");
    while (r.Read()) Console.WriteLine($"  {r.GetString(0)}");
}

Console.WriteLine("\n=== labs 新增字段 ===");
using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "PRAGMA table_info(labs);";
    using var r = cmd.ExecuteReader();
    while (r.Read()) Console.WriteLine($"  {r.GetString(1)}");
}

Console.WriteLine("\nDone.");
