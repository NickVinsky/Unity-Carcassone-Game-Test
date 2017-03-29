using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using UnityEngine;

namespace Code.Game.Building {
    public class Construction {
        public Area Type { get; protected set; }
        public int ID { get; }
        public List<Ownership> Owners { get; }
        public List<Cell> LinkedTiles { get; }

        public bool Finished { get; set; }
        public bool FinishedByPlayer { get; set; }
        public PlayerColor PlayerWhoFinished { get; set; }

        public int ExtraPoints { get; protected set; }

        protected int Edges { get; set; }
        protected int Nodes { get; set; }

        public bool NeedBarnRecalc { get; set; }

        protected Construction(int id, Cell v) {
            ID = id;
            FinishedByPlayer = false;
            Owners = new List<Ownership>();
            LinkedTiles = new List<Cell> {v};
        }

        protected bool Equals(Area type) => type == Type;

        protected bool HasOwner => Owners.Count > 0;

        public bool HasPlayerMeeples(PlayerColor playerColor) => Owners.Where(owner => owner.Color == playerColor)
                         .Any(owner => owner.FollowerType == Follower.Meeple || owner.FollowerType == Follower.BigMeeple ||
                                       owner.FollowerType == Follower.Mayor || owner.FollowerType == Follower.Wagon);

        public bool HasMeeples => Owners.Any(owner => owner.FollowerType == Follower.Meeple || owner.FollowerType == Follower.BigMeeple);
        public bool HasPigOrBuilder(PlayerColor playerColor) => Owners.Where(owner => owner.Color == playerColor).Any(owner => owner.FollowerType == Follower.Pig || owner.FollowerType == Follower.Builder);
        public bool HasBuilder(PlayerColor playerColor) => Owners.Where(owner => owner.Color == playerColor).Any(owner => owner.FollowerType == Follower.Builder);
        public bool HasBarn => Owners.Any(owner => owner.FollowerType == Follower.Barn);
        public bool HasOnlyBarns => Owners.All(owner => owner.FollowerType == Follower.Barn);

        protected bool HasCell(Cell cell) => Enumerable.Contains(LinkedTiles, cell);
        protected bool HasCells(params Cell[] cell) => cell.Intersect(LinkedTiles).Count() == cell.Length;

        /*protected bool HasCells(params Cell[] cell) {
            var cellNotFound = false;
            foreach (var c in cell) {
                if (HasCell(c)) continue;
                cellNotFound = true;
            }
            return !cellNotFound;
        }*/

        public int Size => LinkedTiles.Count;
        public virtual bool NotFinished => true;

        protected void LinkTile(Cell cell) {
            if (!HasCell(cell)) LinkedTiles.Add(cell);
        }

        public void Add(Location location, PlayerColor founder) {
            var linkedConstruct = location.Link;
            CheckForSpecialBuildings(location);
            if (linkedConstruct == ID) {
            } else if (linkedConstruct == -1) {
                Nodes += location.Nodes.Length;
                location.Link = ID;
                LinkTile(location.Parent.IntVector);
            } else {
                Merge(location);
            }
            Edges++;
            CalcNodesToFinish(founder);


            if (HasOwner) location.ReadyForMeeple = false;
            if (HasBarn) {
                if (Type == Area.Field && !HasOnlyBarns) NeedBarnRecalc = true;
                location.ReadyForMeeple = false;
                location.ReadyForPigOrBuilder = false;
            }
            else {
                if (HasPlayerMeeples(founder)) {
                    if (!HasPigOrBuilder(founder) && NotFinished) location.ReadyForPigOrBuilder = true;
                    if (HasBuilder(founder)) {
                        Net.Client.RequestAdditionalTurn();
                    }
                }
            }

            PostAddingAction(location);
        }

        public void SetOwner(Location construct) {
            Owners.Add(construct.Ownership);
            CalcNodesToFinish(construct.Owner);

            if (construct.Ownership.FollowerType != Follower.Barn) return;
            ScoreCalc.Field(GetAsField);
        }

        protected virtual void PostAddingAction(Location location) {}

        protected virtual void CheckForSpecialBuildings(Location location){}

        public virtual void AddExtraPoints(Location loc, int overPoints){}

        protected virtual void AddNodesToFinish(int value){}

        public virtual void CalcNodesToFinish(PlayerColor founder){}

        protected virtual void CalcNodesToFinish(int value){}

        protected virtual void MergeExtraPoints(int value){}

        protected virtual void Delete(Construction construct) {}

        protected virtual void Merge(Location construct) {
            //base.Merge(construct);
            //base.Merge(Builder.Get<ConstructCollection>(construct), construct);
        }

        protected void Merge(Construction former, Location formerLoc) {
            Edges += former.Edges;
            Nodes += former.Nodes;
            if (former.Type == Area.Field) if (former.GetAsField.Gathered) GetAsField.Gathered = former.GetAsField.Gathered;
            foreach (var tile in former.LinkedTiles) {
                foreach (var fLoc in tile.GetTile.GetLocations) {
                    if (!Equals(fLoc.Type)) continue;
                    if (fLoc.Link != former.ID) continue;

                    CheckForSpecialBuildings(fLoc);

                    fLoc.Link = ID;
                    LinkTile(fLoc.Parent.IntVector);
                    if (fLoc.Owner != PlayerColor.NotPicked) Owners.Add(fLoc.Ownership);
                }
            }

            Delete(former);
        }

        public void Debugger() {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t.FollowerType);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            var log = "[" + Type + "#" + ID + "][" + Nodes + "/" + Edges + "][" + LinkedTiles.Count + "] " + s + "/" + vs;
            Debug.Log(log);
            if (Net.Game.IsOnline) Net.Client.ChatMessage(log);
        }

        public City GetAsCity  => this as City;
        public Road GetAsRoad => this as Road;
        public Field GetAsField => this as Field;
    }
}