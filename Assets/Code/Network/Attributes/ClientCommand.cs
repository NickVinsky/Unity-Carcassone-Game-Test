using System;

namespace Code.Network.Attributes {
    public class ClientCommandAttribute : Attribute {

        public readonly short NetCommand;

        public ClientCommandAttribute(short netCommand)
        {
            NetCommand = netCommand;
        }
    }
}