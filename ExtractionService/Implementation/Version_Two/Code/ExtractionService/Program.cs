namespace ExtractionService
{
    using System;
    using System.Threading;
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ExtractionService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
