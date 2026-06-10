using Microsoft.Data.Sqlite;

// Query DB for equipment data
var dbPath = args.Length > 0 ? args[0] : "backend/LimsAuth.Api/lims.db";

using var conn = new SqliteConnection($"Data Source={dbPath}");
await conn.OpenAsync();

using var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT Name, Status, IsActive FROM Equipments";
using var reader = await cmd.ExecuteReaderAsync();

Console.WriteLine("=== Equipments in DB ===");
int count = 0;
while (await reader.ReadAsync())
{
    var name = reader.GetString(0);
    var status = reader.GetString(1);
    var active = reader.GetInt32(2);
    Console.WriteLine($"Name={name,-30} Status={status,-20} Active={active}");
    count++;
}
Console.WriteLine($"\nTotal: {count} devices");
