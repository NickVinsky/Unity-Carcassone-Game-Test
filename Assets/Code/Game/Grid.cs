using Code.Game.Data;
using Code.Handlers;
using UnityEngine;

namespace Code.Game
{
    public class Grid {
        private readonly float _gridX = GameRegulars.TileSizeX / 100;
        private readonly float _gridY = GameRegulars.TileSizeY / 100;

        private readonly int _sizeX;
        private readonly int _sizeY;

        public Vector3 BaseScale;
        public Vector3 EnlargedScale;
        private bool _scaleInited;

        public Grid(int x, int y) {
            _sizeX = x;
            _sizeY = y;
        }
        public Grid() {
            _sizeX = 0;
            _sizeY = 0;
        }

        private int _minX, _maxY, _minY, _maxX;
        
        public void Make(){
            _minX = _sizeX / 2 - _sizeX + _sizeX % 2;
            _maxX = _sizeX / 2;
            _minY = _sizeY / 2 - _sizeY + _sizeY % 2;
            _maxY = _sizeY / 2;
            for (var iX = _minX; iX <= _maxX; iX++) {
                for (var iY = _minY; iY <= _maxY; iY++) {
                    AddCell(new Cell(iX, iY));
                }
            }
        }

        public void Expand(Cell pivot) {
            //Debug.Log("Top" + pivot.Top().XY() + "/Right" + pivot.Right().XY() + "/Bot" + pivot.Bot().XY() + "/left" + pivot.Left().XY());
            Add(pivot.Top());
            Add(pivot.Left());
            Add(pivot.Right());
            Add(pivot.Bot());
        }

        public Cell GetCellCoordinates(GameObject cell) {
            //char[] separatingChars = { '#', ':' };
            //var coordinates = cell.name.Split(separatingChars);
            //var x = Convert.ToInt32(coordinates[1]);
            //var y = Convert.ToInt32(coordinates[2]);
            var x = cell.GetComponent<TileInfo>().X;
            var y = cell.GetComponent<TileInfo>().Y;
            return new Cell(x, y);
        }

        public void CheckBounds(GameObject tileToCheck) {
            var tilePos = GetCellCoordinates(tileToCheck);
            if (tilePos.X == _minX) AddCellsLeft();
            if (tilePos.X == _maxX) AddCellsRight();
            if (tilePos.Y == _minY) AddCellsBot();
            if (tilePos.Y == _maxY) AddCellsTop();
        }

        public void AddCell(Cell v) {
            var tile = new GameObject("cell#" + v.X + ":" + v.Y) {tag = GameRegulars.EmptyCellTag};
            tile.transform.SetParent(GameObject.Find(GameRegulars.GameTable).transform);
            tile.AddComponent<SpriteRenderer>();
            tile.AddComponent<BoxCollider2D>();
            tile.GetComponent<BoxCollider2D>().size = new Vector2(_gridX * 0.9f, _gridY * 0.9f);
            tile.AddComponent<MouseOnGrid>();
            tile.AddComponent<TileInfo>();
            tile.GetComponent<TileInfo>().InitTile(0);
            tile.GetComponent<TileInfo>().X = v.X;
            tile.GetComponent<TileInfo>().Y = v.Y;
            var position = new Vector3(_gridX * v.X, _gridY * v.Y, 0.0f);
            tile.transform.position = position;
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grid");
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            tile.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;

            if (_scaleInited) return;
            BaseScale = tile.transform.localScale;
            EnlargedScale = new Vector3(BaseScale.x * 1.25f, BaseScale.y * 1.25f, BaseScale.z * 1.25f);
            _scaleInited = true;
        }

        private bool Free(Cell v) {
            var cell = GameObject.Find("cell#" + v.X + ":" + v.Y);
            return cell == null;
        }

        private void Add(Cell v) {
            if (Free(v)) AddCell(new Cell(v.X, v.Y));
        }

        public void AddCellsTop() {
            _maxY++;
            for (var iX = _minX; iX <= _maxX; iX++){
                AddCell(new Cell(iX, _maxY));
            }
        }

        public void AddCellsBot(){
            _minY--;
            for (var iX = _minX; iX <= _maxX; iX++){
                AddCell(new Cell(iX, _minY));
            }            
        }

        public void AddCellsLeft(){
            _minX--;
            for (var iY = _minY; iY <= _maxY; iY++){
                AddCell(new Cell(_minX, iY));
            }            
        }

        public void AddCellsRight(){
            _maxX++;
            for (var iY = _minY; iY <= _maxY; iY++){
                AddCell(new Cell(_maxX, iY));
            }
        }
    }
}
