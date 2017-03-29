using System;
using System.Linq;
using Code.Game;
using Code.Game.Data;
using Code.Handlers;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using static Code.MainGame;
using Object = UnityEngine.Object;

namespace Code.Network.Composition {
    public class GameMaster {
        public bool InitComplite { get; private set; }

        public PlayerColor CurrentPlayerColor = PlayerColor.NotPicked;
        public int CurrentPlayerIndex = -1;
        public string CurrentPlayerName;

        public bool TilePicked;
        public Vector2 Pos;

        public bool HasAdditionalTurn { get; set; }
        public int TilesLeftBeforeAdditionalTurn { get; set; }

        private float _streamingCursorOffsetX;
        private float _streamingCursorOffsetY;

        public int LastPickedTileIndex { get; set; }
        public bool CursorIsStreaming { get; set; }

        public bool ChatDisfocused { get; set; }

        public void SetOnline() { IsOffline = false; }
        public void SetOffline() { IsOffline = true; }
        public bool IsOnline => !IsOffline;
        public bool IsOffline { get; private set; } = true;

        public void GameStarted() { IsStarted = true; }
        public void GameEnded() { IsStarted = false; }
        public bool IsStarted { get; private set; }
        public bool InLobby => !IsStarted;

        public GameObject[] Pointer;

        // Выполняется только на сервере
        public void Launch() {
            Net.Server.Queue = new PlayerColor[Net.PlayersList.Count];
            for (var i = 0; i < Net.Server.Queue.Length; i++) {
                Net.Server.Queue[i] = Net.PlayersList[i].Color;
            }
            var random = new Random(DateTime.Now.Millisecond);
            Net.Server.Queue = Net.Server.Queue.OrderBy(x => random.Next()).ToArray();
            CurrentPlayerIndex = 0;
            CurrentPlayerColor = Net.Server.Queue[CurrentPlayerIndex];
            CurrentPlayerName = Net.PlayersList.First(p => p.Color == CurrentPlayerColor).PlayerName;
            GameStarted();
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame{Command = Command.Start, Color = CurrentPlayerColor, Text = CurrentPlayerName});
        }

        public void Init() {
            Pointer = new GameObject[Net.ColorsCount + 1];
            var sprite = Resources.Load<Sprite>("Pointer");
            _streamingCursorOffsetX = sprite.rect.width / 200;
            _streamingCursorOffsetY = sprite.rect.height / 200;
            for (var i = 1; i <= Net.ColorsCount; i++) {
                var curColor = (PlayerColor) i;
                Pointer[i] = new GameObject(i + "//" + curColor);
                Object.DontDestroyOnLoad(Pointer[i]);
                Pointer[i].AddComponent<RectTransform>();
                Pointer[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1f);
                Pointer[i].AddComponent<SpriteRenderer>();
                Pointer[i].GetComponent<SpriteRenderer>().sprite = sprite;
                Pointer[i].GetComponent<SpriteRenderer>().color = Net.Color(curColor);
                Pointer[i].GetComponent<SpriteRenderer>().sortingOrder = 101;

                //Pointer[i].transform.localScale = new Vector3(0f,0f,0f);
                Pointer[i].GetComponent<SpriteRenderer>().enabled = false;
            }
            InitComplite = true;
        }

        public bool MyTurn => CurrentPlayerColor == Player.Color;

        public void StreamCursor(PlayerColor playerColor, Vector3 vector) {
            if (!InitComplite) return;
            var colorInt = (int) playerColor;
            vector = new Vector3(vector.x + _streamingCursorOffsetX, vector.y - _streamingCursorOffsetY, 0f);
            Pointer[colorInt].GetComponent<SpriteRenderer>().enabled = true;
            Pointer[colorInt].transform.position = vector;
        }

        public void StopStreamCursor(PlayerColor playerColor, Vector3 vector) {
            if (!InitComplite) return;
            var colorInt = (int) playerColor;
            Pointer[colorInt].GetComponent<SpriteRenderer>().enabled = false;
        }



