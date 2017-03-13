namespace Code.Game.Data {
    public struct Cell {
        public int X, Y;

        public Cell(int x, int y) {
            X = x;
            Y = y;
        }

        public Cell(Cell v) {
            X = v.X;
            Y = v.Y;
        }

        public TileInfo Get() {
            return Tile.Get(X, Y).gameObject.GetComponent<TileInfo>();
        }

        public Cell Top() { return new Cell(X, Y + 1);}
        public Cell Right() { return new Cell(X + 1, Y);}
        public Cell Bot() { return new Cell(X, Y - 1);}
        public Cell Left() { return new Cell(X - 1, Y);}

        public void OffsetTop() { Y += 1; }
        public void OffsetRight() { X += 1; }
        public void OffsetBot() { Y -= 1; }
        public void OffsetLeft() { X -= 1; }

        public string XY() { return "(" + X + ";" + Y + ")"; }
    }
}