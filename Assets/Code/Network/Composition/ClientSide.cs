using System;
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

        public void Action(Command command, Cell vector, PlayerColor color) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Vector = vector, Color = color});
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
            Net.Client.Send(NetCmd.SubtractFollower, new NetPackFollower{ Color = playerColor, FollowerType = type});
        }

        public void RefreshInGamePlayersList(string m) {
            var pList = m.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lastI = 0;

            for (var i = 0; i < pList.Length; i++) {
                var tracker = 0;
                var p = pList[i];
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                if (pList[i] == string.Empty) continue;
                var pColor = (int) char.GetNumericValue(p[tracker]); tracker++;
                var pMoves = Convert.ToBoolean(char.GetNumericValue(p[tracker])); tracker++;
                var pMeeplesQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pBigMeeplesQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pMayorsQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pPigsQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pBuildersQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pBarnsQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pWagonsQuantity = Convert.ToByte(p.Substring(tracker, 2)); tracker += 2;
                var pScore = Convert.ToInt32(p.Substring(tracker, 4)); tracker += 4;
                Player.Score = pScore;

                var pName = p.Substring(tracker);
                if (o.transform.FindChild("Name").GetComponent<Text>().text == Player.PlayerName) Player.MySlotNumberInGame = i;
                o.transform.FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Score").GetComponent<Text>().text = GameRegulars.ScoreText + pScore;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);

                if (Player.MySlotNumberInGame == i) {
                    Player.MeeplesQuantity = pMeeplesQuantity;
                    Player.BigMeeplesQuantity = pBigMeeplesQuantity;
                    Player.MayorsQuantity = pMayorsQuantity;
                    Player.PigsQuantity = pPigsQuantity;
                    Player.BuildersQuantity = pBuildersQuantity;
                    Player.BarnsQuantity = pBarnsQuantity;
                    Player.WagonsQuantity = pWagonsQuantity;
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("MyMeeple");
                } else {
                    o.transform.FindChild("Meeple").GetComponent<Image>().sprite = Resources.Load<Sprite>("Meeple");
                }


                var renderOffset = 0f;
                FillContainer(o, ref renderOffset, pColor, Follower.BigMeeple, pBigMeeplesQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Mayor, pMayorsQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Meeple, pMeeplesQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Pig, pPigsQuantity);

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

        private void FillContainer(GameObject slot, ref float renderOffset, int ownerColorInt, Follower type, int quantity) {
            var renderInterval = 0f;
            var containerName = string.Empty;
            var followerName = string.Empty;
            var spriteName = "Meeple";
            var scale = new Vector3(26f, 26f, 26f);

            switch (type) {
                case Follower.Meeple:
                    renderInterval = 3f;
                    containerName = "MeepleContainer";
                    followerName = "Meeple";
                    spriteName = "MeepleShadowed";
                    scale = new Vector3(26f, 26f, 26f);
                    break;
                case Follower.BigMeeple:
                    renderInterval = 6f;
                    //containerName = "BigMeepleContainer";
                    containerName = "MeepleContainer";
                    followerName = "BigMeeple";
                    spriteName = "MeepleShadowed";
                    scale = new Vector3(33f, 33f, 33f);
                    break;
                case Follower.Mayor:
                    renderInterval = 8f;
                    //containerName = "MayorContainer";
                    containerName = "MeepleContainer";
                    followerName = "Mayor";
                    spriteName = "MayorShadowed";
                    scale = new Vector3(26f, 33f, 30f);
                    break;
                case Follower.Pig:
                    renderInterval = 5f;
                    //containerName = "PigContainer";
                    containerName = "MeepleContainer";
                    followerName = "Pig";
                    spriteName = "PigShadowed";
                    scale = new Vector3(28f, 28f, 26f);
                    break;
                case Follower.Builder:
                    break;
            }
            var nLen = followerName.Length - 1;

            var parent = slot.transform.FindChild(containerName);
            //if (parent.childCount == quantity) return;

            var children = (from Transform child in parent
                            where child.name.Substring(0, 3) == followerName.Substring(0, 3)
                            select child.gameObject).ToList();
            //parent.DetachChildren();
            children.ForEach(Object.Destroy);

            Meeples = new GameObject[quantity];
            // for (var meepleCounter = quantity - 1; meepleCounter >= 0; meepleCounter--) {
            for (var meepleCounter = 0; meepleCounter < quantity; meepleCounter++) {
                renderOffset += renderInterval + 5f;
                Meeples[meepleCounter] = new GameObject(followerName + meepleCounter);
                Meeples[meepleCounter].transform.SetParent(parent);
                Meeples[meepleCounter].AddComponent<RectTransform>();
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchoredPosition = new Vector3(renderOffset, 0f, 0f);
                // 10 + meepleCounter * renderInterval
                Meeples[meepleCounter].GetComponent<RectTransform>().localScale = scale;
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