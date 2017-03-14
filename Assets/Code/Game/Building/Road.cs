using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Road : Construction {
        protected byte Plugs;

        public Road(int id, Cell v, FollowerLocation loc) : base(id, v) {
            Plugs = (byte) loc.GetNodes().Length;
            Type = Area.Road;
        }

        private void CalcScore() {
            ScoreCalc.Road(this);
        }

        protected override void AddNodesToFinish(int value) {
            if (value == 1) Plugs--;
        }

        protected override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            AddNodesToFinish(value);
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (Plugs == 0) {
                if (Owners.Count == 0) return;
                CalcScore();
            }
        }

        protected override bool Equals(Area type) { return type == Area.Road;}

        protected override void Merge(FollowerLocation construct) {
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