using Code.Game.Data;
using UnityEngine;
using static Code.Network.PlayerSync;

namespace Code.Game {
    public static class ScoreCalc {

        // After tile putting
        public static void Count(GameObject cell) {

            //MainGame.ChangeGameStage(GameStage.Finish);
        }

        //After follower assignment
        public static void ApplyFollower() {
            PlayerInfo.FollowersNumber--;

            //MainGame.ChangeGameStage(GameStage.Finish);
        }

        private static void Monastery(){}
        private static void Road(){}
        private static void City(){}
    }
}