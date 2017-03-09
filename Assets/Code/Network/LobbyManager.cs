using System;
using System.Linq;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.Network {
    public class LobbyManager : NetworkLobbyManager {

        [Header("Fields For Connection")]
        public GameObject IP;
        public GameObject JoinPort;
        public GameObject HostPort;

        public void HostGame() {
            Net.NetworkPort = Convert.ToInt32(HostPort.transform.FindChild("Text").GetComponent<Text>().text);
            Net.Server.StartWithClient();
            Net.Register.Init();
            Net.IsServer = true;

            Net.Game.SetOnline();
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
            Net.Client.Start();
            Net.Register.InitClient();
            Net.IsServer = false;

            Net.Game.SetOnline();
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
            PlayerSync.PlayerInfo.IsReady = !PlayerSync.PlayerInfo.IsReady;
            Net.Client.Send(NetCmd.UpdatePlayerInfoAndReformPList, Net.Client.MyInfo());
        }

        /*public override void OnServerConnect(NetworkConnection conn) {
            base.OnServerConnect(conn);
        }*/

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            // Free slot check
            if (Net.Player.Count == Net.MaxPlayers) {
                Net.Server.SendTo(conn.connectionId, NetCmd.Err, new NetPackErr{Err = ErrType.LobbyIsFull});
                return;
            }

            base.OnServerAddPlayer(conn, playerControllerId);

            Net.Server.SendTo(conn.connectionId, NetCmd.ConnIDReceive, new NetPackPlayerInfo {ID = conn.connectionId});
            Net.Server.SendTo(conn.connectionId, NetCmd.ChatHistory, new NetPackChatMessage {Message = Net.Server.ChatHistory});
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Net.Server.DestroyPlayer(conn);
            if (conn.lastError != NetworkError.Ok){
                if (LogFilter.logError) {
                    Net.Client.ChatInfo(ConnInfo.PlayerDisconnected, conn.lastError.ToString(), Net.Server.GetPlayerName(conn.connectionId));
                }
            }

            var discPlayer = Net.Player.First(p => p.ID == conn.connectionId);
            var index = Net.Player.IndexOf(discPlayer);
            Net.Player.RemoveAt(index);

            Net.Server.ReformPlayersList();
        }
    }
}
