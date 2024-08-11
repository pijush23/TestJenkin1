class Program
{
    static async Task Main(string[] args)
    {
        // Simulate logging for multiple users
        for (int i = 1; i <= 5; i++)
        {
            string userName = $"User{i}";
            Logger logger = new Logger(userName);

            // Perform logging activities
            await LogUserActivityAsync(logger, userName);

            // Start cleanup asynchronously, without waiting for it to complete
            _ = logger.CleanupOldLogsAsync();
        }
    }

    static async Task LogUserActivityAsync(Logger logger, string userName)
    {
        for (int i = 0; i < 10; i++)
        {
            await logger.LogAsync($"{userName} activity {i}");
            await Task.Delay(10); // Simulate some delay between log entries
        }
    }
}