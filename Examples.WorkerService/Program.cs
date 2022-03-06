using Examples.WorkerService;

public static class Program
{
    public async static Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .Build();

        await host.RunAsync();

    }

}
