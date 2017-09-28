using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace OPC2SQL.Types.Abstract
{
    /// <summary>Interface for OPC data reading and storing.</summary>
    public interface IO2SOPCReader
    {
        /// <summary>Logger.</summary>
        ILog Logger { get; }

        /// <summary>Config reader.</summary>
        IO2SConfigReader ConfigReader { get; }

        /// <summary>Tags list from OPC.</summary>
        IList<O2SOPCTag> Tags { get; set; }

        /// <summary>Tags list from OPC.</summary>
        IList<O2SOPCTag> ReadData();
    }

    /// <summary>Struct for tag OPC data.</summary>
    public struct O2SOPCTag
    {
        /// <summary>Tag name.</summary>
        public string Name { get; set; }

        /// <summary>Tag OPCItemID.</summary>
        public string OPCItemID { get; set; }

        /// <summary>Tag timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Tag read error.</summary>
        public int ReadError { get; set; }

        /// <summary>Tag quality.</summary>
        public int Quality { get; set; }

        /// <summary>Tag value.</summary>
        public float Value { get; set; }
    }
}
