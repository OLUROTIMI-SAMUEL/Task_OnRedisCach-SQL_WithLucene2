
using StackExchange.Redis;
using System.Text.Json;
using Task_1.Db_Folder;

namespace Task_1.Services
{
    public class RedisCaching : IRedisCaching
    {
       private IDatabase _cacheDb;

        //public RedisCaching()
        //{
        //   var redis = ConnectionMultiplexer.Connect("localhost:6379");   //For development enviroment

        //  //  var redis = ConnectionMultiplexer.Connect("demo.lr4j4h.clustercfg.euw3.cache.amazonaws.com");

        //    _cacheDb = redis.GetDatabase();
        //}

        //public RedisCaching(IConfiguration configuration)
        //{
        //    var redisConfiguration = configuration.GetSection("Redis");

        //    var redisOptions = new ConfigurationOptions
        //    {
        //        EndPoints = { $"{redisConfiguration["localhost"]}:{redisConfiguration.GetValue<int>("6379")}" },
        //        Password = redisConfiguration["Password"],
        //        AbortOnConnectFail = false
        //    };

        //    var redis = ConnectionMultiplexer.Connect(redisOptions);
        //    _cacheDb = redis.GetDatabase();
        //}

        private readonly IConnectionMultiplexer _redisConnection;
        private readonly CustomerInfo_DbContext dbContext;

        public RedisCaching(IConnectionMultiplexer redisConnection, CustomerInfo_DbContext dbContext)
        {
            _redisConnection = redisConnection;
            this.dbContext = dbContext;
            _cacheDb = _redisConnection.GetDatabase();
        }
        public T GetData<T>(string key)
        {
           var value = _cacheDb.StringGet(key);
             if(!string.IsNullOrEmpty(value))
          //  if (!value.IsNull)
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }

        public object RemoveData(string key)
        {
           var exist = _cacheDb.KeyExists(key);

            if (exist) 
                return _cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T vaue, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(vaue), expiryTime);
        }
    }
}
