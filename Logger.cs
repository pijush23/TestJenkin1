using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class Logger
{
    private readonly string _userName;
    private readonly string _baseLogDirectory;
    private readonly object _lockObj = new object();

    public Logger(string userName, string baseLogDirectory = "Logs")
    {
        _userName = userName;
        _baseLogDirectory = baseLogDirectory;

        // Ensure the log directory structure exists
        string userDirectory = GetUserDirectory();
        if (!Directory.Exists(userDirectory))
        {
            Directory.CreateDirectory(userDirectory);
        }
    }

    private string GetUserDirectory()
    {
        // Hash the username to create a subdirectory path
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_userName));
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
            return Path.Combine(_baseLogDirectory, hashString);
        }
    }

    private string GetLogFilePath()
    {
        string userDirectory = GetUserDirectory();
        return Path.Combine(userDirectory, $"{_userName}.log");
    }

    public async Task LogAsync(string message)
    {
        string filePath = GetLogFilePath();
        lock (_lockObj) // Ensures thread-safe writing
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }

    public Task CleanupOldLogsAsync(int daysToKeep = 6)
    {
        return Task.Run(() =>
        {
            string userDirectory = GetUserDirectory();

            // Get all log files in the user's directory
            var logFiles = Directory.GetFiles(userDirectory, "*.log");

            foreach (var logFile in logFiles)
            {
                var fileInfo = new FileInfo(logFile);

                // If the log file is older than the specified number of days, delete it
                if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-daysToKeep))
                {
                    File.Delete(logFile);
                }
            }
        });
    }
}


