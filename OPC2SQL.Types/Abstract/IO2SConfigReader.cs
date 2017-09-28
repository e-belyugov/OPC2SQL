using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace OPC2SQL.Types.Abstract
{
    /// <summary>Interface for config data reading and storing.</summary>
    public interface IO2SConfigReader
    {
        /// <summary>Config file name with full path.</summary>
        string FileName { get; }

        /// <summary>Flag for enabling SQL data sending.</summary>
        bool SQLSendDataEnabled { get; }

        /// <summary>ConnectionString for SQL Server.</summary>
        string SQLConnectionString { get; }

        /// <summary>Name of stored procedure to send date.</summary>
        string StoredProc { get; }

        /// <summary>OPCServerName.</summary>
        string OPCServerName { get; }

        /// <summary>Read data interval.</summary>
        int ReadInterval { get; }

        /// <summary>Days of log saving.</summary>
        int DaysToLog { get; }

        /// <summary>Tags list from config.</summary>
        IList<O2SConfigTag> Tags { get; set; }

        /// <summary>Logger.</summary>
        ILog Logger { get; set; }

        /// <summary>Reading configuration.</summary>
        void ReadConfig();
    }

    /// <summary>Struct for tag config data.</summary>
    public struct O2SConfigTag
    {
        /// <summary>Tag name.</summary>
        public string Name { get; set; }

        /// <summary>Tag OPCItemID.</summary>
        public string OPCItemID { get; set; }
    }
}
