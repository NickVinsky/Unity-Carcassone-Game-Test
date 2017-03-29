using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Road : Construction {
        public bool HasInn { get; set; }

        public Road(int id, Cell v, Location loc) : base(id, v) {
            Nodes += loc.Nodes.Length;
            Type = Area.Road;
        }

        public override bool NotFinished => 2 * Edges != Nodes;

        private void CalcScore() {
            ScoreCalc.Road(this);
        }

        protected override void CheckForSpecialBuildings(Location location) {
            if (location.HasInn) HasInn = true;
        }

        public override void CalcNodesToFinish(PlayerColor founder) { FinalNodesCalcToFinish(founder); }

        private void FinalNodesCalcToFinish(PlayerColor founder) {
            if (NotFinished) return;
            PlayerWhoFinished = founder;
            Finished = true;

            if (Builder.BiggestRoadSize < Size) {
                Builder.BiggestRoadSize = Size;
                Builder.BiggestRoadFounder = PlayerWhoFinished;
            }

            if (!HasOwner) return;
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