using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class City : Construction {
        public City(int id, Cell v) : base(id, v) {}

        protected override bool Equals(Area type) { return type == Area.City;}

        protected override void Merge(FollowerLocation construct) { base.Merge(Builder.GetCity(construct)); }

        protected override void Delete(Construction construct) {
            Builder.Cities.Remove((City)construct);
        }
    }
}