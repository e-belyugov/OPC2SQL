using log4net;
using OPC2SQL.Types.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC2SQL.Types.Concrete
{
    /// <summary>Class for sending data to SQL Server.</summary>
    public class O2SSQLSender : IO2SSQLSender
    {
        /// <summary>Logger.</summary>
        public ILog Logger { get; set; }

        /// <summary>Config reader.</summary>
        public IO2SConfigReader ConfigReader { get; set; }

        /// <summary>Connection to SQL Server.</summary>
        private OleDbConnection con;

        /// <summary>Connection flag.</summary>
        private bool Connected = false;

        /// <summary>Connects to SQL server.</summary>
        public bool Connect()
        {
            bool _Connected = false;

            try
            {
                if (ConfigReader.SQLSendDataEnabled && (ConfigReader.StoredProc != "") && (ConfigReader.SQLConnectionString != ""))
                {
                    Logger.Info("Соединение с SQL сервером");

                    con = new OleDbConnection(ConfigReader.SQLConnectionString);
                    con.Open();

                    _Connected = true;
                }
            }
            catch (Exception er)
            {
                _Connected = false;
                Logger.Error(er);
                return _Connected;
            }

            return _Connected;
        }

        /// <summary>Constructor.</summary>
        public O2SSQLSender(IO2SConfigReader creader, ILog logger)
        {
            // Initializing fields

            ConfigReader = creader;
            Logger = logger;

            // Initializing connection

            Connected = Connect();
        }

        /// <summary>Sending data to SQL Server.</summary>
        public void SendData(IList<O2SOPCTag> tags)
        {
            try
            {
                if (con != null)
                {
                    // Checking SQL connection

                    if (!Connected) Connected = Connect();
                    if (!Connected) return;

                    // Sending data

                    OleDbCommand myCommand = con.CreateCommand();
                    myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.CommandText = "[" + ConfigReader.StoredProc + "]";

                    int count = 0;
                    foreach (O2SOPCTag tag in tags)
                    {
                        if ((tag.ReadError == 0) && (tag.Quality >= 192))
                        {
                            myCommand.Parameters.Clear();
                            myCommand.Parameters.Add("@ID", OleDbType.VarChar);
                            myCommand.Parameters["@ID"].Value = tag.Name;
                            myCommand.Parameters.Add("@DT", OleDbType.Date);
                            myCommand.Parameters["@DT"].Value = tag.Timestamp;
                            myCommand.Parameters.Add("@Value", OleDbType.Single);
                            myCommand.Parameters["@Value"].Value = tag.Value;
                            myCommand.Parameters.Add("@State", OleDbType.VarChar);
                            myCommand.Parameters["@State"].Value = 0;  // !!!!!!!!!!!!!!!!!!

                            /*
                            string s = "";
                            foreach (OleDbParameter param in myCommand.Parameters)
                                s = s + param.ParameterName + " = " + param.Value + ", ";
                            Logger.Info(s);
                            */ 

                            myCommand.ExecuteNonQuery();

                            count++;
                        }
                    }

                    if (count > 0) Logger.Info("*** Данные на SQL сервер отправлены");
                }
            }
            catch (Exception er)
            {
                Connected = false;
                Logger.Error(er);
            }
        }
    }
}
