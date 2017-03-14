using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class City : Construction {
        protected int NodesToFinish;

        public City(int id, Cell v, FollowerLocation loc) : base(id, v) {
            NodesToFinish = loc.GetNodes().Length;
            Type = Area.City;
        }

        private void CalcScore() {
            ScoreCalc.City(this);
        }

        protected override void AddNodesToFinish(int value) {
            NodesToFinish -= 2;
            NodesToFinish += value;
            //Debug.Log("City#" + ID + " AddNodesToFinish => NodesToFinish = " + NodesToFinish);
        }

        public override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            AddNodesToFinish(value);
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (NodesToFinish == 0) {
                if (!HasOwner()) return;
                CalcScore();
            }
            //Debug.Log("City#" + ID + " CalcNodesToFinish => NodesToFinish = " + NodesToFinish);
        }

        public override void AddExtraPoints(FollowerLocation loc) {
            if (loc.CoatOfArms) ExtraPoints += 2;
        }

        protected override void MergeExtraPoints(int value) { ExtraPoints += value; }

        protected override void Merge(FollowerLocation construct) {
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