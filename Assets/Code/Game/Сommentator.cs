using System;
using Code.Game.Data;
using Code.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game {
    public static class Сommentator {
        private static GameObject _commentator = GameObject.Find("Сommentator");

        private static GameStage _lastStage;
        private static PlayerColor _lastPlayerColor;

        private const string TxtOpponentTurn = "Ходит";
        private const string TxtMyTurn = "Ваш ход";

        public static void Comment(GameStage stage, PlayerColor myColor, PlayerColor curPlayerColor, string curPlayerName) {
            if (stage == _lastStage && curPlayerColor == _lastPlayerColor) return;
            WriteMainLine(myColor, curPlayerColor, curPlayerName);
            WriteSubLine(stage);
            _lastStage = stage;
            _lastPlayerColor = curPlayerColor;
        }

        private static void WriteMainLine(PlayerColor myColor, PlayerColor curPlayerColor, string curPlayerName) {
            if (myColor == curPlayerColor) {
                MyTurn(TxtMyTurn, myColor);
            } else {
                OpponentTurn(TxtOpponentTurn, curPlayerColor, curPlayerName);
            }
        }

        private static void WriteSubLine(GameStage stage) {
            switch (stage) {
                case GameStage.Wait:
                    _commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "";
                    break;
                case GameStage.Start:
                    _commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Возьмите тайл";
                    break;
                case GameStage.PlacingTile:
                    _commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Разместите тайл на поле";
                    break;
                case GameStage.PlacingFollower:
                    _commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Назначьте последователя";
                    break;
                case GameStage.Finish:
                    break;
                case GameStage.End:
                    break;
                default:
                    _commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "";
                    break;
            }
        }

        private static void MyTurn(string comment, PlayerColor myColor) {
            _commentator.GetComponent<Text>().text = "<color=#" + Net.ColorCode(myColor) + ">" + comment + "</color>";
        }

        private static void OpponentTurn(string comment, PlayerColor oppColor, string name) {
            _commentator.GetComponent<Text>().text = comment + " <color=#" + Net.ColorCode(oppColor) + ">" + name + "</color>";
        }
    }
}