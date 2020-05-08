using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Niazza.Vault;

namespace OrderManager.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
          
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseVaultConfig(options => options.ServiceName = "order-manager")
                .Build();

            host.Run();
        }

    }
}
