using System;
using System.Collections.Generic;
using System.Linq;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Composition {
    public class ServerSide {
        public string ChatHistory = string.Empty;
        public PlayerColor[] Queue;

        public void CleanChat() { ChatHistory = string.Empty; }
        public void Log(string message) {
            MainMenuGUI.LogHost(message);
        }

        public void CommandResponse(NetPackChatMessage m, string text) {
            var mToSend = new NetPackChatMessage {
                RequesterID = m.RequesterID,
                Player = m.Player,
                Message = text,
                IsInfoMessage = true
            };
            SendTo(mToSend.RequesterID, NetCmd.ChatMessage, mToSend);
        }

        public void StartWithClient() {
            NetworkManager.singleton.networkPort = Net.NetworkPort;
            NetworkManager.singleton.StartHost();
        }

        public void Stop() {
            Net.Client.Stop();
            NetworkServer.Shutdown();

            //NetworkManager.singleton.StopHost();
        }

        public void SendToAll(short msgType, MessageBase message) {
            NetworkServer.SendToAll(msgType, message);
        }

        public void SendTo(int connId, short msgType, MessageBase message) {
            NetworkServer.SendToClient(connId, msgType, message);
        }

        public void DestroyPlayer(NetworkConnection conn) {
            NetworkServer.DestroyPlayersForConnection(conn);
            conn.Disconnect();
        }

        public string GetPlayerName(int id) {
            string result = string.Empty;
            try {
                result = Net.Player.First(p => p.ConnectionId == id).PlayerName;
            } catch (Exception e) {
                Debug.Log("GetPlayerNameExeption " + e.Message);
                //Console.WriteLine(e);
                //throw;
                result = string.Empty;
            }
            return result;
        }

        public void ClearPlayersList() {
            Net.Player.Clear();
        }

        public void ReformPlayersList() {
            if (Net.Game.IsStarted()) {
                //Net.Server.SendToAll(NetCmd.UpdatePlayerInfo, Net.NullMsg());
                RefreshInGamePlayersList();
            } else {
                //Net.Server.ClearPlayersList();
                //Net.Server.SendToAll(NetCmd.RefreshPlayersInfoAndReformPlayersList, new NetPackChatMessage());
                ServerRegister.FormAndSendPlayersList();
            }
        }

        public void RefreshInGamePlayersList() {
            List<PlayerColor> colorsToDelete = new List<PlayerColor>();
            string l = string.Empty;
            foreach (var q in Net.Server.Queue) {
                if (Net.Player.Any(p => p.Color == q)) {
                    var playerName = Net.Player.First(p => p.Color == q).PlayerName;
                    var isMoving = q == Net.Game.CurrentPlayer ? "1" : "0";
                    l += (int) q + isMoving + playerName + Environment.NewLine;
                } else {
                    // Игрок больше не в игре
                    colorsToDelete.Add(q);
                }
            }

            foreach (var c in colorsToDelete) {
                Queue = Queue.Where(p => p != c).ToArray();
            }

            l = l.TrimEnd('\r', '\n');
            Net.Server.SendToAll(NetCmd.InGamePlayers, new NetPackMessage{ Message = l});
        }

        public void AddToChatHistory(NetPackChatMessage m) {
            string h = Net.Server.ChatHistory;
            if (h != string.Empty) h += Environment.NewLine;
            string newMessage = string.Empty;
            if (m.IsInfoMessage) newMessage = "<color=#" + GameRegulars.ServerInfoColor + ">" + "[INFO] " + m.Message + "</color>";
            else newMessage = m.Player + ": " + m.Message;
            Net.Server.ChatHistory = h + newMessage;
        }
    }
}