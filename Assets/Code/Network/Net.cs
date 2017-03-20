using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using Code.Network.Commands;
using Code.Network.Composition;
using UnityEngine;

namespace Code.Network {
    public static class Net {
        public static RegisterHandler Register = new RegisterHandler();
        public static ServerSide Server = new ServerSide();
        public static ClientSide Client = new ClientSide();
        public static GameMaster Game = new GameMaster();
        public static List<PlayerInfo> PlayersList = new List<PlayerInfo>();
        public static bool IsServer;

        public static NetPackBlank NullMsg = new NetPackBlank();

        public static int NetworkPort;
        public static string NetworkAddress;
        public static float Timer = 3f;
        public static int MaxPlayers = 5;
        public static int ColorsCount = Enum.GetNames(typeof(PlayerColor)).Length - 1;

        static Net() {}

        public static void StartCountdown() {
            LobbyInspector.Timer = Timer;
            LobbyInspector.TimerTicks = (int) Timer;
            LobbyInspector.Countdown = true;
        }

        public static void StopCountdown() {
            LobbyInspector.Countdown = false;
        }

        public static void Reset() {
            MainGame.Player.Color = PlayerColor.NotPicked;
            MainGame.Player.ID = -10;
            MainGame.Player.IsRegistred = false;
            MainGame.Player.WaitingForColorUpgrade = false;
            MainGame.Player.IsReady = false;

            if (!IsServer) return;
            Server.CleanChat();
            PlayersList.Clear();
            IsServer = false;
            NetworkAddress = string.Empty;
            NetworkPort = 0;
        }

        public static string ColorString(PlayerColor pColor, string text) {
            var color = "<color=#" + ColorCode(pColor) + ">" + text + "</color>";
            /*
            switch (pColor) {
                case PlayerColor.Black:
                    color = "<color=#" + "000000" + ">" + target + "</color>";
                    break;
                case PlayerColor.Blue:
                    color = "<color=#" + "0061ff" + ">" + target + "</color>";
                    break;
                case PlayerColor.Green:
                    color = "<color=#" + "29b20e" + ">" + target + "</color>";
                    break;
                case PlayerColor.Red:
                    color = "<color=#" + "d62902" + ">" + target + "</color>";
                    break;
                case PlayerColor.Yellow:
                    color = "<color=#" + "efe813" + ">" + target + "</color>";
                    break;
            }*/
            return color;
        }

        public static Color Color(PlayerColor pColor) {
            switch (pColor) {
                case PlayerColor.Grey:
                    return GameRegulars.PlayerGrey;
                case PlayerColor.Blue:
                    return GameRegulars.PlayerBlue;
                case PlayerColor.Green:
                    return GameRegulars.PlayerGreen;
                case PlayerColor.Red:
                    return GameRegulars.PlayerRed;
                case PlayerColor.Yellow:
                    return GameRegulars.PlayerYellow;
                case PlayerColor.Purple:
                    return GameRegulars.PlayerPurple;
                case PlayerColor.Cyan:
                    return GameRegulars.PlayerCyan;
            }
            return GameRegulars.BlackColor;
        }

        public static string ColorCode(PlayerColor pColor) {
            var sourceColor = Color(pColor);
            var color = string.Empty;
            for (var i = 0; i < 3; i++) {
                float clrComponent = sourceColor[i];
                var hexValue = Convert.ToInt32(255 * clrComponent);
                var hexCode = hexValue.ToString("X");
                if (hexCode.Length == 1) hexCode = "0" + hexCode;
                //Debug.Log("Iteration " + i + " src = " + sourceColor[i] + " ; hex = " + "(" + hexValue + ")" + hexCode);
                color += hexCode;
            }
            //Debug.Log("Result = " + color);
            //int intAgain = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return color;
        }

        public static Color CombineColors(params Color[] aColors) {
            var result = new Color(0,0,0,0);
            result = aColors.Aggregate(result, (current, c) => current + c);
            result /= aColors.Length;
            return result;
        }
    }
}