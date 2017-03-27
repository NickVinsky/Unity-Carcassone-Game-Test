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

        protected bool HasOwner() { return Owners.Count > 0; }

        public bool HasPlayerMeeples(PlayerColor playerColor) {
            return Owners.Where(owner => owner.Color == playerColor)
                         .Any(owner => owner.FollowerType == Follower.Meeple || owner.FollowerType == Follower.BigMeeple ||
                                       owner.FollowerType == Follower.Mayor || owner.FollowerType == Follower.Wagon);
        }

        public bool HasMeeples() { return Owners.Any(owner => owner.FollowerType == Follower.Meeple || owner.FollowerType == Follower.BigMeeple); }

        public bool HasPigOrBuilder(PlayerColor playerColor) {
            return Owners.Where(owner => owner.Color == playerColor).Any(owner => owner.FollowerType == Follower.Pig || owner.FollowerType == Follower.Builder);
        }

        public bool HasBuilder(PlayerColor playerColor) {
            return Owners.Where(owner => owner.Color == playerColor).Any(owner => owner.FollowerType == Follower.Builder);
        }

        public bool HasBarn() { return Owners.Any(owner => owner.FollowerType == Follower.Barn); }

        public bool HasOnlyBarns() { return Owners.All(owner => owner.FollowerType == Follower.Barn); }

        protected bool HasCell(Cell cell) { return Enumerable.Contains(LinkedTiles, cell); }

        protected bool HasCells(params Cell[] cell) {
            var cellNotFound = false;
            foreach (var c in cell) {
                if (HasCell(c)) continue;
                cellNotFound = true;
            }
            return !cellNotFound;
        }

        public virtual bool NotFinished() { return true; }

        protected void LinkTile(Cell cell) {
            if (!HasCell(cell)) LinkedTiles.Add(cell);
        }

        public void Add(Location location, PlayerColor founder) {
            var linkedConstruct = location.Link;
            CheckForSpecialBuildings(location);
            if (linkedConstruct == ID) {
            } else if (linkedConstruct == -1) {
                Nodes += location.GetNodes().Length;
                location.Link = ID;
                LinkTile(location.Parent.IntVector());
            } else {
                Merge(location);
            }
            Edges++;
            CalcNodesToFinish();


            if (HasOwner()) location.ReadyForMeeple = false;
            if (HasBarn()) {
                if (Type == Area.Field && !HasOnlyBarns()) NeedBarnRecalc = true;
                location.ReadyForMeeple = false;
                location.ReadyForPigOrBuilder = false;
            }
            else {
                if (HasPlayerMeeples(founder)) {
                    if (!HasPigOrBuilder(founder) && NotFinished()) location.ReadyForPigOrBuilder = true;
                    if (HasBuilder(founder)) {
                        Net.Client.RequestAdditionalTurn();
                    }
                }
            }

            PostAddingAction(location);
        }

        public void SetOwner(Location construct) {
            Owners.Add(construct.GetOwnership());
            CalcNodesToFinish();

            if (construct.GetOwnership().FollowerType != Follower.Barn) return;
            var field = (Field) this;
            ScoreCalc.Field(field);
        }

        protected virtual void PostAddingAction(Location location) {}

        protected virtual void CheckForSpecialBuildings(Location location){}

        public virtual void AddExtraPoints(Location loc, int overPoints){}

        protected virtual void AddNodesToFinish(int value){}

        public virtual void CalcNodesToFinish(){}

        protected virtual void CalcNodesToFinish(int value){}

        protected virtual void MergeExtraPoints(int value){}

        protected bool Equals(Area type) { return type == Type;}

        protected virtual void Delete(Construction construct) {}

        protected virtual void Merge(Location construct) {
            //base.Merge(construct);
            //base.Merge(Builder.Get<ConstructCollection>(construct), construct);
        }

        protected void Merge(Construction former, Location formerLoc) {
            //var barnDetected = false;
            Edges += former.Edges;
            Nodes += former.Nodes;
            if (former.Type == Area.Field) {
                var field = (Field) this;
                var formerField = (Field) former;
                if (formerField.Gathered) field.Gathered = formerField.Gathered;
            }
            foreach (var tile in former.LinkedTiles) {
                foreach (var fLoc in tile.Get().GetLocations()) {
                    if (!Equals(fLoc.Type)) continue;
                    if (fLoc.Link != former.ID) continue;
                    //if (fLoc.GetOwnership().FollowerType == Follower.Barn) barnDetected = true;

                    CheckForSpecialBuildings(fLoc);

                    fLoc.Link = ID;
                    LinkTile(fLoc.Parent.IntVector());
                    if (fLoc.GetOwner() != PlayerColor.NotPicked) Owners.Add(fLoc.GetOwnership());
                }
            }
            /*if (barnDetected) {
                var field = (Field) this;
                field.Gathered = true;
                Debug.Log("ReGathering Field...");
                ScoreCalc.Field(field);
            }*/
            Delete(former);
        }

        public void Debugger() {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t.FollowerType);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            var log = "[" + Type + "#" + ID + "][" + Nodes + "/" + Edges + "][" + LinkedTiles.Count + "] " + s + "/" + vs;
            Debug.Log(log);
            if (Net.Game.IsOnline()) Net.Client.ChatMessage(log);
        }

        public City GetAsCity() { return this as City; }
        public Road GetAsRoad() { return this as Road; }
        public Field GetAsField() { return this as Field; }
    }
}