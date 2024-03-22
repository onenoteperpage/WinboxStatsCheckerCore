using Microsoft.Data.Sqlite;
using WinboxStatsCheckerCore.Helpers;

namespace SystemInfoCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Provide help and/or advice via args
            if (args.Contains("--version") || args.Contains("-v"))
            {
                Console.WriteLine("Winbox Stats Checker Core v1.0.0");
                return;
            }
            else if (args.Length > 0)
            {
                Console.WriteLine("Winbox Stats Checker Core\n  Usage:  wbscc --version        Shows current version");
                return;
            }

            // Variables used globally
            string currentYearMonth = DateTime.Now.ToString("yyyyMM");
            string computerName = Environment.MachineName;
            string dbName = $"{currentYearMonth}-{computerName}.sqlite";

            // Create SQLiteDB connection
            using (var connection = new SqliteConnection($"Data Source={dbName}"))
            {
                connection.Open();

                // Time now
                DateTime currentTime = DateTime.Now;

                // Create tables for RAM, and CPU
                SqliteDBHelper.CreateTable(connection, "RAM");
                SqliteDBHelper.CreateTable(connection, "CPU");

                // RAM Usage
                var ramUsage = StatsHelper.GetRamUsage();
                SqliteDBHelper.InsertRecord(connection, "RAM", currentTime, ramUsage);
                Console.WriteLine($"RAM Usage: {ramUsage}");

                // CPU Usage
                var cpuUsage = await StatsHelper.GetSystemCpuUsageAsync();
                SqliteDBHelper.InsertRecord(connection, "CPU", currentTime, cpuUsage);
                Console.WriteLine($"CPU Usage: {cpuUsage}");

                // HDD Usage
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                    {
                        var totalSize = drive.TotalSize / (1024 * 1024 * 1024); // in GB
                        var freeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024); // in GB
                        var usedSpace = totalSize - freeSpace;
                        var driveUsagePercent = ((double)usedSpace / totalSize) * 100;
                        var _driveName = drive.Name.Replace(":\\", "_Drive");
                        SqliteDBHelper.CreateTable(connection, _driveName);
                        SqliteDBHelper.InsertRecord(connection, _driveName, currentTime, driveUsagePercent);
                        Console.WriteLine($"Drive {drive.Name} usage: {driveUsagePercent:F2}%");
                    }
                }

                connection.Close();
            }

            Thread.Sleep(1000);
        }
    }
}