        public void FixedUpdate(KeyInputHandler k) {

        }

        // Update is called once per frame
        // This methods call when game is not offline
        // On client
        public void LocalClientUpadate(KeyInputHandler k) {
            Сommentator.Comment(Player.Stage, Player.Color, CurrentPlayerColor, CurrentPlayerName);

            if (Input.GetKey(k.CursorStreaming)) {
                Cursor.visible = false;
                var pos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                pos = new Vector3(pos.x, pos.y, 0f);
                var packet = new NetPackGame {
                    Command = Command.CursorStreaming,
                    Vect3 = pos,
                    Color = Player.Color
                };
                Net.Client.SendUnreliable(NetCmd.Game, packet);
            } else {
                if (TilePicked && CurrentPlayerColor == Player.Color) Cursor.visible = false;
                else Cursor.visible = true;
                var packet = new NetPackGame {
                    Command = Command.CursorStopStreaming,
                    Color = Player.Color
                };
                Net.Client.SendUnreliable(NetCmd.Game, packet);
            }

            if (Input.GetKey(k.ShowLastPlacedTile)) {
                Tile.ShowLastPlaced();
                if (Input.GetKey(k.FocusOnTileOnMouse)) Tile.FocusLastPlaced();
            }
            else Tile.HideLastPlaced();


            if (Player.Stage == GameStage.Wait)
                if (TileOnMouseExist) Tile.AttachToCoordinates(Pos);
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;
            switch (Player.Stage) {
                case GameStage.Wait:
                    if (Input.GetKey(k.FocusOnTileOnMouse)) Tile.OnMouse.Focus();
                    //if (TileOnMouseExist()) Tile.AttachToCoordinates(tPos);
                    break;
                case GameStage.Start:
                    if (Input.GetKeyDown(k.PickTileFromDeck)) {
                        if (!Deck.IsEmpty) {
                            var i = Deck.GenerateIndexSafe();
                            Net.Client.Action(Command.TilePicked, i, Tile.Rotate.Random);
                            AttachTileToMouse();
                            Player.Stage = GameStage.PlacingTile;
                        }
                    }
                    break;
                case GameStage.PlacingTile:
                    Tile.AttachToMouse();
                    if (Input.GetKeyDown(k.RotateTileClockwise) || Input.GetMouseButtonDown(1)) RotateClockwise();
                    if (Input.GetKeyDown(k.RotateTileCounterClockwise)) RotateCounterClockwise();
                    if (Input.GetKeyDown(k.PickTileFromDeck)) RotateClockwise();
                    if (Input.GetKeyDown(k.ReturnTileToDeck)) ReturnTileToDeck();
                    //Stage = GameStage.PlacingFollower; // OnMouseUp()
                    break;
                case GameStage.PlacingFollower:
                    if (Input.GetKeyDown(KeyCode.Tab)) {
                        Tile.LastPlaced.ShowNextPossiblePlacement();
                    }
                    if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(k.ReturnTileToDeck)) {
                        Tile.LastPlaced.HideAll();
                        Player.Stage = GameStage.Finish;
                    }
                    break;
                case GameStage.Finish:
                    //Net.Client.UpdateScore();
                    Net.Client.Action(Command.FinishTurn);
                    Player.Stage = GameStage.Wait;
                    break;
                case GameStage.End:
                    //ScoreCalc.Final();
                    break;
                default:
                    //throw new ArgumentOutOfRangeException();
                    Debug.Log("ArgumentOutOfRangeException");
                    break;
            }

            #region OldLogic

