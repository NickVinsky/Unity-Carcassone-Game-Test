using System;
using UnityEngine;

namespace Code.Game.Data {
    public struct GameRegulars {
        public static int EnumFollowersCount = Enum.GetNames(typeof(Follower)).Length;
        public static int EnumPlacementsCount = Enum.GetNames(typeof(Placements)).Length;

        public static string[] ngTitleF = {"Empress","Queen","Princess","Mistress","Marquise","Countess","Duchess","Baronet","Frau","Abess"};
        public static string[] ngTitleM = {"Emperor","King","Prince","Duke","Marquis","Earl","Count","Viscount","Baron","Lord","Despot","Magister","Cardinal"};
        public static string[] ngNobPart = {" al "," de "," di "," der "," del "," von "," van "," Mc", " Le", " la "," La"};
        public static string[] ngNameF = {
            "Leia","Diana","Annete","Aurielle","Belinda","Franny","Geneva","Grace","Gwyneth","Katherine","Lilith","Mia","Ophelia","Keira","Mavine",
            "Ahri","Akali","Anivia","Anniera","Ashe","Caitlyn","Camille","Cassiopeia","Elise","Evelynn","Fiora","Irelia","Jannette",
            "Khalista","Karma","Kayle","Blank","Leona","Lissandra","Leeloo","Lux","Fortuna","Morgana","Nidalee","Orianna","Quinn",
            "Sejuani","Shyvana","Sivir","Sona","Syndra","Taliyah","Tristana","Vayne","Zyra",
            "Meelina","Katana","Sindel","Holly","Khalissa","Diva"
        };
        public static string[] ngNameM = {"Sade","Barwick","Gaylord","Gerhardt","Henrik","Matiz","Rast","Duck","Zihte","Zul","Fedor","Arthas",
            "Garen","Jarvan","Warwick","Alistar","Aatrox","Amumu","Aurelion","Azir","Bard","Braum","Darius","Draven","Mundo","Ezreal","Fidele",
            "Galio","Gragas","Graves","Ivern","Jayce","Karthus","Kassaidin","Khaz","Lucian","Malzahar","Mordekaiser","Nasus","Nautilus","Olaf",
            "Pantheon","Renekton","Ryze","Sion","Skarner","Swain","Taric","Teemo","Tryndamere","Udyr","Urgot","Varus","Veigar","Vladimir","Xerath",
            "Xin","Zhao","Yorick","Zilus",
            "Salek","Grey"
        };

        public delegate int IntDelegate();
        public delegate string StringDelegate();
        public delegate void EventDelegate(object sender, EventArgs e);

        public const string TileTag = "TileOnBoard";
        public const string EmptyCellTag = "EmptyCell";

        public const string TileOnMouseName = "TileOnMouse";

        public const string SceneGame = "game";
        public const string SceneMainMenu = "mainMemu";

        public const string NewGameButton = "NewGameButton";
        public const string NetGameButton = "NetGameButton";
        public const string HostGameButton = "HostGameButton";
        public const string JoinGameButton = "JoinGameButton";
        public const string QuitGameButton = "QuitGameButton";
        public const string ButtonText = "Text";

        public const string PanelNetGame = "NetGame";
        public const string PanelLobby = "LobbyPanel";
        public const string PanelMultiplayer = "MultiplayerPanel";
        public const string PanelHost = "HostGamePanel";
        public const string PanelJoin = "JoinGamePanel";
        public const string PanelPlayerName = "PlayerNamePanel";
        public const string PanelChat = "ChatPanel";

        public const string PlayerNameInputField = "PlayerNameInputField";
        public const string IPFIeld = "IPInputField";
        public const string PortField = "PortInputField";
        public const string ChatFieldName = "ChatInput";
        public const string ChatHistoryName = "ChatHistory";
        public const string PlayersListField = "PlayersList";

        public const string OverlayBackground = "Overlay Background";
        public const string GameTable = "Game Table";
        public const string OverlayMiddle = "Overlay Middle";
        public const string DeckButton = "Deck Button";
        public const string DeckCounter = "Deck Counter";

        public const string ScoreText = "Score: ";

        public static readonly Color PlayerBlue = new Color(0f, 0.38f, 0.93f, 1f);
        public static readonly Color PlayerCyan = new Color(0.2f, 0.77f, 1f, 1f);
        public static readonly Color PlayerGreen = new Color(0.38f, 0.93f, 0f, 1f);
        public static readonly Color PlayerYellow = new Color(0.97f, 0.95f, 0.24f, 1f);
        public static readonly Color PlayerRed = new Color(0.96f, 0.22f, 0.24f, 1f);
        public static readonly Color PlayerPurple = new Color(0.91f, 0.17f, 0.88f, 1f);
        public static readonly Color PlayerGrey = new Color(0.52f, 0.52f, 0.52f, 1f);

        public static readonly Color BlackColor = Color.black;
        public static readonly Color CurMovingPlayerColor = new Color(0.89f, 0.89f, 0.09f, 0.68f);
        public static readonly Color YourColor = new Color(0.2f, 0.45f, 0.24f, 0.24f);
        public static readonly Color FreeColor = new Color(1f, 1f, 1f, 0.5f);
        public static readonly Color BlankColor = new Color(1f, 1f, 1f, 0f);

        public static readonly Color NormalColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        public static readonly Color CanAttachColor = new Color(0.6F, 1.0F, 0.2F, 1F);
        public static readonly Color CantAttachlColor = new Color(1.0F, 0.18F, 0.18F, 1F);
        public const string ServerInfoColor = "146880";
        public const string ServerCountdownColor = "b5463b";
        public const string ServerGameLaunchingColor = "1c8205";

        public static readonly Vector2 FollowerPositionCenter = new Vector2(0f, 0.06f);
        public static readonly Vector2 FollowerPositionTop = new Vector2(0.02f, 0.334f);
        public static readonly Vector2 FollowerPositionBot = new Vector2(-0.02f, -0.262f);
        public static readonly Vector2 FollowerPositionLeft = new Vector2(-0.3f, 0.045f);
        public static readonly Vector2 FollowerPositionRight = new Vector2(0.3f, 0f);
        public static readonly Vector2 FollowerPositionSEMid = new Vector2(0.1f, -0.075f);
        public static readonly Vector2 FollowerPositionSWMid = new Vector2(-0.1f, -0.075f);
        public static readonly Vector2 FollowerPositionSE = new Vector2(0.35f, -0.275f);
        public static readonly Vector2 FollowerPositionSW = new Vector2(-0.275f, -0.25f);

        public static readonly float ScrollSpeed = 0.25f;
        public static readonly float CamMoveSpeed = 0.15f;
        public static readonly float CameraDistanceMax = 15.0f;
        public static readonly float CameraDistanceMin = 1.75f;

        public static readonly float TileSizeX = 256.0f;
        public static readonly float TileSizeY = 256.0f;
        public static readonly float FollowersScale = 2.909f;

        #region Errors
        public const string JoinLogField = "JoinLog";
        public const string HostLogField = "HostLog";
        public const string CreatingServer = "Creating Lobby...";
        public const string JoiningServer = "Attempt to Join...";
        public const string NoPlayerNameError = "You must enter your name first!";
        #endregion

        public const string StatusReadyText = "I'm Ready!";
        public const string StatusNotReadyText = "I'm Not Ready Yet!";

        public const string LN_NewGameButton = "Play Solo";
        public const string LN_HostGameButton = "Host Game";
        public const string LN_JoinGameButton = "Join Game";
        public const string LN_QuitGameButton = "Quit";

    }
}
