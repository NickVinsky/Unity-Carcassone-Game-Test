using Code.Game.Data;

namespace Code.Game.Building {
    public class Road : Construction {
        public Road(Cell cell, byte[] nodes) : base(cell, nodes) {}
        public Road(Cell cell, PlayerColor owner, byte[] nodes) : base(cell, owner, nodes) {}
    }
}