using ITMusab.GrpcDemo.Server.Services;

namespace ITMusab.GrpcDemo.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc(); // Add gRPC services
            builder.Services.AddGrpcReflection(); // Add gRPC reflection

            builder.Services.AddControllers();

            // Removed Swagger and OpenAPI setup
            // builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Enable gRPC reflection

            if(app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService(); // This enables the gRPC reflection service

            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Map controllers for HTTP-based APIs
            app.MapControllers();

            // Map gRPC service
            app.MapGrpcService<TrackingService>(); // Map your gRPC service here


            app.Run();
        }
    }
}
