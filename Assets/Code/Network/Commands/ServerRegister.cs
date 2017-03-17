using System;
using System.Linq;
using Code.Game;
using Code.Game.Data;
using Code.Network.Attributes;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Code.Network.Commands {
    public static class ServerRegister {
        public static void Subscribe(short msgType, NetworkMessageDelegate handler) {
            NetworkServer.RegisterHandler(msgType, handler);
        }

        [ServerCommand(NetCmd.RegisterMe)]
        public static void RegisterNewPlayer(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            if (Net.Game.InLobby()) AddLobbyPlayer(m);
            else ReturnPlayerToGame(m);
        }

        private static void AddLobbyPlayer(NetPackPlayerInfo m) {
            if (PlayerWithNameExist(m.PlayerName)) {
                Net.Server.SendTo(m.ID, NetCmd.Err, new NetPackErr{Err = ErrType.PlayerNameOccupied});
                return;
            }

            Net.StopCountdown();

            Net.PlayersList.Add(new PlayerInfo {UniKey = m.UniKey, ID = m.ID, PlayerName = m.PlayerName, FollowersNumber = GameRegulars.MaxFollowerNumbers, Score = 0});
            Net.Server.SendTo(m.ID, NetCmd.Inf, new NetPackInf{Inf = ConnInfo.PlayerRegistred});
            Net.Server.SendTo(m.ID, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});

            FormAndSendLobbyPlayersList();
        }

        private static void ReturnPlayerToGame(NetPackPlayerInfo m) {
            if (PlayerWithUniKeyExist(m.UniKey)) {
                var player = GetPlayer(m.UniKey);
                var index = GetIndexOfPlayer(m.UniKey);
                player.ID = m.ID;
                if (player.Stage == GameStage.PlacingTile) {
                    player.Stage = GameStage.Start;
                    // TODO
                    // Уничтожить Tile.OnMouse у всех остальных участников игры
                    // Создать новый Tile.OnMouse по последнему Net.Game.LastPickedTileIndex
                }
                Net.PlayersList[index] = player;

                Net.Server.SendTo(m.ID, NetCmd.ReturnIntoGame, new NetPackPlayer { Player = player});
            } else {
                Net.Server.SendTo(m.ID, NetCmd.Err, new NetPackErr{Err = ErrType.GameAlreadyStarted});
            }
        }

        private static void SendCachedTiles(int id) {
            foreach (var tile in Tile.Cache) {
                var cachedTile = new NetPackTileCache {
                    Cell = tile.Cell,
                    TileID = tile.TileID,
                    TileIndex = tile.TileIndex,
                    Rotation = tile.Rotation,
                    LocactionID = tile.LocactionID,
                    LocationOwner = tile.LocationOwner
                };
                Net.Server.SendTo(id, NetCmd.TileCache, cachedTile);
            }
            Net.Server.SendTo(id, NetCmd.TileCacheFinish, new EmptyMessage());
        }

        [ServerCommand(NetCmd.FreeColorRequest)]
        public static void GetFreeColor(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();

            var origColor = m.Color;
            var origColorIndex = (int) origColor;

            if (origColor == (PlayerColor) Net.ColorsCount) m.Color = PlayerColor.NotPicked;
            for (var i = 1; i <= Net.ColorsCount; i++) {
                var newColorIndex = i + origColorIndex;
                if (newColorIndex > Net.ColorsCount) newColorIndex -= Net.ColorsCount;
                var newColor = (PlayerColor) newColorIndex;
                if (Net.PlayersList.Any(p => p.Color == newColor)) continue;

                var player = Net.PlayersList.First(p => p.ID == m.ID);
                var index = Net.PlayersList.IndexOf(player);
                player.Color = newColor;
                Net.PlayersList[index] = player;

                Net.Server.SendTo(m.ID, NetCmd.FreeColorRequest, new NetPackPlayerInfo{ Color = newColor});
                return;
            }
            Net.Server.SendTo(m.ID, NetCmd.FreeColorRequest, new NetPackPlayerInfo{ Color = origColor});
        }

        [ServerCommand(NetCmd.ChatHistory)]
        public static void ChatSendHistory(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();
            Net.Server.SendTo(m.RequesterID, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});
        }

        [ServerCommand(NetCmd.ChatMessage)]
        public static void ChatSendMessage(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();

            if (m.Message.StartsWith("/")) {
                string pInfo;
                var p = Net.PlayersList.Single(s => s.ID == m.RequesterID);
                switch (m.Message) {
                    case "/myinfo":
                        pInfo = "[AboutPlayer] Name#ID: " + p.PlayerName + "#" + p.ID;
                        pInfo += ", Color: " + p.Color;
                        pInfo += ", Registered: " + p.IsRegistred;
                        Net.Server.CommandResponse(m, pInfo);
                        break;
                    case "/followers":
                        pInfo = "[Followers] " + p.FollowersNumber + " left";
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

        [ServerCommand(NetCmd.Ready)]
        public static void SetPlayerReady(NetworkMessage message) {
            var sender = message.ReadMessage<NetPackPlayerInfo>();

            var curPlayer = Net.PlayersList.First(p => p.ID == sender.ID);
            var index = Net.PlayersList.IndexOf(curPlayer);

            curPlayer.IsReady = sender.IsReady;

            Net.PlayersList[index] = curPlayer;

            if (AllIsReady()) Net.StartCountdown();
            FormAndSendLobbyPlayersList();
        }





        /// <summary>
        /// /////////////////////////
        /// </summary>
        /// <param name="message"></param>









        [ServerCommand(NetCmd.UpdatePlayerInfo)]
        public static void UpdatePlayerInfo(NetworkMessage message) {
            var sender = message.ReadMessage<NetPackPlayerInfo>();

            var curPlayer = Net.PlayersList.First(p => p.ID == sender.ID);
            var index = Net.PlayersList.IndexOf(curPlayer);
            var lastID = Net.PlayersList[index].ID;
            var lastColor = Net.PlayersList[index].Color;

            curPlayer.ID = sender.ID;
            curPlayer.Color = sender.Color;
            curPlayer.PlayerName = sender.PlayerName;
            curPlayer.IsRegistred = sender.IsRegistred;
            curPlayer.IsReady = sender.IsReady;
            curPlayer.FollowersNumber = sender.FollowersNumber;
            curPlayer.Score = sender.Score;

            //if (lastID != curPlayer.ConnectionId || lastColor != curPlayer.Color) Net.Server.SendToAll(NetCmd.RefreshPlayersInfoAndReformPlayersList, NullMsg);
            if(index != -1) Net.PlayersList[index] = curPlayer;
            if (Net.Game.IsStarted()) return;
            if (AllIsReady()) Net.StartCountdown();
            //if (!Net.Game.IsStarted() && AllIsReady()) Net.StartCountdown();
            //else Net.StopCountdown();
        }

        [ServerCommand(NetCmd.UpdatePlayerInfoAndReformPList)]
        public static void UpdatePlayerInfoAndReformPList(NetworkMessage message) {
            UpdatePlayerInfo(message);
            ReformLobbyPlayersList(message);
        }

        [ServerCommand(NetCmd.FormLobbyPlayersList)] // This Command Receives Only Player Name And ConnectionID
        public static void FormPlayersList(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            if (PlayerWithNameExist(m.PlayerName)) {
                if (Net.Game.IsOnline()) {
                    var curPlayer = Net.PlayersList.First(p => p.PlayerName == m.PlayerName);
                    curPlayer.ID = m.ID;
                    FormAndSendLobbyPlayersList();
                    return;
                }
                Net.Server.SendTo(m.ID, NetCmd.Err, new NetPackErr{Err = ErrType.PlayerNameOccupied});
                return;
            }
            if (!m.IsRegistred) {
                Net.Server.SendTo(m.ID, NetCmd.Inf, new NetPackInf{Inf = ConnInfo.PlayerRegistred});
                Net.StopCountdown();
            }

            Net.PlayersList.Add(new PlayerInfo {ID = m.ID, PlayerName = m.PlayerName, FollowersNumber = GameRegulars.MaxFollowerNumbers, Score = 0});
            FormAndSendLobbyPlayersList();
        }

        [ServerCommand(NetCmd.ReformLobbyPlayersList)] // This Command Used For Form Players List With Full knowlege about player (color etc.)
        public static void ReformLobbyPlayersList(NetworkMessage message) {
            FormAndSendLobbyPlayersList();
        }

        [ServerCommand(NetCmd.ReformPlayersListWithAdding)] // This Command Receives Full Player Info And Reform List (when someone leave for example)
        public static void ReformPlayersListWithAdding(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            Net.PlayersList.Add(new PlayerInfo {
                ID = m.ID,
                PlayerName = m.PlayerName,
                Color = m.Color,
                IsRegistred = m.IsRegistred,
                FollowersNumber = GameRegulars.MaxFollowerNumbers,
                Score = 0
            });
            if (Net.Game.IsStarted()) {
                Net.Server.RefreshInGamePlayersList();
            } else {
                FormAndSendLobbyPlayersList();
            }
        }

        #region InGameCommands
        [ServerCommand(NetCmd.TransferCache)]
        public static void TransferChatToGame(NetworkMessage message) {
            var m = message.ReadMessage<NetPackChatMessage>();
            Net.Server.SendTo(m.RequesterID, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});
            SendCachedTiles(m.RequesterID);
            Net.Server.SendTo(m.RequesterID, NetCmd.GameData, new NetPackGame { Byte = (byte) Net.Game.CurrentPlayerIndex,
                Color = Net.Game.CurrentPlayer, Trigger = Net.Game.TilePicked, Value = Net.Game.LastPickedTileIndex});
            Net.Server.RefreshInGamePlayersList();
        }

        [ServerCommand(NetCmd.GameData)]
        public static void SendCurrentPlayerTurn(NetworkMessage message) {
            var m = message.ReadMessage<NetPackGame>();
            //Net.Server.SendTo(m.RequesterID ,NetCmd.CurrentPlayerIs, new NetPackGame { Value = Net.Game.CurrentPlayerIndex, Color = Net.Game.CurrentPlayer});
        }

        [ServerCommand(NetCmd.RefreshScore)]
        public static void RefreshScore(NetworkMessage message) {
            var sender = message.ReadMessage<NetPackPlayerInfo>();

            var curPlayer = Net.PlayersList.First(p => p.ID == sender.ID);
            var index = Net.PlayersList.IndexOf(curPlayer);

            curPlayer.FollowersNumber = sender.FollowersNumber;
            curPlayer.Score = sender.Score;

            if(index != -1) Net.PlayersList[index] = curPlayer;

            //UpdatePlayerInfo(message);
            Net.Server.RefreshInGamePlayersList();
        }

        [ServerCommand(NetCmd.SubtractFollower)]
        public static void SubtractFollower(NetworkMessage message) {
            var pColor = message.ReadMessage<NetPackPlayerColor>().Color;
            Net.Server.SubtractFollower(pColor);
        }

        [ServerCommand(NetCmd.Game)]
        public static void GameHandler(NetworkMessage message) {
            var m = message.ReadMessage<NetPackGame>();
            switch (m.Command) {
                case Command.FinishTurn:
                    //Net.Server.SendToAll(NetCmd.Game, m);
                    Net.Server.NextPlayerTurn();
                    break;
                case Command.TilePicked:
                    Net.Server.SendToAll(NetCmd.Game, m);
                    break;
                case Command.MouseCoordinates:
                    Net.Server.SendToAll(NetCmd.Game, m, 1);
                    break;
                case Command.Follower:
                    Tile.Cache.Last().LocactionID = (sbyte) m.Value;
                    Tile.Cache.Last().LocationOwner = m.Color;
                    Net.Server.SendToAll(NetCmd.Game, m);
                    break;
                case Command.CursorStreaming:
                    Net.Server.SendToAll(NetCmd.Game, m, 1);
                    break;
                case Command.CursorStopStreaming:
                    Net.Server.SendToAll(NetCmd.Game, m, 1);
                    break;
                default:
                    Net.Server.SendToAll(NetCmd.Game, m);
                    break;
            }
        }
        #endregion

        #region Private Methods
        public static void FormAndSendLobbyPlayersList() {
            string pList = Net.PlayersList.Aggregate(string.Empty, (current, p) => current + (int)p.Color + Convert.ToInt32(p.IsReady) + p.PlayerName + Environment.NewLine);
            pList = pList.TrimEnd('\r', '\n');
            Net.Server.SendToAll(NetCmd.FormLobbyPlayersList, new NetPackMessage{ Message = pList});

            // Code for RichTextList
            //string pList = Net.Player.Aggregate(string.Empty, (current, p) => current + ColorString(p.Color, p.PlayerName) + Environment.NewLine);
            //pList = pList.TrimEnd('\r', '\n');
            //Net.Server.SendToAll(NetCmd.FormPlayersList, new NetPackMessage{Message = pList});
        }

        private static PlayerInfo GetPlayer(string uniKey) {
            return Net.PlayersList.First(p => p.UniKey == uniKey);
        }

        private static int GetIndexOfPlayer(string uniKey) {
            return Net.PlayersList.IndexOf(GetPlayer(uniKey));
        }

        private static bool PlayerWithNameExist(string playerName) {
            return Net.PlayersList.Any(p => p.PlayerName == playerName);
        }

        private static bool PlayerWithUniKeyExist(string uniKey) {
            return Net.PlayersList.Any(player => player.UniKey.Equals(uniKey));
        }

        private static int PlayerIndex(string playerName) {
            for (var i = 0; i < Net.PlayersList.Count; i++) {
                if (Net.PlayersList[i].PlayerName == playerName) {
                    return i;
                }
            }
            return -1;
        }

        private static bool AllIsReady() {
            return Net.PlayersList.All(p => p.IsReady);
        }
        #endregion
    }
}