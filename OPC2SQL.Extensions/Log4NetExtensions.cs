using log4net;
using OPC2SQL.Types.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC2SQL.Extensions
{
    public static class Log4NetExtensions
    {
        /// <summary>Deleting old log files.</summary>
        public static void DeleteOldLogs(this ILog logger, IO2SConfigReader creader)
        {
            try
            {
                logger.Info("*** Проверка старых логов");

                DateTime dt_file;
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(Path.GetDirectoryName(creader.FileName) + "\\Logs");
                System.IO.DirectoryInfo[] dirs = info.GetDirectories();
                System.IO.FileInfo[] files = info.GetFiles();
                foreach (FileInfo file in files)
                {
                    dt_file = file.LastWriteTime;
                    if (dt_file < DateTime.Now.Date.AddDays(-creader.DaysToLog))
                    {
                        File.Delete(file.FullName);
                    }
                }
            }
            catch (Exception er)
            {
                logger.Error(er);
            }
        }
    }
}