            /*if (TileOnMouseExist()) Tile.AttachToCoordinates(tPos);

            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return;  // Проверка - мой ли сейчас ход
            if (TileOnMouseExist()) AttachTileToMouse();
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;
            if ((Input.GetKeyDown(k.RotateTileClockwise) || Input.GetMouseButtonDown(1)) && TileOnMouseExist())
                RotateClockwise();
            if (Input.GetKeyDown(k.RotateTileCounterClockwise) && TileOnMouseExist()) RotateCounterClockwise();
            if (Input.GetKeyDown(k.PickTileFromDeck)) {
                if (!TileOnMouseExist() && !Deck.DeckIsEmpty()) {
                    var i = Deck.GenerateIndex();
                    Net.Client.Action(Command.TilePicked, i);
                    AttachTileToMouse();
                }
                if (TileOnMouseExist()) RotateClockwise();
            }
            if (Input.GetKeyDown(k.ReturnTileToDeck)) {
                if (TileOnMouseExist()) {
                    ReturnTileToDeck();
                }
            }*/

            #endregion
        }

        public void OnMouseOver(GameObject c) {
            if (!MyTurn) return; // Проверка - мой ли сейчас ход
            if (Tile.Nearby.TileOnMouseCanBeAttachedTo(c) && TileOnMouseExist) {
                c.GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                Net.Server.SendToAll(NetCmd.Game, new NetPackGame{ Command = Command.HighlightCell, Text = c.name, Value = 1});
                return;
            }
            if (TileOnMouseExist) {
                c.GetComponent<SpriteRenderer>().color = GameRegulars.CantAttachlColor;
                Net.Server.SendToAll(NetCmd.Game, new NetPackGame {Command = Command.HighlightCell, Text = c.name, Value = 2});
                return;
            }
            c.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }
        public void OnMouseExit(GameObject c) {
            if (!MyTurn) return; // Проверка - мой ли сейчас ход
            if (c.GetComponent<SpriteRenderer>().color == GameRegulars.NormalColor) return;
            c.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame {Command = Command.HighlightCell, Text = c.name, Value = 0});
        }
        public void OnMouseUp(GameObject c) {
            if (!MyTurn) return; // Проверка - мой ли сейчас ход
            if (!Tile.Nearby.TileOnMouseCanBeAttachedTo(c) || MouseState == State.Dragging) return;
            PutTileFromMouse(c);
        }

        public void PostTilePut() {
            //if (Player.MeeplesQuantity > 0) {
                Player.Stage = GameStage.PlacingFollower;
                //var c  = GameObject.Find("cell#" + v.X + ":" + v.Y);
                Tile.ShowPossibleFollowersLocations(Tile.LastPlacedTile, Follower.Meeple);
            //} else {
                //Player.Stage = GameStage.Finish;
            //}
        }

        public void DeckClick(Vector2 t, Vector2 m) {
            if (!MyTurn) return;
            if (Player.Stage != GameStage.Start) return;
            if (Deck.IsEmpty) return;
            var i = Deck.GenerateIndexSafe();
            Net.Client.Action(Command.TilePicked, i);
            AttachTileToMouse();
            Tile.AttachToMouse();
            Player.Stage = GameStage.PlacingTile;
        }

        private bool TileOnMouseExist => TilePicked;

        private void PutTileFromMouse(GameObject o) {
            TilePicked = false;
            Net.Client.Action(Command.TileNotPicked);
            Net.Client.Action(Command.PutTile, MainGame.Grid.GetCellCoordinates(o), Player.Color);
        }

        private void RotateClockwise() {
            Tile.Rotate.Clockwise();
            Net.Client.Action(Command.RotateTile, Tile.OnMouse.GetRotation);
        }

        private void RotateCounterClockwise() {
            Tile.Rotate.CounterClockwise();
            Net.Client.Action(Command.RotateTile, Tile.OnMouse.GetRotation);
        }

        private void AttachTileToMouse() {
            var tp = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            tp = new Vector3(tp.x, tp.y, 0f);
            Net.Client.Action(Command.MouseCoordinates, tp);
        }

        private void ReturnTileToDeck() {
            //Stage = GameStage.Start;
        }

    }
}