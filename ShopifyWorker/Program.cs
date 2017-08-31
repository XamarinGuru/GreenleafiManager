using System.Diagnostics;
using Microsoft.Azure.WebJobs;

namespace ShopifyWorker {
    internal class Program {
        private static void Main () {
            var config = new JobHostConfiguration();
            config.UseTimers();
            
            var host = new JobHost( config );

            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            host.RunAndBlock();
        }
    }
}