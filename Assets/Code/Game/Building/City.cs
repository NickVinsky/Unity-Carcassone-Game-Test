using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class City : Construction {
        public byte CoatOfArmsQuantity { get; private set; }
        public bool HasCathedral { get; private set; }

        public City(int id, Cell v, Location loc) : base(id, v) {
            Nodes += loc.GetNodes().Length;
            Type = Area.City;
        }

        //public bool Finished() { return NodesToFinish == 0; }
        public override bool NotFinished() { return 2 * Edges != Nodes; }

        private void CalcScore() {
            ScoreCalc.City(this);
        }

        public override void AddExtraPoints(Location loc, int overPoints) {
            ExtraPoints += overPoints;

            if (!loc.CoatOfArms) return;
            CoatOfArmsQuantity++;
            ExtraPoints += 2;
        }

        protected override void CheckForSpecialBuildings(Location location) {
            if (location.HasCathedral) HasCathedral = true;
        }

        public override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (NotFinished()) return;
            Finished = true;

            if (!HasOwner()) return;
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