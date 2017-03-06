using System;
using System.Reflection;
using Code.Network.Attributes;
using Code.Network.Commands;
using UnityEngine.Networking;

namespace Code.Network.Composition {
    public class RegisterHandler : IRegisterHandler {

        public void Init() {
            InitServer();
            InitClient();
        }

        public void InitServer() {
            var srType = typeof(ServerRegister);
            var methods = srType.GetMethods();

            foreach (var m in methods) {
                ParameterInfo[] myParameters = m.GetParameters();
                if (myParameters.Length <= 0) continue;
                for(int j = 0; j < myParameters.Length; j++) {
                    var myAttributes = m.GetCustomAttributes(typeof(ServerCommandAttribute), false);

                    if (myAttributes.Length <= 0) continue;
                    foreach (object att in myAttributes) {
                        ServerCommandAttribute sca = att as ServerCommandAttribute;
                        //Net.Server.Register.Subscribe(sca.NetCommand, (NetworkMessageDelegate)Delegate.CreateDelegate(typeof(NetworkMessageDelegate),m));

                        ServerRegister.Subscribe(sca.NetCommand, (NetworkMessageDelegate)Delegate.CreateDelegate(typeof(NetworkMessageDelegate),m));
                    }
                }
            }
        }

        public void InitClient() {
            var srType = typeof(ClientRegister);
            var methods = srType.GetMethods();

            foreach (var m in methods) {
                ParameterInfo[] myParameters = m.GetParameters();
                if (myParameters.Length <= 0) continue;
                for(int j = 0; j < myParameters.Length; j++) {
                    var myAttributes = m.GetCustomAttributes(typeof(ClientCommandAttribute), false);

                    if (myAttributes.Length <= 0) continue;
                    foreach (object t in myAttributes) {
                        ClientCommandAttribute sca = t as ClientCommandAttribute;
                        ClientRegister.Subscribe(sca.NetCommand, (NetworkMessageDelegate)Delegate.CreateDelegate(typeof(NetworkMessageDelegate),m));
                    }
                }
            }
        }

        public void AddToServer(short msgType, NetworkMessageDelegate handler) {
            ServerRegister.Subscribe(msgType, handler);
        }

        public void AddToClient(short msgType, NetworkMessageDelegate handler) {
            ClientRegister.Subscribe(msgType, handler);
        }

        public void Add(short msgType, NetworkMessageDelegate hServer, NetworkMessageDelegate hClient) {
            ServerRegister.Subscribe(msgType, hServer);
            ClientRegister.Subscribe(msgType, hClient);
        }
    }
}
