using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    // Delegate ?

    interface IExceptionMonitor
    {
        void OnUserException(Exception e);
        void OnSystemException(Exception e);
    }
    interface IConnectionMonitor
    {
        void OnConnectClient();
        void OnDisconnectClient();

        void OnConnectCluster();
        void OnDisconnectCluster();
    }
    interface IPacketMonitor
    {
        void OnSend(Packet e);
        void OnRecv(Packet e);
    }
}
