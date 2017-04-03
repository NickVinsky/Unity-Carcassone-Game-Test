using System;
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
        private Vector3 v { get; set; }

        private const float OffsetFromLeft = 231.8773f;
        private const float PatronIconWidth = 52f;

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

        public void RequestAdditionalTurn() {
            Net.Client.Send(NetCmd.Game, new NetPackGame {Command = Command.AdditionalTurn});
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

        public void Action(Command command, PlayerColor color, byte byteValue, string text) {
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = command, Color = color, Value = byteValue, Text = text});
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
                var hasKingsPatronage = Convert.ToBoolean(char.GetNumericValue(p[tracker])); tracker++;
                var hasAtamansPatronage = Convert.ToBoolean(char.GetNumericValue(p[tracker])); tracker++;
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
                if (o.transform.FindChild("Panel").FindChild("Name").GetComponent<Text>().text == Player.PlayerName) Player.MySlotNumberInGame = i;
                o.transform.FindChild("Panel").FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Panel").FindChild("Score").GetComponent<Text>().text = GameRegulars.ScoreText + pScore;
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
                FillContainer(o, ref renderOffset, pColor, Follower.Barn, pBarnsQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.BigMeeple, pBigMeeplesQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Mayor, pMayorsQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Builder, pBuildersQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Meeple, pMeeplesQuantity);
                FillContainer(o, ref renderOffset, pColor, Follower.Pig, pPigsQuantity);

                var patronsCount = 0;
                AddPatron(hasKingsPatronage, o.transform.FindChild("King"), ref patronsCount, (PlayerColor) pColor);
                AddPatron(hasAtamansPatronage, o.transform.FindChild("Ataman"), ref patronsCount, (PlayerColor) pColor);

                v = o.transform.FindChild("Panel").GetComponent<RectTransform>().anchoredPosition;
                o.transform.FindChild("Panel").GetComponent<RectTransform>().anchoredPosition = new Vector3(OffsetFromLeft + PatronIconWidth * patronsCount, v.y, v.z);

                if (pMoves) {
                    o.GetComponent<Image>().color = GameRegulars.CurMovingPlayerColor;
                } else {
                    o.GetComponent<Image>().color = GameRegulars.BlankColor;
                    //o.GetComponent<Image>().color = PlayerInfo.MySlotNumberInGame == i ? GameRegulars.YourColor : GameRegulars.BlankColor;
                }


                lastI = i + 1;
            }
            for (var i = lastI; i < Net.MaxPlayers; i++) {
                var o = GameObject.Find("PlayerInGame(" + i + ")");
                o.transform.localScale = new Vector3(0f,0f,0f);
            }
        }

        private void AddPatron(bool hasPatron, Transform patron, ref int patronCounter, PlayerColor color) {
            if (hasPatron) {
                patron.GetComponent<Image>().enabled = true;
                v = patron.GetComponent<RectTransform>().anchoredPosition; // MeepleOffset + PatronIconWidth * patronCounter + 4f
                patron.GetComponent<RectTransform>().anchoredPosition = new Vector3(74f + PatronIconWidth * patronCounter, v.y, v.z);
                patron.GetComponent<Outline>().effectColor = Net.Color(color);
                patronCounter++;
            } else patron.GetComponent<Image>().enabled = false;
        }

        private void FillContainer(GameObject slot, ref float renderOffset, int ownerColorInt, Follower type, int quantity) {
            var renderInterval = 0f;
            var containerName = string.Empty;
            var followerName = string.Empty;
            var spriteName = "Meeple";
            var scale = new Vector3(26f, 26f, 26f);
            var sortingOrder = 3;

            switch (type) {
                case Follower.Meeple:
                    renderInterval = 3f;
                    containerName = "MeepleContainer";
                    followerName = "Meeple";
                    spriteName = "MeepleShadowed";
                    scale = new Vector3(26f, 26f, 26f);
                    sortingOrder = 11;
                    break;
                case Follower.BigMeeple:
                    renderInterval = 6f;
                    containerName = "MeepleContainer";
                    followerName = "BigMeeple";
                    spriteName = "MeepleShadowed";
                    scale = new Vector3(33f, 33f, 33f);
                    sortingOrder = 14;
                    break;
                case Follower.Mayor:
                    renderInterval = 8f;
                    containerName = "MeepleContainer";
                    followerName = "Mayor";
                    spriteName = "MayorShadowed";
                    scale = new Vector3(26f, 33f, 30f);
                    sortingOrder = 15;
                    break;
                case Follower.Pig:
                    renderInterval = 5f;
                    containerName = "MeepleContainer";
                    followerName = "Pig";
                    spriteName = "PigShadowed";
                    scale = new Vector3(28f, 28f, 28f);
                    sortingOrder = 10;
                    break;
                case Follower.Builder:
                    renderInterval = 7f;
                    containerName = "MeepleContainer";
                    followerName = "Builder";
                    spriteName = "BuilderShadowed";
                    scale = new Vector3(30f, 30f, 30f);
                    sortingOrder = 12;
                    break;
                case Follower.Barn:
                    renderInterval = 7f;
                    containerName = "MeepleContainer";
                    followerName = "Barn";
                    spriteName = "BarnShadowed";
                    scale = new Vector3(28f, 28f, 28f);
                    sortingOrder = 13;
                    break;
            }
            var nLen = followerName.Length - 1;

            var parent = slot.transform.FindChild("Panel").FindChild(containerName);

            var children = (from Transform child in parent
                            where child.name.Substring(0, 3) == followerName.Substring(0, 3)
                            select child.gameObject).ToList();

            children.ForEach(Object.Destroy);

            Meeples = new GameObject[quantity];
            for (var meepleCounter = 0; meepleCounter < quantity; meepleCounter++) {
                renderOffset += renderInterval + 5f;
                Meeples[meepleCounter] = new GameObject(followerName + meepleCounter);
                Meeples[meepleCounter].transform.SetParent(parent);
                Meeples[meepleCounter].transform.localPosition = new Vector3(Meeples[meepleCounter].transform.localPosition.x, Meeples[meepleCounter].transform.localPosition.y, 0f);
                Meeples[meepleCounter].AddComponent<RectTransform>();
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
                Meeples[meepleCounter].GetComponent<RectTransform>().anchoredPosition = new Vector3(renderOffset, 0f, 0f);
                Meeples[meepleCounter].GetComponent<RectTransform>().localScale = scale;
                Meeples[meepleCounter].GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f, 0.9f);
                Meeples[meepleCounter].AddComponent<SpriteRenderer>();
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spriteName);
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().color = Net.Color((PlayerColor) ownerColorInt);
                Meeples[meepleCounter].GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
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
            var sInfo = string.Empty;
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