using System;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
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

        public void SendFollower(PlayerColor owner, byte id, string name) {
            Action(Command.Follower, owner, id, name);
        }

        public void Action(Command command) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command});
        }

        public void Action(Command command, int value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Value = value});
        }

        public void Action(Command command, int value, byte byteVal) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Value = value, Byte = byteVal});
        }

        public void Action(Command command, PlayerColor color, byte value, string text) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Color = color, Value = value, Text = text});
        }

        public void Action(Command command, string text, int value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Text = text, Value = value});
        }

        public void Action(Command command, Vector3 vector) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vect3 = vector});
        }

        public void Action(Command command, Cell vector) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vector = vector});
        }

        public void Action(Command command, Cell vector, byte value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vector = vector, Byte = value});
        }

        public void Action(Command command, Cell vector, int value) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vector = vector, Value = value});
        }

        public void UpdateScore() {
            Send(NetCmd.RefreshScore, MyInfo());
        }

        public void SubtractFollower(PlayerColor playerColor) {
            Net.Client.Send(NetCmd.SubtractFollower, new NetPackPlayerColor{ Color = playerColor});
        }

        public void RefreshInGamePlayersList(string m) {
            string[] pList = m.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lastI = 0;

            for (int i = 0; i < pList.Length; i++) {
                var p = pList[i];
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                if (pList[i] == string.Empty) continue;
                var pColor = (int) char.GetNumericValue(p[0]);
                var pMoves = Convert.ToBoolean(char.GetNumericValue(p[1]));
                var pFollowersNumber = Convert.ToByte(char.GetNumericValue(p[2]));
                var pScore = Convert.ToInt32(p.Substring(3, 4));
                var pName = p.Substring(7);
                if (o.transform.FindChild("Name").GetComponent<Text>().text == PlayerInfo.PlayerName) PlayerInfo.MySlotNumberInGame = i;
                o.transform.FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Score").GetComponent<Text>().text = GameRegulars.ScoreText + pScore;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);

                if (PlayerInfo.MySlotNumberInGame == i) {
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("MyMeeple");
                } else {
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("Meeple");
                }


                o.transform.FindChild("MeepleStack").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);
                o.transform.FindChild("MeepleStack").GetComponent<Image>().sprite = Resources.Load<Sprite>("MeepleStack/" + pFollowersNumber);



                if (pMoves) {
                    o.GetComponent<Image>().color = GameRegulars.CurMovingPlayerColor;
                } else {
                    o.GetComponent<Image>().color = GameRegulars.BlankColor;
                    //o.GetComponent<Image>().color = PlayerInfo.MySlotNumberInGame == i ? GameRegulars.YourColor : GameRegulars.BlankColor;
                }


                lastI = i + 1;
            }
            for (int i = lastI; i < Net.MaxPlayers; i++) {
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                o.transform.localScale = new Vector3(0f,0f,0f);
            }
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