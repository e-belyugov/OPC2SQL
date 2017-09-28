using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPC2SQL.Types.Abstract;
using log4net;
using OPCAutomation;

namespace OPC2SQL.Types.Concrete
{
    /// <summary>Class for OPC data reading and storing.</summary>
    public class O2SOPCReader : IO2SOPCReader
    {
        /// <summary>Logger.</summary>
        public ILog Logger { get; set;  }

        /// <summary>Config reader.</summary>
        public IO2SConfigReader ConfigReader { get; set; }

        /// <summary>Tags list from OPC.</summary>
        public IList<O2SOPCTag> Tags { get; set; }

        // Fields for OPC
        public OPCServer _OPCServer;
        public OPCGroups _OPCGroups;
        public OPCGroup _OPCGroup;
        public bool Connected = false;

        /// <summary>OPC initialization.</summary>
        public bool InitOPC()
        {
            bool _Connected = false;

            try
            {
                Logger.Info("Инициализация OPC");

                _OPCServer = new OPCServer();
                _OPCServer.Connect(ConfigReader.OPCServerName);
                _OPCGroups = _OPCServer.OPCGroups;
                _OPCGroup = _OPCGroups.Add("OPCGroup1");

                Logger.Info("Добавление тэгов");

                int chandle = 0;
                Tags.Clear(); 
                foreach (O2SConfigTag tag in ConfigReader.Tags)
                {
                    O2SOPCTag otag = new O2SOPCTag();
                    otag.Name = tag.Name;
                    otag.OPCItemID = tag.OPCItemID;
                    Tags.Add(otag);
                    _OPCGroup.OPCItems.AddItem(otag.OPCItemID, ++chandle);
                }

                //_OPCGroup.UpdateRate = 1000;
                _OPCGroup.UpdateRate = ConfigReader.ReadInterval;
                _OPCGroup.IsActive = true;
                _OPCGroup.IsSubscribed = false;

                _Connected = true;
            }
            catch (Exception er)
            {
                _Connected = false;
                Logger.Error(er);
            }

            return _Connected;
        }

        /// <summary>Constructor.</summary>
        public O2SOPCReader(IO2SConfigReader creader, ILog logger)
        {
            try
            {
                // Initializing fields

                ConfigReader = creader;
                Logger = logger;
                Tags = new List<O2SOPCTag>();

                // Initializing OPC

                Connected = InitOPC();
            }
            catch (Exception er)
            {
                Logger.Error(er);
            }
        }

        /// <summary>Finalizer.</summary>
        ~O2SOPCReader()
        {
            //_OPCGroup.IsActive = false;
            //_OPCServer?.Disconnect();
        }

        /// <summary>Tags list from OPC.</summary>
        public IList<O2SOPCTag> ReadData()
        {
            try
            {
                // Checking OPC connection

                if (!Connected) Connected = InitOPC();
                if (!Connected) return null;

                // Reading tags

                int count = _OPCGroup.OPCItems.Count;
                int[] arH = new int[1 + count];
                Array arValues = new object[1 + count];
                Array arHandles;
                Array arErrors;
                object Qualities;
                object Timestamps;

                int i = 0;
                foreach (OPCItem item in _OPCGroup.OPCItems) arH[++i] = item.ServerHandle;
                arHandles = (Array)arH;

                _OPCGroup.SyncRead(
                    (short)OPCDataSource.OPCDevice,
                    count,
                    ref arHandles,
                    out arValues,
                    out arErrors,
                    out Qualities,
                    out Timestamps);

                Logger.Info("-----------------------------------");
                Logger.Info("Чтение OPC тэгов");

                float _value;
                int _error;
                int _quality;
                int h;
                //string ss;
                DateTime _timestamp;
                O2SOPCTag _tag;
                for (i = 1; i <= count; i++)
                {
                    _value = Convert.ToSingle(arValues.GetValue(i));
                    //ss = arValues.GetValue(i).ToString();
                    _error = Convert.ToInt32(arErrors.GetValue(i));
                    _quality = Convert.ToInt32((Qualities as Array).GetValue(i));
                    _timestamp = Convert.ToDateTime((Timestamps as Array).GetValue(i));
                    h = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
                    _timestamp = _timestamp.AddHours(h);

                    _tag = Tags[i - 1];
                    _tag.Value = _value;
                    _tag.ReadError = _error;
                    _tag.Quality = _quality;
                    _tag.Timestamp = _timestamp;
                    Tags[i - 1] = _tag;

                    string s = "------ " + _tag.Name;
                    Logger.Info(s);
                    Logger.Info("timestamp = " + _timestamp);
                    Logger.Info("value     = " + _value);
                    //Logger.Info("string    = " + ss);
                    Logger.Info("error     = " + _error);
                    Logger.Info("quality   = " + _quality);
                }
            }
            catch (Exception er)
            {
                Connected = false;
                Logger.Error(er);
            }

            //return Tags.Select(item => (O2SOPCTag)item.Clone());
            return new List<O2SOPCTag>(Tags);
        }
    }
}
