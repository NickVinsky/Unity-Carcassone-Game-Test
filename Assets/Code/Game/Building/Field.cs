using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Field : Construction {
        public Field(int id, Cell v) : base(id, v) {
            Type = Area.Field;
        }

        protected override bool Equals(Area type) { return type == Area.Field;}

        protected override void Merge(FollowerLocation construct) {
            base.Merge(construct);
            base.Merge(Builder.GetField(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Fields.Remove((Field)construct);
        }
    }
}