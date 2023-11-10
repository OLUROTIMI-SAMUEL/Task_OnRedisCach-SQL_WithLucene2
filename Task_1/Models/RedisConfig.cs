namespace Task_1.Models
{
    public class RedisConfig
    {
        // Properties representing Redis connection settings
        public string ConnectionString { get; set; }

        //   public string Password { get; set; }

        public RedisConfig()
        {
            ConnectionString = string.Empty; // or set to a default value
        }
    }
}
