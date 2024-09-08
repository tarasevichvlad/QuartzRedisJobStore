using Quartz;
using Quartz.JobStore.Redis;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
/*builder.Services.AddSingleton<IJobStore>(new RedisJobStore
{
    Database = 1,
    Host = "localhost",
    Ssl = false,
    Port = 6380,
    Password = "my-password"
});*/

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380,password=my-password"));

builder.Services.AddQuartz(x =>
{
    x.SchedulerId = "AUTO";

    x.UseDefaultThreadPool(v => v.MaxConcurrency = 20);
    x.UsePersistentStore<RedisJobStore>(c =>
    {
        c.UseSystemTextJsonSerializer();
        c.UseClustering();
    });
    
    //x.UseInMemoryStore();

    for (var i = 0; i < 1; i++)
    {
        var jobKey = new JobKey($"HelloJob-{i}", $"group-{i}");

        x.AddJob<HelloJob>(jobKey, c => c
                .DisallowConcurrentExecution()
                .WithDescription("my awesome trigger configured for a job with single call"));

        x.AddTrigger(configurator => configurator
            .ForJob(jobKey)
            .WithIdentity($"{jobKey.Name}-trigger", jobKey.Group)
            .WithSimpleSchedule(simpleScheduleBuilder => simpleScheduleBuilder.WithIntervalInSeconds(1).RepeatForever())
            .WithDescription("my awesome simple trigger"));
    }
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});



var app = builder.Build();

app.Run();