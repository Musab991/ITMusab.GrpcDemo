using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using ITMusab.GrpcDemo.Server.Protos;
using System;


namespace ITMusab.GrpcDemo.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly int deviceId;
        private  TrackingService.TrackingServiceClient _client;

        private TrackingService.TrackingServiceClient client
        {
            get
            {

                if (_client == null)
                {
                    var channel = GrpcChannel.ForAddress(" https://localhost:7053");
                     _client = new TrackingService.TrackingServiceClient(channel);
                }

                return _client; 
                
            }
        }
        public Worker(ILogger<Worker> logger,int deviceId)
        {
            _logger = logger;
            this.deviceId = deviceId;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            Random random = new Random();
            using var stream= client.KeepAlive();
            
            
            //task 1 
            var keepAliveTask=  Task.Run(async () =>
               {
                     while (!stoppingToken.IsCancellationRequested)
                     {

                        PulseMessage message = new PulseMessage
                        {
                            State = ClientStatus.Working,
                            Details = "Client is operational",
                            Stamp = Timestamp.FromDateTime(DateTime.UtcNow)
                        };
                        await stream.RequestStream.WriteAsync(message);
                        await Task.Delay(2000, stoppingToken);
                     }
               }, stoppingToken);

            //task 2 
            var sendMessageTask = Task.Run(async () =>
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    await SendMessage(random);
                    await Task.Delay(4000, stoppingToken);

                }
            }, stoppingToken);


            //in parrlaler both..
            await Task.WhenAll(keepAliveTask, sendMessageTask);


        }

        private async Task SendMessage(Random random)
        {

            TrackingMessage request = new TrackingMessage
            {

                DeviceId = random.Next(0, 220),
                Speed = random.Next(100, 500),
                Location = new Location
                {
                    City = "Amman",
                    Country = "Jordan",
                    Line1 = "newyork",
                    Line2 = "Virigina",
                    PostalCode = 11521,
                    ZipCode = 12314
                },
                Stamp = Timestamp.FromDateTime(DateTime.UtcNow),


            };

            request.Sensors.Add(new Sensor { Key = "temp", Value = 12 });
            request.Sensors.Add(new Sensor { Key = "door", Value = 20 });

            var response = await client.SendMessageAsync(request);
            _logger.LogInformation($"response:{response.Success}");

        }

    }
}
