using Microsoft.Data.Sqlite;

var dbPath = @"C:\Users\abc29\Desktop\github\lims-auth\backend\LimsAuth.Api\lims.db";
using var connection = new SqliteConnection($"Data Source={dbPath}");
connection.Open();

using (var update = connection.CreateCommand())
{
    update.CommandText = @"
UPDATE Equipments
SET Status = CASE
    WHEN Status = '正常' THEN '在库-可用'
    WHEN Status = '维修中' THEN '在库-待维修'
    WHEN Status = '借用中' THEN '借出'
    ELSE Status
END
WHERE Status IN ('正常', '维修中', '借用中');";
    var affected = update.ExecuteNonQuery();
    Console.WriteLine($"UPDATED={affected}");
}

using (var query = connection.CreateCommand())
{
    query.CommandText = "SELECT Status, COUNT(*) FROM Equipments GROUP BY Status ORDER BY Status;";
    using var reader = query.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)}\t{reader.GetInt32(1)}");
    }
}
