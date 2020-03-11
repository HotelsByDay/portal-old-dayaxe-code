using System;
using System.ServiceProcess;

namespace Dayaxe.Schedule
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Started in user interactive mode!");
                try
                {
                    AutoSendEmailServiceProduction service1 = new AutoSendEmailServiceProduction(args);
                    service1.TestStartupAndStop(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " - " + ex.InnerException);
                }
            }
            else
            {
                Console.WriteLine("Started as service!");
                var servicesToRun = new ServiceBase[]
                {
                    new AutoSendEmailServiceProduction(args), 
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
