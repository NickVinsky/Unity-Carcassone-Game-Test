using System;
using System.Linq;
using Code.Game.Data;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.Network {
    public class LobbyManager : NetworkManager {

        [Header("Fields For Connection")]
        public GameObject IP;
        public GameObject JoinPort;
        public GameObject HostPort;

        void Start() {
            MainGame.Player.UniKey = SystemInfo.deviceUniqueIdentifier;
        }

        public void HostGame() {
            Net.NetworkPort = Convert.ToInt32(HostPort.transform.FindChild("Text").GetComponent<Text>().text);
            MainGame.Player.PlayerName = GameObject.Find(GameRegulars.PlayerNameInputField).GetComponent<InputField>().text;
            Net.Server.StartWithClient();

            #region Old Register Initiation for Server

            //Net.Register.Add(NetCmd.ChatMessage, ServerRegister.ChatSendMessage, ClientRegister.ChatReceiveMessage);
            //Net.Register.Add(NetCmd.ChatHistory, ServerRegister.ChatSendHistory, ClientRegister.ChatReceiveHistory);
            //Net.Register.Add(NetCmd.PlayersList, ServerRegister.FormPlayersList, ClientRegister.RefreshPlayersList);
            //Net.Register.AddToClient(NetCmd.RefreshPlayersInfo, ClientRegister.RefreshPlayersInfo);
            //Net.Register.AddToClient(NetCmd.PlayerInfo, ClientRegister.ReceivePlayerInfo);
            //Net.Register.AddToClient(NetCmd.Err, ClientRegister.Err);
            //Net.Register.AddToClient(NetCmd.Inf, ClientRegister.Inf);

            #endregion
        }

        public void JoinGame() {
            Net.NetworkAddress = IP.transform.FindChild("Text").GetComponent<Text>().text;
            Net.NetworkPort = Convert.ToInt32(JoinPort.transform.FindChild("Text").GetComponent<Text>().text);
            if (Net.NetworkAddress == "127.0.0.1")  MainGame.Player.UniKey += "dublicate";
            MainGame.Player.PlayerName = GameObject.Find(GameRegulars.PlayerNameInputField).GetComponent<InputField>().text;
            Net.Client.Start();

            #region Old Register Initiation for Client
            //Net.Register.AddToClient(NetCmd.ChatMessage, ClientRegister.ChatReceiveMessage);
            //Net.Register.AddToClient(NetCmd.ChatHistory, ClientRegister.ChatReceiveHistory);
            //Net.Register.AddToClient(NetCmd.PlayersList, ClientRegister.RefreshPlayersList);
            //Net.Register.AddToClient(NetCmd.RefreshPlayersInfo, ClientRegister.RefreshPlayersInfo);
            //Net.Register.AddToClient(NetCmd.PlayerInfo, ClientRegister.ReceivePlayerInfo);
            //Net.Register.AddToClient(NetCmd.Err, ClientRegister.Err);
            //Net.Register.AddToClient(NetCmd.Inf, ClientRegister.Inf);
            #endregion
        }

        public void Disconnect() {
            switch (Net.IsServer) {
                case true:
                    Net.Server.Stop();
                    break;
                case false:
                    Net.Client.Stop();
                    break;
            }
            Net.Reset();
            MainMenuGUI.SwitchPanels(GameRegulars.PanelMultiplayer, GameRegulars.PanelLobby);
        }

        public void Ready() {
            MainGame.Player.IsReady = !MainGame.Player.IsReady;
            Net.Client.Send(NetCmd.Ready, Net.Client.MyInfo());
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            // Free slot check
            if (Net.PlayersList.Count == Net.MaxPlayers) {
                Net.Server.SendTo(conn.connectionId, NetCmd.Err, new NetPackErr{Err = ErrType.LobbyIsFull});
                return;
            }

            base.OnServerAddPlayer(conn, playerControllerId);


            Net.Server.SendTo(conn.connectionId, NetCmd.InitRegistration, new NetPackPlayerInfo {ID = conn.connectionId});
            //Net.Server.SendTo(conn.connectionId, NetCmd.Game, new NetPackGame{Command = Command.PlayerReturn});
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            Net.Server.DestroyPlayer(conn);
            if (conn.lastError != NetworkError.Ok){
                if (LogFilter.logError) {
                    Net.Client.ChatInfo(ConnInfo.PlayerDisconnected, conn.lastError.ToString(), Net.Server.GetPlayerName(conn.connectionId));
                }
            }

            try {
                var discPlayer = Net.PlayersList.First(p => p.ID == conn.connectionId);
                var index = Net.PlayersList.IndexOf(discPlayer);
                if (Net.Game.InLobby) Net.PlayersList.RemoveAt(index);
                else discPlayer.Left = true;
            } catch (Exception ex) {
                // ignored
                Debug.logger.Log(LogType.Exception, ex.Message);
            }

            Net.Server.ReformPlayersList();
        }
    }
}
