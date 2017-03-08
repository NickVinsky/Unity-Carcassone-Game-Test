using System;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.Network {
    public static class LobbyInspector {

        public static GameObject ChatHistory;
        public static GameObject ChatField;
        public static GameObject PlayersList;

        public static float Timer;
        public static int TimerTicks;
        public static bool Countdown = false;

        //public static GameObject PlayerInLobby = (GameObject) Resources.Load("Prefabs/PlayerLobby");
        //public static GameObject newPlayer;


        // Use this for initialization
        public static void Init() {
            ChatHistory = GameObject.Find(GameRegulars.ChatHistoryName);
            ChatField = GameObject.Find(GameRegulars.ChatFieldName);
            PlayersList = GameObject.Find(GameRegulars.PlayersListField);
            //newPlayer = Object.Instantiate(PlayerInLobby, Vector3.zero, Quaternion.identity, GameObject.Find("PlayersPanel").transform);
            //newPlayer.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(_halfX, -_halfY, 0f);
            //Debug.Log("anchoredPosition" + test.GetComponent<RectTransform>().anchoredPosition);
            //Debug.Log("anchoredPosition3D" + test.GetComponent<RectTransform>().anchoredPosition3D);
            //Debug.Log("position" + test.GetComponent<RectTransform>().position);
            //Debug.Log("localPosition" + test.GetComponent<RectTransform>().localPosition);
        }
	
        // Update is called once per frame
        public static void Update() {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                string s = ChatField.GetComponent<InputField>().text;
                if (s != string.Empty) {
                    Net.Client.ChatMessage(s);
                    ChatField.GetComponent<InputField>().text = string.Empty;
                }
                ChatField.GetComponent<InputField>().ActivateInputField();
            }

            // Only Server-Side
            if (Countdown) {
                Timer -= Time.deltaTime;
                if (TimerTicks == -1) {
                    Countdown = false;
                    Net.Game.Launch();
                }
                if (!(Math.Floor(Timer) < TimerTicks)) return;
                Net.Server.SendToAll(NetCmd.Countdown, new NetPackMessage{Message = TimerTicks.ToString()});
                TimerTicks--;
            }

            // Debug Command
        }

        public static void RefreshPlayersListOld(NetworkMessage message) {
            var m = message.ReadMessage<NetPackMessage>();
            PlayersList.GetComponent<Text>().text = string.Empty;
            PlayersList.GetComponent<Text>().text = m.Message;
        }
        public static void RefreshPlayersList(NetworkMessage message) {
            var m = message.ReadMessage<NetPackMessage>().Message;

            string[] pList = m.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lastI = 0;

            for (int i = 0; i < pList.Length; i++) {
                var p = pList[i];
                var o = GameObject.Find("PlayerLobby(" + i + ")");
                if (pList[i] == string.Empty) continue;
                var pColor = (int) char.GetNumericValue(p[0]);
                var pReady = Convert.ToBoolean(char.GetNumericValue(p[1]));
                var pName = p.Substring(2);
                o.transform.FindChild("Name").GetComponent<Text>().text = pName;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = Net.Color((PlayerColor) pColor);

                if (o.transform.FindChild("Name").GetComponent<Text>().text == PlayerSync.PlayerInfo.PlayerName)
                    PlayerSync.PlayerInfo.MySlotNumber = i;

                if (i == PlayerSync.PlayerInfo.MySlotNumber) {
                    o.GetComponent<Image>().color = GameRegulars.YourColor;
                    //o.transform.FindChild("Toggle").GetComponent<Toggle>().interactable = true;
                    o.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = PlayerSync.PlayerInfo.IsReady;
                    GameObject.Find("ReadyButtonText").GetComponent<Text>().text = PlayerSync.PlayerInfo.IsReady ?
                        GameRegulars.StatusNotReadyText : GameRegulars.StatusReadyText;
                } else {
                    o.GetComponent<Image>().color = GameRegulars.BlankColor;
                    //o.transform.FindChild("Toggle").GetComponent<Toggle>().interactable = false;
                    o.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = pReady;
                }
                  lastI = i + 1;
            }
            for (int i = lastI; i < Net.MaxPlayers; i++) {
                var o = GameObject.Find("PlayerLobby(" + i + ")");
                o.transform.FindChild("Name").GetComponent<Text>().text = " ~ Free Slot ~ ";
                o.GetComponent<Image>().color = GameRegulars.BlankColor;
                o.transform.FindChild("Meeple").GetComponent<Image>().color = GameRegulars.FreeColor;
                o.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;
                //o.transform.FindChild("Toggle").GetComponent<Toggle>().interactable = false;
            }
        }

        public static void AddChatHistory(NetworkMessage message) {
            NetPackChatMessage m = message.ReadMessage<NetPackChatMessage>();
            ChatHistory.GetComponent<Text>().text = m.Message;
        }

        public static void AddMsg(NetworkMessage message) {
            NetPackChatMessage m = message.ReadMessage<NetPackChatMessage>();
            string h = ChatHistory.GetComponent<Text>().text;
            if (h != string.Empty) h += Environment.NewLine;
            string newMessage = string.Empty;
            if (m.IsInfoMessage) newMessage = "<color=#" + GameRegulars.ServerInfoColor + ">" + "[INFO] " + m.Message + "</color>";
            else newMessage = m.Player + ": " + m.Message;
            ChatHistory.GetComponent<Text>().text = h + newMessage;
        }

        public static void AddCountdownMsg(NetworkMessage message) {
            var m = message.ReadMessage<NetPackMessage>();
            var h = ChatHistory.GetComponent<Text>().text;
            if (h != string.Empty) h += Environment.NewLine;
            var newMessage = string.Empty;
            newMessage = m.Message.Equals("0") ?
                "<color=#" + GameRegulars.ServerGameLaunchingColor + ">Launching game...</color>" :
                "<color=#" + GameRegulars.ServerCountdownColor + ">" + "The game begins in " + m.Message +
                "...</color>";
            //if (m.Message.Equals("0")) newMessage = "<color=#" + GameRegulars.ServerCountdownColor + ">Launching game...</color>";
            //else newMessage = "<color=#" + GameRegulars.ServerCountdownColor + ">" + "The game begins in " + m.Message + "...</color>";
            ChatHistory.GetComponent<Text>().text = h + newMessage;
        }
    }
}
