﻿using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string queueName = "LdQueue";

            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = "host.docker.internal",
                    Port = 5672,
                    DispatchConsumersAsync = true,
                    UserName = "guest",
                    Password = "guest",
                    //RequestedConnectionTimeout = 3000, // milliseconds
                };


                using (var rabbitConnection = connectionFactory.CreateConnection())
                {
                    using (var channel = rabbitConnection.CreateModel())
                    {
                        // Declaring a queue is idempotent 
                        channel.QueueDeclare(
                            queue: queueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        while (true)
                        {
                            string body = $"LD: {DateTime.Now.Ticks} " + DateTime.UtcNow;
                            channel.BasicPublish(
                                exchange: string.Empty,
                                routingKey: queueName,
                                basicProperties: null,
                                body: Encoding.UTF8.GetBytes(body));


                            Console.WriteLine("------------------------- PRODUCER: "+ body);
                            await Task.Delay(4000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
