using System;
using System.Linq;
using Code.Network.Attributes;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Commands {
    public static class ServerRegister {
        public static NetPackBlank NullMsg = new NetPackBlank();
        //Debug.Log("Color = " + m.Color + " ;ID = " + m.ID + " ;PlayerName = " + m.PlayerName + " ; IsRegistred = " + m.IsRegistred);
        public static void Subscribe(short msgType, NetworkMessageDelegate handler) {
            NetworkServer.RegisterHandler(msgType, handler);
        }

        [ServerCommand(NetCmd.ChatMessage)]
        public static void ChatSendMessage(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();

            if (m.Message.StartsWith("/")) {
                switch (m.Message) {
                    case "/myinfo":
                        string pInfo;
                        var p = Net.Player.Single(s => s.ID == m.RequesterID);
                        pInfo = "[AboutPlayer] Name#ID: " + p.PlayerName + "#" + p.ID;
                        pInfo += ", Color: " + p.Color;
                        pInfo += ", Registered: " + p.IsRegistred;
                        Net.Server.CommandResponse(m, pInfo);
                        break;
                    default:
                        Net.Server.CommandResponse(m, "Unknown command.");
                        break;
                }
                return;
            }
            // Add message to chat history
            Net.Server.AddToChatHistory(m);

            Net.Server.SendToAll(NetCmd.ChatMessage, m);
        }

        [ServerCommand(NetCmd.ChatHistory)]
        public static void ChatSendHistory(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();
            Net.Server.SendTo(m.RequesterID, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});
        }

        [ServerCommand(NetCmd.UpdatePlayerInfo)]
        public static void UpdatePlayerInfo(NetworkMessage message) {
            var sender = message.ReadMessage<NetPackPlayerInfo>();

            var curPlayer = Net.Player.First(p => p.ID == sender.ID);
            var index = Net.Player.IndexOf(curPlayer);
            var lastID = Net.Player[index].ID;
            var lastColor = Net.Player[index].Color;

            curPlayer.ID = sender.ID;
            curPlayer.Color = sender.Color;
            curPlayer.PlayerName = sender.PlayerName;
            curPlayer.IsRegistred = sender.IsRegistred;
            curPlayer.IsReady = sender.IsReady;

            //if (lastID != curPlayer.ConnectionId || lastColor != curPlayer.Color) Net.Server.SendToAll(NetCmd.RefreshPlayersInfoAndReformPlayersList, NullMsg);
            if(index != -1) Net.Player[index] = curPlayer;
            if (!Net.Game.IsStarted() && AllIsReady()) Net.StartCountdown();
            //else Net.StopCountdown();
        }

        [ServerCommand(NetCmd.UpdatePlayerInfoAndReformPList)]
        public static void UpdatePlayerInfoAndReformPList(NetworkMessage message) {
            UpdatePlayerInfo(message);
            ReformPlayersList(message);
        }

        [ServerCommand(NetCmd.FormPlayersList)] // This Command Receives Only Player Name And ConnectionID
        public static void FormPlayersList(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            if (PlayerExist(m.PlayerName)) {
                Net.Server.SendTo(m.ID, NetCmd.Err, new NetPackErr{Err = ErrType.PlayerNameOccupied});
                return;
            }
            if (!m.IsRegistred) {
                Net.Server.SendTo(m.ID, NetCmd.Inf, new NetPackInf{Inf = ConnInfo.PlayerRegistred});
                Net.StopCountdown();
            }

            Net.Player.Add(new PlayerInformation{ID = m.ID, PlayerName = m.PlayerName});
            FormAndSendPlayersList();
        }

        [ServerCommand(NetCmd.ReformPlayersList)] // This Command Used For Form Players List With Full knowlege about player (color etc.)
        public static void ReformPlayersList(NetworkMessage message) {
            FormAndSendPlayersList();
        }

        [ServerCommand(NetCmd.ReformPlayersListWithAdding)] // This Command Receives Full Player Info And Reform List (when someone leave for example)
        public static void ReformPlayersListWithAdding(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            Net.Player.Add(new PlayerInformation {
                ID = m.ID,
                PlayerName = m.PlayerName,
                Color = m.Color,
                IsRegistred = m.IsRegistred
            });
            if (Net.Game.IsStarted()) {
                Net.Server.RefreshInGamePlayersList();
            } else {
                FormAndSendPlayersList();
            }
        }

        [ServerCommand(NetCmd.FreeColorRequest)]
        public static void GetFreeColor(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            var origColor = m.Color;
            var origColorInt = (int) origColor;

            if (origColor == (PlayerColor) Net.MaxPlayers) m.Color = 0;
            for (var i = 1; i <= Net.MaxPlayers; i++) {
                var curColorInt = i + origColorInt;
                if (curColorInt > Net.MaxPlayers) curColorInt -= 5;
                var curColor = (PlayerColor) curColorInt;
                if (Net.Player.Any(p => p.Color == curColor)) continue;
                Net.Server.SendTo(m.ID, NetCmd.FreeColorRequest, new NetPackPlayerInfo{ Color = curColor});
                return;
            }
            Net.Server.SendTo(m.ID, NetCmd.FreeColorRequest, new NetPackPlayerInfo{ Color = origColor});
        }

        #region InGameCommands
        [ServerCommand(NetCmd.GameStartInfo)]
        public static void SendGameStartInfo(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();
            Net.Server.SendTo(m.RequesterID, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});
            Net.Server.RefreshInGamePlayersList();
        }


        [ServerCommand(NetCmd.Game)]
        public static void GameHandler(NetworkMessage message) {
            var m = message.ReadMessage<NetPackGame>();
            switch (m.Command) {
                case Command.PutTile:
                    Net.Server.SendToAll(NetCmd.Game, m);
                    Net.Server.NextPlayerTurn();
                    break;
                default:
                    Net.Server.SendToAll(NetCmd.Game, m);
                    break;
            }
        }
        #endregion

        #region Private Methods
        public static void FormAndSendPlayersList() {
            string pList = Net.Player.Aggregate(string.Empty, (current, p) => current + (int)p.Color + Convert.ToInt32(p.IsReady) + p.PlayerName + Environment.NewLine);
            pList = pList.TrimEnd('\r', '\n');
            Net.Server.SendToAll(NetCmd.FormPlayersList, new NetPackMessage{ Message = pList});

            // Code for RichTextList
            //string pList = Net.Player.Aggregate(string.Empty, (current, p) => current + ColorString(p.Color, p.PlayerName) + Environment.NewLine);
            //pList = pList.TrimEnd('\r', '\n');
            //Net.Server.SendToAll(NetCmd.FormPlayersList, new NetPackMessage{Message = pList});
        }

        private static bool PlayerExist(string playerName) {
            return Net.Player.Any(p => p.PlayerName == playerName);
        }

        private static bool AllIsReady() {
            return Net.Player.All(p => p.IsReady);
        }
        #endregion
    }
}