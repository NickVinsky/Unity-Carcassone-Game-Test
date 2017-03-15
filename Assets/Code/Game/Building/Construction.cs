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
        public List<PlayerColor> Owners { get; }
        public List<Cell> LinkedTiles { get; }
        public bool FinishedByPlayer { get; set; }
        public int ExtraPoints { get; protected set; }

        protected int Edges { get; set; }
        protected int Nodes { get; set; }

        protected Construction(int id, Cell v) {
            ID = id;
            FinishedByPlayer = false;
            Owners = new List<PlayerColor>();
            LinkedTiles = new List<Cell> {v};
        }

        protected bool HasOwner() { return Owners.Count > 0; }

        protected bool HasCell(Cell cell) { return Enumerable.Contains(LinkedTiles, cell); }

        protected void LinkTile(Cell cell) {
            if (!HasCell(cell)) LinkedTiles.Add(cell);
        }

        public void Add(FollowerLocation construct) {
            var linkedConstruct = construct.Link;
            if (linkedConstruct == ID) {
                Edges++;
                Nodes += construct.GetNodes().Length;
                //AddNodesToFinish(0);
            } else if (linkedConstruct == -1) {
                Edges++;
                Nodes += construct.GetNodes().Length;
                construct.Link = ID;
                //AddExtraPoints(construct);
                LinkTile(construct.Parent.IntVector());
                //CalcNodesToFinish(construct.GetNodes().Length);
                CalcNodesToFinish();
            } else {
                Merge(construct);
            }
            //Debug.Log(Type + " Nodes = " + Nodes + ", Edges = " + Edges);
            if (HasOwner()) construct.PosFree = false;
        }

        public void SetOwner(FollowerLocation construct) {
            Owners.Add(construct.GetOwner());
            CalcNodesToFinish();
        }

        public virtual void AddExtraPoints(FollowerLocation loc){}

        protected virtual void AddNodesToFinish(int value){}

        public virtual void CalcNodesToFinish(){}

        protected virtual void CalcNodesToFinish(int value){}

        protected virtual void MergeExtraPoints(int value){}

        protected bool Equals(Area type) { return type == Type;}

        protected virtual void Delete(Construction construct) {}

        protected virtual void Merge(FollowerLocation construct) {
            //base.Merge(construct);
            //base.Merge(Builder.Get<ConstructCollection>(construct), construct);
        }

        protected void Merge(Construction former, FollowerLocation formerLoc) {
            foreach (var tile in former.LinkedTiles) {
                foreach (var fLoc in tile.Get().GetLocations()) {
                    if (!Equals(fLoc.Type)) continue;
                    if (fLoc.Link != former.ID) continue;
                    //Debug.logger.Log("MERGED " + loc.Link + " = > " + ID);
                    fLoc.Link = ID;
                    //AddExtraPoints(loc);
                    LinkTile(fLoc.Parent.IntVector());
                    if (fLoc.GetOwner() != PlayerColor.NotPicked) Owners.Add(fLoc.GetOwner());
                    AddNodesToFinish(fLoc.GetNodes().Length);
                    Debug.Log("Merging");
                    Edges++;
                    Nodes += fLoc.GetNodes().Length;
                }
            }
            Debug.Log("ExtraPoints " + former.ExtraPoints);
            //MergeExtraPoints(former.ExtraPoints);
            CalcNodesToFinish();
            Delete(former);
        }

        public void Debugger() {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            var log = "[" + Type + "#" + ID + "][" + LinkedTiles.Count + "] " + s + "/" + vs;
            Debug.Log(log);
            if (Net.Game.IsOnline()) Net.Client.ChatMessage(log);
        }
    }
}