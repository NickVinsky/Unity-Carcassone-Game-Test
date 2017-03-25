namespace Code.Game.Data {
    public enum PlayerColor {
        NotPicked = 0,
        Blue = 1,
        Cyan = 2,
        Green = 3,
        Yellow = 4,
        Red = 5,
        Purple = 6,
        Grey = 7
    }

    public enum GameStage {
        Wait,
        Start,
        PlacingTile,
        PlacingFollower,
        Finish,
        End
    }

    public enum Area {Empty, Field, Road, City, Monastery}

    public enum Side {
        Top = 0,
        Right = 1,
        Bot = 2,
        Left = 3
    }

    public enum Follower {
        Meeple,
        BigMeeple,
        Mayor,
        Pig,
        Builder,
        Barn,
        Wagon
    }

    public enum Placements {
        AllRestricted = 0,
        MeeplesPigsAndBuilders = 1,
        BigMeeples = 2,
        Mayor = 3,
        BarnAndWagons = 4
    }

    /*public enum Follower {
        None,
        Monk,
        Knight,
        Farmer,
        Thief
    }*/
    //public enum fields : int { TOP_left = 0, TOP_right = 1, RIGHT_top = 2, RIGHT_bot = 3, BOT_right = 4, BOT_left = 5, LEFT_bot = 6, LEFT_top = 7}
}
