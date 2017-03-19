﻿using System;
using System.Collections;
using System.Linq;
using Code.Game.Data;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Code.MainGame;
using Object = UnityEngine.Object;

namespace Code.Network.Composition {
    public class ClientSide {
        public GameObject[] Meeples;

        public NetPackPlayerInfo MyInfo() {
            return new NetPackPlayerInfo {
                ID = Player.ID,
                PlayerName = Player.PlayerName,
                IsRegistred = Player.IsRegistred,
                IsReady = Player.IsReady,
                Color = Player.Color,
                FollowersNumber = Player.MeeplesQuantity,
                Score = Player.Score
            };
        }

        public void Start() {
            NetworkManager.singleton.networkAddress = Net.NetworkAddress;
            NetworkManager.singleton.networkPort = Net.NetworkPort;
            NetworkManager.singleton.StartClient();
            Net.Register.InitClient();

            Net.IsServer = false;
            Net.Game.SetOnline();
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

        public void SendUnreliable(short msgType, MessageBase msg) {
            NetworkManager.singleton.client.SendByChannel(msgType, msg, 1);
        }

        public void SendFollower(PlayerColor owner, byte id, string name, Follower type) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.Follower, Color = owner, Value = id, Text = name, Follower = type});
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

        public void SubtractFollower(PlayerColor playerColor, Follower type) {
            Net.Client.Send(NetCmd.SubtractFollower, new NetPackFollower{ Color = playerColor, followerType = type});
        }

        public void RefreshInGamePlayersList(string m) {
            string[] pList = m.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lastI = 0;

            for (var i = 0; i < pList.Length; i++) {
                var p = pList[i];
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                if (pList[i] == string.Empty) continue;
                var pColor = (int) char.GetNumericValue(p[0]);
                var pMoves = Convert.ToBoolean(char.GetNumericValue(p[1]));
                var pFollowersNumber = Convert.ToByte(char.GetNumericValue(p[2]));
                var pScore = Convert.ToInt32(p.Substring(3, 4));
                var pName = p.Substring(7);
                if (o.transform.FindChild("Name").GetComponent<Text>().text == Player.PlayerName) Player.MySlotNumberInGame = i;
                o.transform.FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Score").GetComponent<Text>().text = GameRegulars.ScoreText + pScore;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);

                if (Player.MySlotNumberInGame == i) {
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("MyMeeple");
                } else {
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("Meeple");
                }


                FillContainer(o, pColor, Follower.Meeple, pFollowersNumber);

                //o.transform.FindChild("MeepleStack").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);
                //o.transform.FindChild("MeepleStack").GetComponent<Image>().sprite = Resources.Load<Sprite>("MeepleStack/" + pFollowersNumber);



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

        private void FillContainer(GameObject slot, int ownerColorInt, Follower type, int quantity) {
            var renderInterval = 0f;
            var containerName = string.Empty;
            var spriteName = "Meeple";
            switch (type) {
                case Follower.Meeple:
                    renderInterval = 7f;
                    containerName = "MeepleContainer";
                    spriteName = "MeepleShadowed";
                    break;
                case Follower.BigMeeple:
                    break;
                case Follower.Mayor:
                    break;
                case Follower.Pig:
                    break;
                case Follower.Builder:
                    break;
            }

            var parent = slot.transform.FindChild(containerName);
            var children = (from Transform child in parent select child.gameObject).ToList();
            children.ForEach(Object.Destroy);

            Meeples = new GameObject[quantity];
            for (var meepleCounter = quantity - 1; meepleCounter >= 0; meepleCounter--) {
                Meeples[meepleCounter] = new GameObject("Meeple" + meepleCounter);
                Meeples[meepleCounter].transform.SetParent(parent);
                Meeples[meepleCounter].AddComponent<RectTransform>();
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchoredPosition = new Vector3(10 + meepleCounter * renderInterval, 0f, 0f);
                Meeples[meepleCounter].GetComponent<RectTransform>().localScale = new Vector3(26f, 26f, 26f);
                Meeples[meepleCounter].GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f, 0.9f);
                Meeples[meepleCounter].AddComponent<SpriteRenderer>();
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spriteName);
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().color = Net.Color((PlayerColor) ownerColorInt);
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().sortingOrder = 3;
            }
        }

        public void ChatMessage(string text) {
            var mToSend = new NetPackChatMessage {
                Player = Player.PlayerName,
                Message = text,
                RequesterID = Player.ID
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
                    sInfo = Player.PlayerName + " joined the lobby";
                    break;
                case ConnInfo.PlayerDisconnected:
                    sInfo = fromPlayer + " disconnected (" + extraMessage + ")";
                    break;
            }
            var mToSend = new NetPackChatMessage {
                IsInfoMessage = true,
                Player = "Server",
                Message = sInfo,
                RequesterID = Player.ID
            };
            Send(NetCmd.ChatMessage, mToSend);
        }
    }
}