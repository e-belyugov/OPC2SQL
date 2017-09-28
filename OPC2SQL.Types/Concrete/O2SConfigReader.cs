using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPC2SQL.Types.Abstract;
using System.Xml;
using log4net;

namespace OPC2SQL.Types.Concrete
{
    /// <summary>Class for config data reading and storing.</summary>
    public class O2SConfigReader : IO2SConfigReader
    {
        /// <summary>Config file name with full path.</summary>
        public string FileName { get; set; }

        /// <summary>Flag for enabling SQL data sending.</summary>
        public bool SQLSendDataEnabled { get; set; }

        /// <summary>ConnectionString for SQL Server.</summary>
        public string SQLConnectionString { get; set; }

        /// <summary>Name of stored procedure to send date.</summary>
        public string StoredProc { get; set; }

        /// <summary>OPCServerName.</summary>
        public string OPCServerName { get; set; }

        /// <summary>Read data interval.</summary>
        public int ReadInterval { get; set; }

        /// <summary>Days of log saving.</summary>
        public int DaysToLog { get; set; }

        /// <summary>Tags list from config.</summary>
        public IList<O2SConfigTag> Tags { get; set; }

        /// <summary>Logger.</summary>
        public ILog Logger { get; set; }

        /// <summary>Constructor.</summary>
        public O2SConfigReader(string filename, ILog logger)
        {
            FileName = filename;
            Tags = new List<O2SConfigTag>();
            Logger = logger;
            ReadConfig();
        }

        /// <summary>Reading configuration.</summary>
        //public void ReadConfig()
        public void ReadConfig()
        {
            try
            {
                // Чтение конфигурации - Reading configuration

                Logger.Info("Чтение конфигурации");

                XmlTextReader reader = new XmlTextReader(FileName);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // Узел является элементом - Node is an element

                            if (reader.Name == "SendData")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "enabled") SQLSendDataEnabled = Convert.ToBoolean(reader.Value);
                                }
                            }

                            if (reader.Name == "ConnectionString")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "value") SQLConnectionString = reader.Value;
                                }
                            }

                            if (reader.Name == "StoredProc")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name") StoredProc = reader.Value;
                                }
                            }

                            if (reader.Name == "OPCServer")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name") OPCServerName = reader.Value;
                                    if (reader.Name == "readinterval") ReadInterval = Convert.ToInt32(reader.Value);
                                }
                            }

                            if (reader.Name == "tag")
                            {
                                O2SConfigTag tag = new O2SConfigTag();
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name") tag.Name = reader.Value;
                                    if (reader.Name == "OPCItemID") tag.OPCItemID = reader.Value;
                                }
                                Tags.Add(tag);
                            }

                            if (reader.Name == "DaysToLog")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "value") DaysToLog = Convert.ToInt32(reader.Value);
                                }
                            }

                            break;
                    }
                }

                reader.Close();

                // Запись конфигурации в лог - Logging Configuration

                //Logger.Info("ConnectionString = " + SQLConnectionString);
                //Logger.Info("StoredProc       = " + StoredProc);
                Logger.Info("SendData      = " + SQLSendDataEnabled);
                Logger.Info("OPCServerName = " + OPCServerName);
                Logger.Info("ReadInterval  = " + ReadInterval);
                Logger.Info("Тэги:");
                foreach (O2SConfigTag tag in Tags)
                {
                    string s = tag.Name + " --- " + tag.OPCItemID;
                    Logger.Info(s);
                }

            }
            catch (Exception er)
            {
                Logger.Error(er);
            }
        }
    }
}
