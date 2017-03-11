using Code.Game.Data;

namespace Code.Game.Building {
    public class City : Construction {
        public City(Cell cell, byte[] nodes) : base(cell, nodes) {}
        public City(Cell cell, PlayerColor owner, byte[] nodes) : base(cell, owner, nodes) {}
    }
}