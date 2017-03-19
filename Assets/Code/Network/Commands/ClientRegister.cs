using System;
using Code.Game;
using Code.Game.Data;
using Code.GUI;
using Code.Network.Attributes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static Code.MainGame;

namespace Code.Network.Commands {
    public static class ClientRegister {
        //Debug.Log("Color = " + PlayerInfo.Color+ " ;ID = " + PlayerInfo.ConnectionId + " ;PlayerName = " + PlayerInfo.PlayerName + " ; IsRegistred = " + PlayerInfo.IsRegistred);
        public static void Subscribe(short msgType, NetworkMessageDelegate handler) {
            NetworkManager.singleton.client.RegisterHandler(msgType, handler);
        }
        public static NetPackPlayerInfo MyInfo() {
            return Net.Client.MyInfo();
        }
        public static string MyInfoDebug() {
            return "ID=" + Player.ID + " ;Name=" + Player.PlayerName + " ;Reg=" + Player.IsRegistred +
                   " ;Color=" + Player.Color + " ;Ready=" + Player.IsReady;
        }

        [ClientCommand(NetCmd.InitRegistration)]
        public static void ReceivePlayerID(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            Player.ID = m.ID;
            Player.WaitingForColorUpgrade = true;

            var myInfo = new NetPackPlayerInfo {
                UniKey = Player.UniKey,
                PlayerName = Player.PlayerName,
                ID = Player.ID,
                Color = PlayerColor.NotPicked,
                IsRegistred = Player.IsRegistred
            };

            Net.Client.Send(NetCmd.RegisterMe, myInfo);
        }

        [ClientCommand(NetCmd.Inf)]
        public static void Inf(NetworkMessage message) {
            var m = message.ReadMessage<NetPackInf>();
            switch (m.Inf) {
                case ConnInfo.PlayerRegistred:
                    Net.Client.ChatInfo(ConnInfo.PlayerRegistred);
                    Net.Client.Send(NetCmd.FreeColorRequest, MyInfo());
                    Player.IsRegistred = true;
                    Net.Client.Log("");
                    Net.Server.Log("");
                    MainMenuGUI.SwitchPanels(GameRegulars.PanelLobby, GameRegulars.PanelMultiplayer);
                    break;
            }
        }

        [ClientCommand(NetCmd.Err)]
        public static void Err(NetworkMessage message) {
            var m = message.ReadMessage<NetPackErr>();
            switch (m.Err) {
                case ErrType.PlayerNameOccupied:
                    Net.Client.Stop();
                    Net.Client.Log("Player with that name already exist on server!");
                    MainMenuGUI.SwitchPanels(GameRegulars.PanelMultiplayer, GameRegulars.PanelLobby);
                    break;
                case ErrType.LobbyIsFull:
                    Net.Client.Stop();
                    Net.Client.Log("Server is Full!");
                    MainMenuGUI.SwitchPanels(GameRegulars.PanelMultiplayer, GameRegulars.PanelLobby);
                    break;
                case ErrType.GameAlreadyStarted:
                    Net.Client.Stop();
                    Net.Client.Log("Game already running!");
                    MainMenuGUI.SwitchPanels(GameRegulars.PanelMultiplayer, GameRegulars.PanelLobby);
                    break;
            }
        }

        [ClientCommand(NetCmd.FreeColorRequest)]
        public static void ReceivePlayerColor(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            Player.Color = m.Color;
            Player.WaitingForColorUpgrade = false;
            Net.Client.Send(NetCmd.ReformLobbyPlayersList, Net.NullMsg);
        }

        [ClientCommand(NetCmd.ChatMessage)]
        public static void ChatReceiveMessage(NetworkMessage message) {
            LobbyInspector.AddMsg(message);
        }

        [ClientCommand(NetCmd.ChatHistory)]
        public static void ChatReceiveHistory(NetworkMessage message) {
            LobbyInspector.AddChatHistory(message);
        }

        [ClientCommand(NetCmd.Countdown)]
        public static void Countdown(NetworkMessage message) {
            LobbyInspector.AddCountdownMsg(message);
        }

        [ClientCommand(NetCmd.ReturnIntoGame)]
        public static void ReturnIntoGame(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayer>();
            Net.Game.GameStarted();
            SceneManager.LoadScene(GameRegulars.SceneGame);
            //var workingID = Player.ID;
            Player = m.Player;
            //Player.ID = workingID;

            Net.Client.UpdateScore();
            Net.Client.Send(NetCmd.TransferCache, new NetPackChatMessage {RequesterID = Player.ID});
            //Net.Client.Send(NetCmd.CurrentPlayerIs, new NetPackGame { Value = Net.Game.CurrentPlayerIndex, Color = Net.Game.CurrentPlayer});
        }

        [ClientCommand(NetCmd.TileCache)]
        public static void ReconstructTilesOnGrid(NetworkMessage message) {
            var m = message.ReadMessage<NetPackTileCache>();
            Tile.Reconstruct(m.Cell, m.TileID, m.TileIndex, m.Rotation, m.LocactionID, m.LocationOwner, m.FollowerType);
        }

