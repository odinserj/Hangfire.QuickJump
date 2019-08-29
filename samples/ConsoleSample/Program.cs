using System;
using Hangfire;
using Hangfire.QuickJump;
using Microsoft.Owin.Hosting;

namespace ConsoleSample
{
    class Program
    {
        static void Main()
        {
            GlobalConfiguration.Configuration
                .UseQuickJump()
                .UseColouredConsoleLogProvider()
                .UseSqlServerStorage("Database=Hangfire.QuickJump;Integrated Security=SSPI;");

            BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

            using (WebApp.Start<Startup>("http://localhost:12002"))
            {
                Console.ReadLine();
            }
        }
    }
}