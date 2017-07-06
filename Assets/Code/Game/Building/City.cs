using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class City : Construction {
        public byte CoatOfArmsQuantity { get; private set; }
        public bool HasCathedral { get; private set; }

        public City(int id, Cell v, Location loc) : base(id, v) {
            Nodes += loc.Nodes.Length;
            Type = Area.City;
            if (loc.HasCathedral) HasCathedral = true;
        }

        public override bool NotFinished => 2 * Edges != Nodes;

        private void CalcScore() {
            ScoreCalc.City(this);
        }

        public override void AddExtraPoints(Location loc, int overPoints) {
            if (!loc.CoatOfArms) return;
            CoatOfArmsQuantity++;
            ExtraPoints += 2 + overPoints;
        }

        protected override void CheckForSpecialBuildings(Location location) {
            if (location.HasCathedral) HasCathedral = true;
        }

        public override void CalcNodesToFinish(PlayerColor founder) { FinalNodesCalcToFinish(founder); }

        private void FinalNodesCalcToFinish(PlayerColor founder) {
            if (NotFinished) return;
            PlayerWhoFinished = founder;
            Finished = true;

            if (Builder.BiggestCitySize < Size) {
                Builder.BiggestCitySize = Size;
                Builder.BiggestCityFounder = PlayerWhoFinished;
            }

            if (!HasOwner) return;
            CalcScore();
        }

        protected override void MergeExtraPoints(int value) { ExtraPoints += value; }

        protected override void Merge(Location construct) {
            base.Merge(construct);
            base.Merge(Builder.GetCity(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Cities.Remove((City)construct);
        }

        public void Delete() {
            Builder.Cities.Remove(this);
        }
    }
}