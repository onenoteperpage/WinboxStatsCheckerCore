using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace WinboxStatsCheckerCore.Helpers;

internal class StatsHelper
{
    // return RAM usage as % of total
    public static double GetRamUsage(bool isDebug)
    {
        long totalPhysicalMemory = GetTotalPhysicalMemory();
        long availableMemory = GetAvailableMemory();

        long usedMemory = totalPhysicalMemory - availableMemory;
        double usedMemoryPercentage = (double)usedMemory / totalPhysicalMemory * 100;

        if (isDebug)
        {
            Console.WriteLine($"Total Physical Memory: {totalPhysicalMemory / (1024 * 1024 * 1024)} GB");
            Console.WriteLine($"Used Memory: {usedMemory / (1024 * 1024 * 1024)} GB");
            Console.WriteLine($"Approximate RAM usage: {usedMemoryPercentage:F2}%");
        }

        return usedMemoryPercentage;
    }

    public static async Task<double> GetSystemCpuUsageAsync()
    {
        if (OperatingSystem.IsWindows())
        {
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue(); // Call once to initialize counter
                await Task.Delay(1000); // Wait a second to get a valid reading
                return cpuCounter.NextValue();
            }
        }
        else
        {
            // Implement logic for Linux/macOS or throw not supported exception
            throw new PlatformNotSupportedException("CPU Usage calculation is not supported on this platform.");
        }
    }

    // return CPU usage as % of total
    public static async Task<double> GetCpuUsageForProcess()
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
        await Task.Delay(1000);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;
        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        return cpuUsageTotal * 100;
    }

    private static long GetTotalPhysicalMemory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using (var query = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (var item in query.Get())
                {
                    return Convert.ToInt64(item.Properties["TotalPhysicalMemory"].Value);
                }
            }
            return 0;
#pragma warning restore CA1416 // Validate platform compatibility
        }
        else
        {
            return 0;
        }
    }
    private static long GetAvailableMemory()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        using (var query = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem"))
        {
            foreach (var item in query.Get())
            {
                return Convert.ToInt64(item.Properties["FreePhysicalMemory"].Value) * 1024; // Convert from KB to Bytes
            }
        }
#pragma warning restore CA1416 // Validate platform compatibility
        return 0;
    }
}
