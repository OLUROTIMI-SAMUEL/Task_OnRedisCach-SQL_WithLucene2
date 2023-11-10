using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Task_1.Db_Folder;
using Task_1.Models;
using Task_1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.Configure<RedisConfig>(Configuration.GetSection("Redis"));
builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("Redis"));

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var redisConfig = provider.GetRequiredService<IOptions<RedisConfig>>().Value;
    var redisOptions = ConfigurationOptions.Parse(redisConfig.ConnectionString);
  
    return ConnectionMultiplexer.Connect(redisOptions);
});


//builder.Services.AddDistributedRedisCache(options =>

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RedisSample_";
});


builder.Services.AddDbContext<CustomerInfo_DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));

builder.Services.AddScoped<IRedisCaching, RedisCaching>();

builder.Services.AddScoped<LuceneService>();
//builder.Services.AddSingleton(typeof(ILuceneService<>), typeof(LuceneService<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
