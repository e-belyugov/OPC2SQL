using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC2SQL.Types.Abstract
{
    /// <summary>Interface for sending data to SQL Server.</summary>
    public interface IO2SSQLSender
    {
        /// <summary>Logger.</summary>
        ILog Logger { get; }

        /// <summary>Config reader.</summary>
        IO2SConfigReader ConfigReader { get; }

        /// <summary>Sending data to SQL Server.</summary>
        void SendData(IList<O2SOPCTag> tags);
    }
}
