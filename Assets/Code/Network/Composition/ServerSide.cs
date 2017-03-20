using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game;
using Code.Game.Data;
using Code.GUI;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Composition {
    public class ServerSide {
        public string ChatHistory = string.Empty;
        public PlayerColor[] Queue;

        public void NextPlayerTurn() {
            var nextPlayerIndex = Net.Game.CurrentPlayerIndex + 1;
            if (nextPlayerIndex >= Queue.Length) nextPlayerIndex = 0;
            var nextPlayerColor = Queue[nextPlayerIndex];
            var nextPlayerName = Net.PlayersList.First(p => p.Color == nextPlayerColor).PlayerName;
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame{ Command = Command.NextPlayer, Value = nextPlayerIndex, Color = nextPlayerColor, Text = nextPlayerName});
        }

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
            Net.Register.Init();

            Net.IsServer = true;
            Net.Game.SetOnline();
        }

        public void Stop() {
            Net.Client.Stop();
            NetworkServer.Shutdown();

            //NetworkManager.singleton.StopHost();
        }

        public void SendToAll(short msgType, MessageBase message) {
            NetworkServer.SendToAll(msgType, message);
        }

        public void SendToAll(short msgType, MessageBase message, int channelId) {
            NetworkServer.SendByChannelToAll(msgType, message, channelId);
        }

        public void SendTo(int connId, short msgType, MessageBase message) {
            NetworkServer.SendToClient(connId, msgType, message);
        }

        public void DestroyPlayer(NetworkConnection conn) {
            NetworkServer.DestroyPlayersForConnection(conn);
            conn.Disconnect();
        }

        public string GetPlayerName(int id) {
            var result = string.Empty;
            try {
                result = Net.PlayersList.First(p => p.ID == id).PlayerName;
            } catch (Exception e) {
                Debug.Log("GetPlayerNameExeption " + e.Message);
                //Console.WriteLine(e);
                //throw;
                result = string.Empty;
            }
            return result;
        }

        public void ClearPlayersList() {
            Net.PlayersList.Clear();
        }

        public void ReformPlayersList() {
            if (Net.Game.IsStarted()) {
                RefreshInGamePlayersList();
            } else {
                ServerRegister.FormAndSendLobbyPlayersList();
            }
        }

        public void SubtractFollower(PlayerColor playerColor, Follower type) {
            var player = Net.PlayersList.First(p => p.Color == playerColor);
            var index = Net.PlayersList.IndexOf(player);
            ScoreCalc.RecalcFollowersNumber(type, player, -1);
            Net.PlayersList[index] = player;
            RefreshInGamePlayersList();
        }

        public void RefreshInGamePlayersList() {
            var colorsToDelete = new List<PlayerColor>();
            var l = string.Empty;
            foreach (var q in Net.Server.Queue) {
                if (Net.PlayersList.Any(p => p.Color == q)) {
                    var curPlayer = Net.PlayersList.First(p => p.Color == q);
                    var isMoving = q == Net.Game.CurrentPlayerColor ? "1" : "0";
                    l += (int) q + isMoving + curPlayer.MeeplesQuantity.ToString("D1") +
                         curPlayer.Score.ToString("D4") + curPlayer.PlayerName + Environment.NewLine;
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
            var h = Net.Server.ChatHistory;
            if (h != string.Empty) h += Environment.NewLine;
            var newMessage = string.Empty;
            if (m.IsInfoMessage) newMessage = "<color=#" + GameRegulars.ServerInfoColor + ">" + "[INFO] " + m.Message + "</color>";
            else newMessage = m.Player + ": " + m.Message;
            Net.Server.ChatHistory = h + newMessage;
        }
    }
}