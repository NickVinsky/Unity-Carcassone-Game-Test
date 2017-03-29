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

        public TileInfo GetTile => Tile.Get(this).gameObject.GetComponent<TileInfo>();

        public Cell Top => new Cell(X, Y + 1);
        public Cell Right => new Cell(X + 1, Y);
        public Cell Bot => new Cell(X, Y - 1);
        public Cell Left => new Cell(X - 1, Y);

        public Cell CornerLeftBot => new Cell(X - 1, Y - 1);
        public Cell CornerRightBot => new Cell(X + 1, Y - 1);
        public Cell CornerLeftTop => new Cell(X - 1, Y + 1);
        public Cell CornerRightTop => new Cell(X + 1, Y + 1);

        public void OffsetTop() { Y += 1; }
        public void OffsetRight() { X += 1; }
        public void OffsetBot() { Y -= 1; }
        public void OffsetLeft() { X -= 1; }

        public Cell Offset(int x, int y) => new Cell(X + x, Y + y);

        public string XY => "(" + X + ";" + Y + ")";
    }
}