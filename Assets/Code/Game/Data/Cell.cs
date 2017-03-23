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

        public Cell(Cell v, int x, int y) {
            X = v.X + x;
            Y = v.Y + y;
        }

        public TileInfo Get() {
            return Tile.Get(this).gameObject.GetComponent<TileInfo>();
        }

        public Cell Top() { return new Cell(X, Y + 1);}
        public Cell Right() { return new Cell(X + 1, Y);}
        public Cell Bot() { return new Cell(X, Y - 1);}
        public Cell Left() { return new Cell(X - 1, Y);}

        public Cell CornerLeftBot() { return new Cell(X - 1, Y - 1);}
        public Cell CornerRightBot() { return new Cell(X + 1, Y - 1);}
        public Cell CornerLeftTop() { return new Cell(X - 1, Y + 1);}
        public Cell CornerRightTop() { return new Cell(X + 1, Y + 1);}

        public void OffsetTop() { Y += 1; }
        public void OffsetRight() { X += 1; }
        public void OffsetBot() { Y -= 1; }
        public void OffsetLeft() { X -= 1; }

        public Cell Offset(int x, int y) { return new Cell(X + x, Y + y);}

        public string XY() { return "(" + X + ";" + Y + ")"; }
    }
}