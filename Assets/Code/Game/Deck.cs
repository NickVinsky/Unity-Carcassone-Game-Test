﻿using System;
using Code.Game.Data;
using Code.GameComponents;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Game {
    public static class Deck {
        private static TilesPack<int> _tilesPack = new TilesPack<int>();
        private static int _lastPickedIndex;

        public static void Add(int type) {
            _tilesPack.Add(type);
        }

        public static int LastPickedIndex() { return _lastPickedIndex; }

        public static int GenerateIndex() {
            var maxIndex = _tilesPack.Count;
            var rnd = Random.Range(0, maxIndex);
            return rnd;
        }

        public static int GenerateIndexSafe() {
            var maxIndex = _tilesPack.Count;
            var rnd = Random.Range(0, maxIndex);

            return Tile.CannotBePlacedOnBoard(rnd) ? GenerateIndexSafe() : rnd;
        }

        public static int GetRandomTile() {
            var rnd = GenerateIndex();
            int result = _tilesPack[rnd];
            _lastPickedIndex = rnd;
            //_tilesPack.RemoveAt(rnd);
            return result;
        }

        public static int GetAndSaveIndex(int tileId) {
            for (int i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] == tileId) {
                    _lastPickedIndex = i;
                    return i;
                }
            }
            return 0;
        }

        public static int GetTileByIndex(int index) {
            int result = _tilesPack[index];
            _lastPickedIndex = index;
            //_tilesPack.RemoveAt(index);
            return result;
        }

        public static void Delete(int tileId) {
            for (var i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] != tileId) continue;
                _tilesPack.RemoveAt(i);
                return;
            }
        }

        public static void SetLastPickedIndex(int index) { _lastPickedIndex = index; }


        public static void DeleteLastPicked() { _tilesPack.RemoveAt(_lastPickedIndex); }

        public static void DeleteFirst() {
            _tilesPack.RemoveAt(0);
        }

        public static int DeckSize() {
            return _tilesPack.Count;
        }
        public static bool IsEmpty() {
            return _tilesPack.Count == 0;
        }
        public static bool LastTile() {
            return _tilesPack.Count == 1;
        }
        public static void InitVanillaDeck() {
            _tilesPack.OnAddOrRemove += EventOnAddOrRemove;
            _tilesPack.Clear();

            AddVanillaTiles();

            AddInnsAndCathedralsTiles();

            AddKingsTiles();

            AddTradersAndBuildersTiles();

            AddCustomTiles();
        }

        private static void AddVanillaTiles(int multiplier = 1) {
            AddTilesToDeck(1, 4, multiplier);
            AddTilesToDeck(2, 2, multiplier);
            AddTilesToDeck(3, 1, multiplier);
            AddTilesToDeck(4, 3, multiplier);
            AddTilesToDeck(5, 1, multiplier);
            AddTilesToDeck(6, 1, multiplier);
            AddTilesToDeck(7, 2, multiplier);
            AddTilesToDeck(8, 3, multiplier);
            AddTilesToDeck(9, 2, multiplier);
            AddTilesToDeck(10, 3, multiplier);
            AddTilesToDeck(11, 2, multiplier);
            AddTilesToDeck(12, 1, multiplier);
            AddTilesToDeck(13, 2, multiplier);
            AddTilesToDeck(14, 2, multiplier);
            AddTilesToDeck(15, 3, multiplier);
            AddTilesToDeck(16, 5, multiplier);
            AddTilesToDeck(17, 3, multiplier);
            AddTilesToDeck(18, 3, multiplier);
            AddTilesToDeck(19, 3, multiplier);
            AddTilesToDeck(20, 4, multiplier); // Starting Tile
            AddTilesToDeck(21, 8, multiplier);
            AddTilesToDeck(22, 9, multiplier);
            AddTilesToDeck(23, 4, multiplier);
            AddTilesToDeck(24, 1, multiplier);
        }

        private static void AddInnsAndCathedralsTiles(int multiplier = 1) {
            AddTilesToDeck(25, 1, multiplier);
            AddTilesToDeck(26, 2, multiplier);
            AddTilesToDeck(27, 2, multiplier);
            AddTilesToDeck(28, 2, multiplier);
            AddTilesToDeck(29, 3, multiplier);
            AddTilesToDeck(30, 2, multiplier);
            AddTilesToDeck(31, 2, multiplier);
            AddTilesToDeck(32, 3, multiplier);
            AddTilesToDeck(33, 3, multiplier);
            AddTilesToDeck(34, 2, multiplier);
            AddTilesToDeck(35, 1, multiplier);
            AddTilesToDeck(36, 1, multiplier);
            AddTilesToDeck(37, 1, multiplier);
            AddTilesToDeck(38, 1, multiplier);
            AddTilesToDeck(39, 1, multiplier);
            AddTilesToDeck(40, 1, multiplier);
            AddTilesToDeck(41, 5, multiplier);
        }

        private static void AddKingsTiles(int multiplier = 1) {
            AddTilesToDeck(52, 1, multiplier);
            AddTilesToDeck(53, 4, multiplier);
            AddTilesToDeck(54, 4, multiplier);
            AddTilesToDeck(55, 2, multiplier);
            AddTilesToDeck(56, 2, multiplier);
        }

        private static void AddTradersAndBuildersTiles(int multiplier = 1) {
            AddTilesToDeck(57, 1, multiplier);
            AddTilesToDeck(58, 4, multiplier);
            AddTilesToDeck(59, 3, multiplier);
            AddTilesToDeck(60, 4, multiplier);
        }

        private static void AddCustomTiles(int multiplier = 1) {
            AddTilesToDeck(81, 4, multiplier);
            AddTilesToDeck(82, 2, multiplier);
        }

        private static void AddTilesToDeck(int type, int quantity, int multiplier = 1) {
            var totalQuantity = quantity * multiplier;
            for (var i = 0; i < totalQuantity; i++) {
                AddTileToDeck(type);
            }
        }

        private static void AddTileToDeck(int type) {
            _tilesPack.Add(type);
        }

        public static void EventOnAddOrRemove(object sender, EventArgs e) {
            if (GameObject.Find(GameRegulars.DeckCounter) == null) return;
            GameObject.Find(GameRegulars.DeckCounter).GetComponent<Text>().text = DeckSize() + "x";
        }
    }
}
