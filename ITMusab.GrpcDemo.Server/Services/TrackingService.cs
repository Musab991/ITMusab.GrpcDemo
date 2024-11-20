using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITMusab.GrpcDemo.Server.Protos;

namespace ITMusab.GrpcDemo.Server.Services
{
    public class TrackingService : Protos.TrackingService.TrackingServiceBase
    {
        private readonly ILogger<TrackingService> logger;

        public TrackingService(ILogger<TrackingService> logger)
        {
            this.logger = logger;
        }

        public override Task<TrackingResponse> SendMessage(TrackingMessage request, ServerCallContext context)
        {
            
            logger.LogInformation($"New message received: DeviceId: {request.DeviceId}, Speed: {request.Speed} km/h, " +
                $"Location: {request.Location.Line1}, {request.Location.Line2}, {request.Location.City}, " +
                $"{request.Location.Country}, Postal Code: {request.Location.PostalCode}, Zip Code: " +
                $"{request.Location.ZipCode}\n Sensosrs:[{request.Sensors.Count}]");

            // Return a successful response
            return Task.FromResult(new TrackingResponse { Success = true });
        }

        public override async Task<Empty> KeepAlive(
                IAsyncStreamReader<PulseMessage> requestStream,
                ServerCallContext context)
        {

            var task = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    logger.LogInformation($"Received message: Status={message.State}, Details={message.Details}, Timestamp={message.Stamp}");

                    // Here, additional logic based on `message.State`
                    if (message.State == ClientStatus.Failed)
                    {
                        logger.LogWarning("Client has reported a FAILURE status!");
                    }

                }
            });
            await task;


            return new Empty();
        }
    }
}
