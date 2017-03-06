using System;
using Code.Handlers;
using Code.Tiles;
using UnityEngine;
using static Code.GameRegulars;

namespace Code
{
    public class Grid {
        private readonly float _gridX = TileSizeX / 100;
        private readonly float _gridY = TileSizeY / 100;

        private readonly int _sizeX;
        private readonly int _sizeY;

        public Grid(int x, int y) {
            _sizeX = x;
            _sizeY = y;
        }
        public Grid() {
            _sizeX = 5;
            _sizeY = 5;
        }

        private int _minX, _maxY, _minY, _maxX;
        
        public void Make(){
            _minX = _sizeX / 2 - _sizeX + _sizeX % 2;
            _maxX = _sizeX / 2;
            _minY = _sizeY / 2 - _sizeY + _sizeY % 2;
            _maxY = _sizeY / 2;
            for (var iX = _minX; iX <= _maxX; iX++) {
                for (var iY = _minY; iY <= _maxY; iY++) {
                    AddCell(iX, iY);
                }
            }
        }

        public Vector2 GetCellCoordinates(GameObject cell) {
            char[] separatingChars = { '#', ':' };
            var coordinates = cell.name.Split(separatingChars);
            var x = Convert.ToInt32(coordinates[1]);
            var y = Convert.ToInt32(coordinates[2]);
            return new Vector2(x, y);
        }

        public void CheckBounds(GameObject tileToCheck) {
            var tilePos = GetCellCoordinates(tileToCheck);
            if ((int)tilePos.x == _minX) AddCellsLeft();
            if ((int)tilePos.x == _maxX) AddCellsRight();
            if ((int)tilePos.y == _minY) AddCellsBot();
            if ((int)tilePos.y == _maxY) AddCellsTop();
        }

        private void AddCell(int x, int y) {
            var tile = new GameObject("cell#" + x + ":" + y);
            tile.transform.SetParent(GameObject.Find(GameTable).transform);
            tile.AddComponent<SpriteRenderer>();
            tile.AddComponent<BoxCollider2D>();
            tile.AddComponent<MouseOnGrid>();
            tile.AddComponent<Tile>();
            tile.GetComponent<Tile>().InitTile(0);
            var position = new Vector3(_gridX * x, _gridY * y, 0.0f);
            tile.transform.position = position;
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grid");
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1 ;
            tile.GetComponent<SpriteRenderer>().color = NormalColor;
        }

        public void AddCellsTop() {
            _maxY++;
            for (var iX = _minX; iX <= _maxX; iX++){
                AddCell(iX, _maxY);
            }
        }

        public void AddCellsBot(){
            _minY--;
            for (var iX = _minX; iX <= _maxX; iX++){
                AddCell(iX, _minY);
            }            
        }

        public void AddCellsLeft(){
            _minX--;
            for (var iY = _minY; iY <= _maxY; iY++){
                AddCell(_minX, iY);
            }            
        }

        public void AddCellsRight(){
            _maxX++;
            for (var iY = _minY; iY <= _maxY; iY++){
                AddCell(_maxX, iY);
            }
        }
    }
}
