using NetFrame;
using NetFrame.auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic{
    interface HandlerInterface {
        void ClientConnect(UserToken token);
        void MessageReceive(UserToken token, SocketModel message);
        void ClientClose(UserToken token, string error);
    }
}
