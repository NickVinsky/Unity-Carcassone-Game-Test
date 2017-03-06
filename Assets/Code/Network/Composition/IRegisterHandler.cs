using UnityEngine.Networking;

namespace Code.Network.Composition {
    public interface IRegisterHandler {
        void Init();
        void InitServer();
        void InitClient();

        void Add(short msgType, NetworkMessageDelegate hServer, NetworkMessageDelegate hClient);
        void AddToServer(short msgType, NetworkMessageDelegate handler);
        void AddToClient(short msgType, NetworkMessageDelegate handler);
    }
}