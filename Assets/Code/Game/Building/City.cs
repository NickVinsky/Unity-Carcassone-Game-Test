using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class City : Construction {
        protected int NodesToFinish;

        public City(int id, Cell v, Location loc) : base(id, v) {
            NodesToFinish = loc.GetNodes().Length;
            Nodes += loc.GetNodes().Length;
            Type = Area.City;
        }

        public bool Finished() { return NodesToFinish == 0; }

        private void CalcScore() {
            ScoreCalc.City(this);
        }

        protected override void AddNodesToFinish(int value) {
            NodesToFinish -= 2;
            NodesToFinish += value;
        }

        public override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            AddNodesToFinish(value);
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (2 * Edges != Nodes) return;
            if (!HasOwner()) return;
            CalcScore();
        }

        public override void AddExtraPoints(Location loc) {
            if (loc.CoatOfArms) ExtraPoints += 2;
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