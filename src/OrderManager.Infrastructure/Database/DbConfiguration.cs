namespace OrderManager.Infrastructure.Database
{
    public class DbConfiguration
    {
        public int RetryCount { get; set; }
        public int CooldownIntervalMs { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public DbCredentials Credentials { get; set; }
        public string ApplicationName { get; set; }
    }

    public class DbCredentials
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
