using System;
using Ninject;
using System.Reflection;
using OPC2SQL.Types.Abstract;
using OPC2SQL.Client.Infrastructure;
using log4net;
using System.Collections.Generic;
using System.Threading;
using OPC2SQL.Extensions;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace OPC2SQL.Client
{

    class Program
    {
        static IO2SConfigReader creader;
        static IO2SOPCReader oreader;
        static IO2SSQLSender sender;
        static ILog logger;

        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            logger = kernel.Get<ILog>();
            logger.Info("-----------------------------------");
            logger.Info("Запуск сервиса");

            oreader = kernel.Get<IO2SOPCReader>();
            creader = kernel.Get<IO2SConfigReader>();
            sender = kernel.Get<IO2SSQLSender>();

            IList<O2SOPCTag> tags = oreader.ReadData();
            if (creader.SQLSendDataEnabled) sender.SendData(tags);

            logger.DeleteOldLogs(creader);

            Timer tm = new Timer(Worker, null, 0, creader.ReadInterval);
            //Timer tmlogs = new Timer(LogWorker, null, 0, creader.ReadInterval);

            Console.ReadKey();

            logger.Info("-----------------------------------");
            logger.Info("Остановка сервиса");
        }

        static void Worker(object obj)
        {
            IList<O2SOPCTag> tags = oreader.ReadData();
            if (creader.SQLSendDataEnabled && (tags != null)) sender.SendData(tags);
        }

        static void LogWorker(object obj)
        {
            logger.DeleteOldLogs(creader);
        }
    }
}
