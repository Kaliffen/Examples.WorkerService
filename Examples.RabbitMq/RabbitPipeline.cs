using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace Examples.RabbitMq;

public class RabbitPipeline
{
    private string queueName;

    public RabbitPipeline(string queueName)
    {
        this.queueName = queueName;
    }

    public static RabbitPipeline aQueue(string queueName)
    {
        return new RabbitPipeline(queueName);
    }

    public async Task Listen(Action<string> handler, CancellationToken stoppingToken)
    {
        await WithModel(async channel =>
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handler(message);
            };
            
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            await stoppingToken;
        });
    }

    public void Send(Action<IModel> handler)
    {
        WithModel(channel => {

            handler(channel);
            
        });
    }

    public void SendMessage(string message)
    {
        WithModel(channel => {

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
        });
    }

    private async Task<RabbitPipeline> WithModel(Func<IModel, Task> handler)
    {
        var factory = new ConnectionFactory() { HostName = "172.17.0.3" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            await handler(channel);
        }
        return this;
    }

    private RabbitPipeline WithModel(Action<IModel> handler)
    {
        var factory = new ConnectionFactory() { HostName = "172.17.0.3", };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            handler(channel);
        }
        return this;
    }

   
}

