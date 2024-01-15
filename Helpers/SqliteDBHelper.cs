using Microsoft.Data.Sqlite;

namespace WinboxStatsCheckerCore.Helpers
{
    internal class SqliteDBHelper
    {
        public static void CreateTable(SqliteConnection connection, string tableName)
        {
            using (var command = new SqliteCommand($"CREATE TABLE IF NOT EXISTS [{tableName}] (Timestamp DATETIME, Value REAL)", connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void InsertRecord(SqliteConnection connection, string tableName, DateTime timestamp, double value)
        {
            using (var insertCommand = new SqliteCommand($"INSERT INTO [{tableName}] (Timestamp, Value) VALUES (@timestamp, @value)", connection))
            {
                insertCommand.Parameters.AddWithValue("@timestamp", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                insertCommand.Parameters.AddWithValue("@value", value);
                insertCommand.ExecuteNonQuery();
            }
        }
    }
}
