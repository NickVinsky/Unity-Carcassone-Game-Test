using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Road : Construction {
        protected int NodesToFinish;
        public bool HasInn { get; set; }

        public Road(int id, Cell v, Location loc) : base(id, v) {
            NodesToFinish = loc.GetNodes().Length;
            Nodes += loc.GetNodes().Length;
            Type = Area.Road;
        }

        public bool NotFinished() { return 2 * Edges != Nodes; }

        private void CalcScore() {
            ScoreCalc.Road(this);
        }

        protected override void CheckForSpecialBuildings(Location location) {
            if (location.HasInn) HasInn = true;
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
            if (NotFinished()) return;
            Finished = true;

            if (!HasOwner()) return;
            CalcScore();
        }

        protected override void Merge(Location construct) {
            base.Merge(construct);
            base.Merge(Builder.GetRoad(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Roads.Remove((Road)construct);
        }

        public void Delete() {
            Builder.Roads.Remove(this);
        }
    }
}