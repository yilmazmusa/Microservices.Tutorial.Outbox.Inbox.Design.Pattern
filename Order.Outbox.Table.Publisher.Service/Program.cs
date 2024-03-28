using MassTransit;
using Order.Outbox.Table.Publisher.Service.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddQuartz(configurator =>
{
    JobKey jobKey = new JobKey("OrderOutboxPublishJob");

    configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobKey)); // Job'ı ekledik.

    TriggerKey triggerKey = new("OrderOutboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobKey) // Job'ı(OrderOutboxPublishJob) tetikleyecek Trigger'ı(OrderOutboxPublishTrigger) ekledik.
    .WithIdentity(triggerKey)
    .StartAt(DateTime.UtcNow) //Trigger'ın(Job'ın) ne zaman tetikleneceğini söyledik.
    .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5) // Trigger'ın(Job'ın) tetiklenme periyodunu belirttik 5 saniyede 1
    .RepeatForever())); // Sonsuza kadar bu Job'ın çalışması gerektiğini söylüyorum.

});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true); //Yapılan bu Job çalışmasını host ediyoruz

var host = builder.Build();
host.Run();
