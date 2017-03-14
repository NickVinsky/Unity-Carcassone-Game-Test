using System;
using System.Linq;
using Code.Game;
using Code.Game.Data;
using Code.Handlers;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Code.Network.Composition {
    public class GameMaster {
        private bool _offline = true;
        private bool _isStarted;
        public GameStage Stage;
        public PlayerColor CurrentPlayer = PlayerColor.NotPicked;
        public int CurrentPlayerIndex = -1;

        public bool TilePicked;
        public Vector2 tPos;

        public void SetOnline() { _offline = false; }
        public void SetOffline() { _offline = true; }
        public bool IsOnline() { return !_offline; }

        public void GameStarted() { _isStarted = true; }
        public void GameEnded() { _isStarted = false; }
        public bool IsStarted() { return _isStarted; }

        public void Launch() {
            Net.Server.Queue = new PlayerColor[Net.Player.Count];
            for (int i = 0; i < Net.Server.Queue.Length; i++) {
                Net.Server.Queue[i] = Net.Player[i].Color;
            }
            var random = new Random(DateTime.Now.Millisecond);
            Net.Server.Queue = Net.Server.Queue.OrderBy(x => random.Next()).ToArray();
            CurrentPlayerIndex = 0;
            CurrentPlayer = Net.Server.Queue[CurrentPlayerIndex];
            GameStarted();
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame{Command = Command.Start, Color = CurrentPlayer});
        }

        public bool MyTurn() { return CurrentPlayer == PlayerSync.PlayerInfo.Color; }

        // Update is called once per frame
        // This methods call when game is not offline
        // On client
        public void LocalClientUpadate(KeyInputHandler k) {
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;
            switch (Stage) {
                case GameStage.Wait:
                    if (TileOnMouseExist()) Tile.AttachToCoordinates(tPos);
                    break;
                case GameStage.Start:
                    //if (Input.GetKeyDown(KeyCode.F)) ScoreCalc.Final();
                    if (Input.GetKeyDown(k.PickTileFromDeck)) {
                        if (!Deck.IsEmpty()) {
                            var i = Deck.GenerateIndex();
                            Net.Client.Action(Command.TilePicked, i, Tile.Rotate.Random());
                            AttachTileToMouse();
                            Stage = GameStage.PlacingTile;
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
                    if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(k.ReturnTileToDeck)) {
                        Tile.LastPlaced().HideAll();
                        Stage = GameStage.Finish;
                    }
                    break;
                case GameStage.Finish:
                    //Net.Client.UpdateScore();
                    Net.Client.Action(Command.FinishTurn);
                    Stage = GameStage.Wait;
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
            if (!MyTurn()) return; // Проверка - мой ли сейчас ход
            if (Tile.Nearby.CanBeAttachedTo(c) && TileOnMouseExist()) {
                c.GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                Net.Server.SendToAll(NetCmd.Game, new NetPackGame{ Command = Command.HighlightCell, Text = c.name, Value = 1});
                return;
            }
            if (TileOnMouseExist()) {
                c.GetComponent<SpriteRenderer>().color = GameRegulars.CantAttachlColor;
                Net.Server.SendToAll(NetCmd.Game, new NetPackGame {Command = Command.HighlightCell, Text = c.name, Value = 2});
                return;
            }
            c.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }
        public void OnMouseExit(GameObject c) {
            if (!MyTurn()) return; // Проверка - мой ли сейчас ход
            if (c.GetComponent<SpriteRenderer>().color == GameRegulars.NormalColor) return;
            c.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame {Command = Command.HighlightCell, Text = c.name, Value = 0});
        }
        public void OnMouseUp(GameObject c) {
            if (!MyTurn()) return; // Проверка - мой ли сейчас ход
            if (!Tile.Nearby.CanBeAttachedTo(c) || MainGame.MouseState == MainGame.State.Dragging) return;
            PutTileFromMouse(c);
        }

        public void PostTilePut(Cell v) {
            if (PlayerSync.PlayerInfo.FollowersNumber > 0) {
                Stage = GameStage.PlacingFollower;
                var c  = GameObject.Find("cell#" + v.X + ":" + v.Y);
                Tile.ShowPossibleFollowersLocations(c);
            } else {
                Stage = GameStage.Finish;
            }
        }

        public void DeckClick(Vector2 t, Vector2 m) {
            if (!MyTurn()) return;
            if (Stage != GameStage.Start) return;
            if (Deck.IsEmpty()) return;
            var i = Deck.GenerateIndex();
            Net.Client.Action(Command.TilePicked, i);
            AttachTileToMouse();
            Tile.AttachToMouse();
            Stage = GameStage.PlacingTile;
        }

        private bool TileOnMouseExist() { return TilePicked; }

        private void PutTileFromMouse(GameObject o) {
            TilePicked = false;
            Net.Client.Action(Command.TileNotPicked);
            Net.Client.Action(Command.PutTile, MainGame.Grid.GetCellCoordinates(o));
        }

        private void RotateClockwise() {
            Tile.Rotate.Clockwise();
            Net.Client.Action(Command.RotateTile, Tile.OnMouse.GetRotation());
        }

        private void RotateCounterClockwise() {
            Tile.Rotate.CounterClockwise();
            Net.Client.Action(Command.RotateTile, Tile.OnMouse.GetRotation());
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