using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using OPC2SQL.Types.Abstract;
using Ninject;
using log4net;
using System.Reflection;
using OPC2SQL.Extensions;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace OPC2SQL.Service
{
    public partial class Service1 : ServiceBase
    {
        IO2SConfigReader creader;
        IO2SOPCReader oreader;
        IO2SSQLSender sqlsender;
        ILog logger;
        System.Timers.Timer timer;
        System.Timers.Timer timerLogs;

        public Service1()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            logger = kernel.Get<ILog>();

            logger.Info("-----------------------------------");
            logger.Info("Запуск сервиса");

            creader = kernel.Get<IO2SConfigReader>();
            oreader = kernel.Get<IO2SOPCReader>();
            sqlsender = kernel.Get<IO2SSQLSender>();

            timer = new System.Timers.Timer();
            timer.Interval = creader.ReadInterval;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            timerLogs = new System.Timers.Timer();
            timerLogs.Interval = 3600000 * 12; // Раз в 12 часов
            timerLogs.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerLogs);
            timerLogs.Start();
        }

        protected override void OnStop()
        {
            logger.Info("-----------------------------------");
            logger.Info("Остановка сервиса");
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                timer.Enabled = false;

                IList<O2SOPCTag> tags = oreader.ReadData();
                if (creader.SQLSendDataEnabled && (tags != null)) sqlsender.SendData(tags);

                timer.Enabled = true;
            }
            //catch (Exception er)
            catch (Exception)
            {
                //logger.Info("Ошибка в основном таймере");
                //logger.Error(er);
                timer.Enabled = true;
            }
        }

        public void OnTimerLogs(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                timerLogs.Enabled = false;

                logger.DeleteOldLogs(creader);

                timerLogs.Enabled = true;
            }
            //catch (Exception er)
            catch (Exception)
            {
                //logger.Info("Ошибка в таймере для очистки логов");
                //logger.Error(er);
                timerLogs.Enabled = true;
            }
        }
    }
}