        [ClientCommand(NetCmd.TileCacheFinish)]
        public static void ReconstructFinish(NetworkMessage message) {
            if (Player.Stage == GameStage.PlacingFollower) Tile.ShowPossibleFollowersLocations(Tile.LastPlacedTile, Follower.Meeple);
        }






        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="message"></param>








        [ClientCommand(NetCmd.FormLobbyPlayersList)]
        public static void RefreshPlayersList(NetworkMessage message) {
            LobbyInspector.RefreshPlayersList(message);
        }

        [ClientCommand(NetCmd.RefreshPlayersInfoAndReformPlayersList)]
        public static void RefreshPlayersInfo(NetworkMessage message) {
            Net.Client.Send(NetCmd.ReformPlayersListWithAdding, MyInfo());
        }

        [ClientCommand(NetCmd.UpdatePlayerInfo)]
        public static void UpdateMyInfo(NetworkMessage message) {
            Net.Client.Send(NetCmd.UpdatePlayerInfo, MyInfo());
        }

        #region InGameCommands
        [ClientCommand(NetCmd.Game)]
        public static void GameHandler(NetworkMessage message) {
            var m = message.ReadMessage<NetPackGame>();
            switch (m.Command) {
                case Command.Start:
                    //var ChatPanel = GameObject.Find("ChatPanel");
                    //GameObject.DontDestroyOnLoad(ChatPanel.transform.root);
                    Net.Game.CurrentPlayer = m.Color;
                    Player.MeeplesQuantity = GameRegulars.MaxMeeplesNumber;
                    Player.Score = 0;
                    SceneManager.LoadScene(GameRegulars.SceneGame);
                    Net.Client.UpdateScore();
                    Player.Stage = Net.Game.MyTurn() ? GameStage.Start : GameStage.Wait;
                    Net.Game.GameStarted();
                    Net.Client.Send(NetCmd.TransferCache, new NetPackChatMessage {RequesterID = Player.ID});
                    break;
                case Command.TilePicked:
                    Net.Game.TilePicked = true;
                    Net.Game.LastPickedTileIndex = m.Value;
                    Tile.Pick(m.Value, m.Byte);
                    break;
                case Command.TileNotPicked:
                    Net.Game.TilePicked = false;
                    break;
                case Command.MouseCoordinates:
                    Net.Game.tPos = m.Vect3;
                    break;
                case Command.CursorStreaming:
                    Net.Game.StreamCursor(m.Color, m.Vect3);
                    break;
                case Command.CursorStopStreaming:
                    Net.Game.StopStreamCursor(m.Color, m.Vect3);
                    break;
                case Command.RotateTile:
                    Tile.OnMouse.SetRotation(m.Value);
                    break;
                case Command.HighlightCell:
                    Tile.Highlight(m.Text, m.Value);
                    break;
                case Command.PutTile:
                    Tile.OnMouse.Put(m.Vector);
                    if (Net.Game.MyTurn()) Net.Game.PostTilePut(m.Vector);
                    break;
                case Command.Follower:
                    if (Player.Color != m.Color) Tile.Get(m.Text).AssignOpponentFollower(m.Color, (byte) m.Value, m.Follower);
                    break;
                case Command.RemovePlacement:
                    Tile.Get(m.Vector).GetLocation(m.Byte).RemovePlacement();
                    break;
                case Command.RemovePlacementMonk:
                    Tile.Get(m.Vector).RemovePlacement(m.Value);
                    break;
                case Command.NextPlayer:
                    Net.Game.CurrentPlayerIndex = m.Value;
                    Net.Game.CurrentPlayer = m.Color;
                    if (Deck.IsEmpty()) {
                        Player.Stage = GameStage.End;
                        if (Net.IsServer) ScoreCalc.Final();
                    } else {
                        Player.Stage = Net.Game.MyTurn() ? GameStage.Start : GameStage.Wait;
                    }
                    if (Net.IsServer) Net.Server.RefreshInGamePlayersList();
                    break;
            }
        }

        [ClientCommand(NetCmd.GameData)]
        public static void GetCurrentPlayerTurn(NetworkMessage message) {
            var m = message.ReadMessage<NetPackGame>();
            Net.Game.CurrentPlayer = m.Color;
            Net.Game.CurrentPlayerIndex = m.Byte;
            Net.Game.TilePicked = m.Trigger;
            Net.Game.LastPickedTileIndex = m.Value;
            if (Player.Stage == GameStage.PlacingTile) {
                Tile.Pick(Net.Game.LastPickedTileIndex, 0);
            }
        }

        [ClientCommand(NetCmd.InGamePlayers)]
        public static void InGamePlayers(NetworkMessage message) {
            var m = message.ReadMessage<NetPackMessage>().Message;
            Net.Client.RefreshInGamePlayersList(m);
        }


        #endregion
    }
}