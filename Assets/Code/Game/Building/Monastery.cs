using Code.Game.Data;

namespace Code.Game.Building {
    public class Monastery {
        public PlayerColor Owner { get; set; }

        public Monastery(){}
        public Monastery(PlayerColor owner) { Owner = owner; }
    }
}