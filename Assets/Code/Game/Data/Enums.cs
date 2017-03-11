﻿namespace Code.Game.Data {
    public enum GameStage {
        Wait,
        Start,
        PlacingTile,
        PlacingFollower,
        Finish
    }

    public enum Area {Empty, Field, Road, City, Monastery}

    public enum Side {
        Top = 0,
        Right = 1,
        Bot = 2,
        Left = 3
    }

    public enum Follower {
        None,
        Monk,
        Knight,
        Farmer,
        Thief
    }
    //public enum fields : int { TOP_left = 0, TOP_right = 1, RIGHT_top = 2, RIGHT_bot = 3, BOT_right = 4, BOT_left = 5, LEFT_bot = 6, LEFT_top = 7}
}