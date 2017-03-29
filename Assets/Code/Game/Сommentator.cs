using System;
using Code.Game.Data;
using Code.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game {
    public static class Сommentator {
        private static readonly GameObject Commentator = GameObject.Find("Сommentator");
        private static readonly GameObject Finalists = GameObject.Find("GameResults");

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
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "";
                    break;
                case GameStage.Start:
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Возьмите тайл";
                    break;
                case GameStage.PlacingTile:
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Разместите тайл на поле";
                    break;
                case GameStage.PlacingFollower:
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "Назначьте последователя";
                    break;
                case GameStage.Finish:
                    break;
                case GameStage.End:
                    Commentator.GetComponent<Text>().text = "";
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "";
                    break;
                default:
                    Commentator.transform.FindChild("SubComment").GetComponent<Text>().text = "";
                    break;
            }
        }

        private static void MyTurn(string comment, PlayerColor myColor) {
            Commentator.GetComponent<Text>().text = "<color=#" + Net.ColorCode(myColor) + ">" + comment + "</color>";
        }

        private static void OpponentTurn(string comment, PlayerColor oppColor, string name) {
            Commentator.GetComponent<Text>().text = comment + " <color=#" + Net.ColorCode(oppColor) + ">" + name + "</color>";
        }

        public static void GameResults(string textList) {
            Finalists.transform.FindChild("Text").localScale = new Vector3(1f, 1f, 1f);
            var player = textList.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var outText = string.Empty;
            var outField = Finalists.transform.FindChild("Text");
            var fontSize = Finalists.transform.FindChild("Text").GetComponent<Text>().fontSize;

            foreach (var p in player) {
                var pColor = (PlayerColor) char.GetNumericValue(p[0]);
                var pScore = Convert.ToInt32(p.Substring(1, 4));
                var pName = p.Substring(5);

                outText += "<size=" + fontSize + "><color=#" + Net.ColorCode(pColor) + ">" + pName + "</color></size>: " + pScore + Environment.NewLine;
                fontSize -= 8;
            }

            outText = outText.TrimEnd('\r', '\n');
            outField.GetComponent<Text>().text = outText;
        }
    }
}