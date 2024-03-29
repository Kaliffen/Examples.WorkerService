using Examples.RabbitMq;
using System.Text;

namespace Examples.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                RabbitPipeline.aQueue("MyQueue").SendMessage("Hello World!");

                await RabbitPipeline.aQueue("MyQueue").Listen( message => { 
                    
                    Console.WriteLine(" [x] Received {0}", message); 

                }, stoppingToken);
            }
        }
    }
}