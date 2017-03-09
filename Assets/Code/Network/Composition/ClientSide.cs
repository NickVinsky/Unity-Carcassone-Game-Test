using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using static Code.Network.PlayerSync;

namespace Code.Network.Composition {
    public class ClientSide {

        public NetPackPlayerInfo MyInfo() {
            return new NetPackPlayerInfo {
                ID = PlayerInfo.ID,
                PlayerName = PlayerInfo.PlayerName,
                IsRegistred = PlayerInfo.IsRegistred,
                IsReady = PlayerInfo.IsReady,
                Color = PlayerInfo.Color,
                FollowersNumber = PlayerInfo.FollowersNumber,
                Score = PlayerInfo.Score
            };
        }

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

        public void Action(Command command) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command});
        }

        public void Action(Command command, int value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Value = value});
        }

        public void Action(Command command, string text, int value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Text = text, Value = value});
        }

        public void Action(Command command, Vector3 vect3) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vect3 = vect3});
        }

        public void ChatMessage(string text) {
            var mToSend = new NetPackChatMessage {
                Player = PlayerInfo.PlayerName,
                Message = text,
                RequesterID = PlayerInfo.ID
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
                RequesterID = PlayerInfo.ID
            };
            Send(NetCmd.ChatMessage, mToSend);
        }
    }
}