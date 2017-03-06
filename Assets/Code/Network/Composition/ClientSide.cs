using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using static Code.Network.PlayerSync;

namespace Code.Network.Composition {
    public class ClientSide {

        public void Start() {
            NetworkManager.singleton.networkAddress = Net.NetworkAddress;
            NetworkManager.singleton.networkPort = Net.NetworkPort;
            NetworkManager.singleton.StartClient();
        }

        public void Log(string message) {
            MainMenuGUI.LogJoin(message);
        }

        public void Stop() {
            NetworkManager.singleton.client.Disconnect();
            NetworkManager.singleton.client.Shutdown();
            ClientScene.DestroyAllClientObjects();
        }

        public void Send(short msgType, MessageBase msg) {
            NetworkManager.singleton.client.Send(msgType, msg);
        }

        public void ChatMessage(string text) {
            var mToSend = new NetPackChatMessage {
                Player = PlayerInfo.PlayerName,
                Message = text,
                RequesterID = PlayerInfo.ConnectionId
            };
            Send(NetCmd.ChatMessage, mToSend);
        }

        public void ChatInfo(ConnInfo infoType) {
            ChatInfo(infoType, string.Empty);
        }
        public void ChatInfo(ConnInfo infoType, string extraMessage) {
            ChatInfo(infoType, extraMessage, string.Empty);
        }
        public void ChatInfo(ConnInfo infoType, string extraMessage, string fromPlayer) {
            string sInfo = string.Empty;
            switch (infoType) {
                case ConnInfo.PlayerRegistred:
                    sInfo = PlayerInfo.PlayerName + " joined the lobby";
                    break;
                case ConnInfo.PlayerDisconnected:
                    sInfo = fromPlayer + " disconnected (" + extraMessage + ")";
                    break;
            }
            var mToSend = new NetPackChatMessage {
                IsInfoMessage = true,
                Player = "Server",
                Message = sInfo,
                RequesterID = PlayerInfo.ConnectionId
            };
            Send(NetCmd.ChatMessage, mToSend);
        }

        public NetPackPlayerInfo MyInfo() {
            return new NetPackPlayerInfo {
                ID = PlayerInfo.ConnectionId,
                PlayerName = PlayerInfo.PlayerName,
                IsRegistred = PlayerInfo.IsRegistred,
                IsReady = PlayerInfo.IsReady,
                Color = PlayerInfo.Color
            };
        }
    }
}