namespace ITMusab.GrpcDemo.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Device-Id:");
            int deviceId=int.Parse(Console.ReadLine());
            
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>(provider =>
            {
                var logger=provider.GetService<ILogger<Worker>>();
            
                    return new Worker(logger,deviceId);
            });


            var host = builder.Build();
            host.Run();
        }
    }
}