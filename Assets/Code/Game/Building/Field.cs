using Code.Game.Data;

namespace Code.Game.Building {
    public class Field : Construction {
        public Field(Cell cell, byte[] nodes) : base(cell, nodes) {}
        public Field(Cell cell, PlayerColor owner, byte[] nodes) : base(cell, owner, nodes) {}

        protected override void Init() {
            Top = new byte[] {0, 1};
            Right = new byte[] {2, 3};
            Bot = new byte[] {4, 5};
            Left = new byte[] {6, 7};
        }
    }
}