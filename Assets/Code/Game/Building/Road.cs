using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Road : Construction {
        public Road(int id, Cell v) : base(id, v) {}

        protected override bool Equals(Area type) { return type == Area.Road;}

        protected override void Merge(FollowerLocation construct) { base.Merge(Builder.GetRoad(construct)); }

        protected override void Delete(Construction construct) {
            Builder.Roads.Remove((Road)construct);
        }
    }
}