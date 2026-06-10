using Microsoft.Data.Sqlite;

var dbPath = @"C:\Users\abc29\Desktop\github\lims-auth\backend\LimsAuth.Api\lims.db";
var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

var cmd = conn.CreateCommand();
cmd.CommandText = "PRAGMA table_info(teaching_applications)";
var reader = cmd.ExecuteReader();
Console.WriteLine("=== teaching_applications schema ===");
while (reader.Read()) {
    Console.WriteLine($"  cid={reader["cid"]} name={reader["name"]} type={reader["type"]} notnull={reader["notnull"]} dflt_value={reader["dflt_value"]}");
}

cmd.CommandText = "SELECT * FROM teaching_applications LIMIT 1";
reader.Close();
reader = cmd.ExecuteReader();
Console.WriteLine("\n=== Sample row (raw columns) ===");
for (int i = 0; i < reader.FieldCount; i++) {
    Console.WriteLine($"  [{i}] {reader.GetName(i)} = {reader.GetValue(i)}");
}
reader.Close();

// Check if start_week/end_week exist
cmd.CommandText = "SELECT COUNT(*) FROM pragma_table_info('teaching_applications') WHERE name IN ('start_week', 'end_week')";
var cnt = Convert.ToInt32(cmd.ExecuteScalar());
Console.WriteLine($"\nstart_week/end_week columns present: {cnt}/2");

conn.Close();
