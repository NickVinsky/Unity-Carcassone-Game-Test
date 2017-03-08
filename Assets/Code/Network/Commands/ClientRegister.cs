using System;
using Code.Game;
using Code.GUI;
using Code.Network.Attributes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Code.Network.PlayerSync;

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
            return "ID=" + PlayerInfo.ConnectionId + " ;Name=" + PlayerInfo.PlayerName + " ;Reg=" + PlayerInfo.IsRegistred +
                   " ;Color=" + PlayerInfo.Color + " ;Ready=" + PlayerInfo.IsReady;
        }

        [ClientCommand(NetCmd.Inf)]
        public static void Inf(NetworkMessage message) {
            var m = message.ReadMessage<NetPackInf>();
            switch (m.Inf) {
                case ConnInfo.PlayerRegistred:
                    PlayerInfo.IsRegistred = true;
                    Net.Client.Log("");
                    Net.Server.Log("");
                    MainMenuGUI.SwitchPanels(GameRegulars.PanelLobby, GameRegulars.PanelMultiplayer);
                    Net.Client.ChatInfo(ConnInfo.PlayerRegistred);
                    Net.Client.Send(NetCmd.FreeColorRequest, MyInfo());
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
            }
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

        [ClientCommand(NetCmd.FormPlayersList)]
        public static void RefreshPlayersList(NetworkMessage message) {
            LobbyInspector.RefreshPlayersList(message);
        }

        [ClientCommand(NetCmd.RefreshPlayersInfoAndReformPlayersList)]
        public static void RefreshPlayersInfo(NetworkMessage message) {
            Net.Client.Send(NetCmd.ReformPlayersListWithAdding, MyInfo());
        }

        [ClientCommand(NetCmd.ConnIDReceive)]
        public static void ReceivePlayerID(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            PlayerInfo.ConnectionId = m.ID;
            Net.Client.Send(NetCmd.FormPlayersList, MyInfo());
        }

        [ClientCommand(NetCmd.FreeColorRequest)]
        public static void ReceivePlayerColor(NetworkMessage message) {
            var m = message.ReadMessage<NetPackPlayerInfo>();
            PlayerInfo.Color = m.Color;
            PlayerInfo.WaitingForColorUpgrade = false;
            Net.Client.Send(NetCmd.UpdatePlayerInfoAndReformPList, MyInfo());
            //Net.Client.Send(NetCmd.FormPlayersList, MyInfo());
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
                    SceneManager.LoadScene(GameRegulars.SceneGame);
                    Net.Game.GameStarted();
                    Net.Client.Send(NetCmd.GameStartInfo, new NetPackChatMessage {RequesterID = PlayerInfo.ConnectionId});
                    break;
                case Command.TilePicked:
                    Net.Game.TilePicked = true;
                    Tile.Pick(m.Value, Tile.Rotate.Random());
                    break;
                case Command.TileNotPicked:
                    Net.Game.TilePicked = false;
                    break;
                case Command.MouseCoordinates:
                    Net.Game.tPos = m.Vect3;
                    break;
                case Command.RotateTile:
                    Tile.OnMouse.SetRotation(m.Value);
                    break;
                case Command.HighlightCell:
                    Tile.Highlight(m.Text, m.Value);
                    break;
                case Command.PutTile:
                    Tile.OnMouse.Put(m.Vect3);
                    break;
                case Command.NextPlayer:
                    Net.Game.CurrentPlayerIndex = m.Value;
                    Net.Game.CurrentPlayer = m.Color;
                    if (Net.IsServer) Net.Server.RefreshInGamePlayersList();
                    break;
            }
        }

        [ClientCommand(NetCmd.InGamePlayers)]
        public static void InGamePlayers(NetworkMessage message) {
            var m = message.ReadMessage<NetPackMessage>().Message;

            string[] pList = m.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lastI = 0;

            for (int i = 0; i < pList.Length; i++) {
                var p = pList[i];
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                if (pList[i] == string.Empty) continue;
                var pColor = (int) char.GetNumericValue(p[0]);
                var pMoves = Convert.ToBoolean(char.GetNumericValue(p[1]));
                var pName = p.Substring(2);
                o.transform.FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);

                if (o.transform.FindChild("Name").GetComponent<Text>().text == PlayerInfo.PlayerName) PlayerInfo.MySlotNumberInGame = i;

                if (pMoves) {
                    o.GetComponent<Image>().color = GameRegulars.CurMovingPlayerColor;
                } else {
                    o.GetComponent<Image>().color = PlayerInfo.MySlotNumberInGame == i ? GameRegulars.YourColor : GameRegulars.BlankColor;
                }


                lastI = i + 1;
            }
            for (int i = lastI; i < Net.MaxPlayers; i++) {
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                o.transform.localScale = new Vector3(0f,0f,0f);
            }
        }
        #endregion
    }
}