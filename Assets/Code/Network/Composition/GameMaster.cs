﻿using System;
using System.Linq;
using Code.Game;
using Code.Handlers;
using Code.Network.Commands;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Code.Network.Composition {
    public class GameMaster {
        private bool _offline = true;
        private bool _isStarted;
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


        // Update is called once per frame
        // This methods call when game is not offline
        // On client
        public void LocalClientUpadate(KeyInputHandler k) {
            if (TileOnMouseExist()) AttachTileToCoordinatesReceiver(tPos);

            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return;  // Проверка - мой ли сейчас ход
            if (TileOnMouseExist()) AttachTileToMouse();
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;
            if ((Input.GetKeyDown(k.RotateTileClockwise) || Input.GetMouseButtonDown(1)) && TileOnMouseExist())
                RotateClockwise();
            if (Input.GetKeyDown(k.RotateTileCounterClockwise) && TileOnMouseExist()) RotateCounterClockwise();
            if (Input.GetKeyDown(k.PickTileFromDeck)) {
                if (!TileOnMouseExist() && !Deck.DeckIsEmpty()) {
                    var i = Deck.GenerateIndex();
                    Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.TilePicked, Value = i});
                    AttachTileToMouse();
                }
                if (TileOnMouseExist()) RotateClockwise();
            }
            if (Input.GetKeyDown(k.ReturnTileToDeck)) {
                if (TileOnMouseExist()) {
                    ReturnTileToDeck();
                }
            }
        }

        public void OnMouseOver(GameObject c) {
            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return; // Проверка - мой ли сейчас ход
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
            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return; // Проверка - мой ли сейчас ход
            if (c.GetComponent<SpriteRenderer>().color == GameRegulars.NormalColor) return;
            c.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
            Net.Server.SendToAll(NetCmd.Game, new NetPackGame {Command = Command.HighlightCell, Text = c.name, Value = 0});
        }
        public void OnMouseUp(GameObject c) {
            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return; // Проверка - мой ли сейчас ход
            if (Tile.Nearby.CanBeAttachedTo(c) && MainGame.MouseState != MainGame.State.Dragging) PutTileFromMouse(c);
        }

        public void DeckClick(Vector2 t, Vector2 m) {
            if (CurrentPlayer != PlayerSync.PlayerInfo.Color) return;
            if (TileOnMouseExist() || Deck.DeckIsEmpty()) return;

            var i = Deck.GenerateIndex();
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.TilePicked, Value = i});
            AttachTileToMouse();
        }

        private bool TileOnMouseExist() { return TilePicked; }

        private void PutTileFromMouse(GameObject o) {
            TilePicked = false;
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.TileNotPicked});
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.PutTile, Vect2 = MainGame.Grid.GetCellCoordinates(o)});
        }

        private void RotateClockwise() {
            Tile.Rotate.Clockwise();
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.RotateTile, Value = Tile.OnMouse.GetRotation()});
        }

        private void RotateCounterClockwise() {
            Tile.Rotate.CounterClockwise();
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.RotateTile, Value = Tile.OnMouse.GetRotation()});
        }

        private void AttachTileToMouse() {
            var tp = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            tp = new Vector3(tp.x, tp.y, 0f);
            Net.Client.Send(NetCmd.Game, new NetPackGame{ Command = Command.MouseCoordinates, Vect2 = tp});
        }

        private void AttachTileToCoordinatesReceiver(Vector2 t) {
            Tile.AttachToCoordinates(t);
        }

        private void ReturnTileToDeck() {}

    }
}