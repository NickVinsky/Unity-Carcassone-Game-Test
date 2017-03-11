namespace Code.Game.Data {
    public struct Cell {
        public int X, Y;

        public Cell(int x, int y) {
            X = x;
            Y = y;
        }

        public TileInfo Get() {
            return Tile.Get(X, Y).gameObject.GetComponent<TileInfo>();
        }
    }
}