using System;

namespace Code.Network.Attributes {
    public class ServerCommandAttribute : Attribute {

        public readonly short NetCommand;

        public ServerCommandAttribute(short netCommand)
        {
            NetCommand = netCommand;
        }
    }
}