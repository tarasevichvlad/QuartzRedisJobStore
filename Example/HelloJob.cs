using Quartz;

public class HelloJob : IJob
{
    /// <summary>
    /// Called by the <see cref="IScheduler" /> when a
    /// <see cref="ITrigger" /> fires that is associated with
    /// the <see cref="IJob" />.
    /// </summary>
    public ValueTask Execute(IJobExecutionContext context)
    {
        // Say Hello to the World and display the date/time
        var timestamp = DateTime.Now;
        Console.WriteLine($"Hello World! - from {context.JobDetail.Key} job - {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
        return ValueTask.CompletedTask;
    }
}