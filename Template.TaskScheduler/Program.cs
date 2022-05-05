using Template.TaskScheduler;
using Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

using IHost host = builder
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<XxContext>((services, options) =>
        {
            options
            .UseSqlServer(context.Configuration.GetValue<string>("Settings:ConnectionString"));
        });

        services.AddQuartz(o =>
        {
            o.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey(nameof(XxTask));

            o.AddJob<XxTask>(jobKey);

            o.AddTrigger(t => t
                .WithIdentity("TriggerWhenNewDay")
                .ForJob(jobKey)
                .StartNow()
                .WithCronSchedule("0 0 0 * * ?")
            );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

    })
    .UseSerilog((context, services, loggerConfiguration) =>
    {
        var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}){NewLine}{Message:lj}{NewLine}{Exception}";
        loggerConfiguration
                     .ReadFrom.Configuration(context.Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.File("./log/log.txt",
                             outputTemplate: outputTemplate,
                             rollingInterval: RollingInterval.Day)
                     .WriteTo.Console(outputTemplate: outputTemplate);
    })
    .Build();


#if DEBUG
var factory = host.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await factory.GetScheduler();
await scheduler.TriggerJob(new JobKey(nameof(XxTask)));
#endif

await host.RunAsync();

