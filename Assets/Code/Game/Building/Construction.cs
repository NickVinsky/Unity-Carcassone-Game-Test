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
        public bool Finished { get; set; }
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

        public void Add(Location location) {
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
            if (HasOwner()) location.FreeMeeplePos = false;
        }

        public void SetOwner(Location construct) {
            Owners.Add(construct.GetOwner());
            CalcNodesToFinish();
        }

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
            Edges += former.Edges;
            Nodes += former.Nodes;
            foreach (var tile in former.LinkedTiles) {
                foreach (var fLoc in tile.Get().GetLocations()) {
                    if (!Equals(fLoc.Type)) continue;
                    if (fLoc.Link != former.ID) continue;
                    //Nodes += fLoc.GetNodes().Length;
                    CheckForSpecialBuildings(fLoc);
                    fLoc.Link = ID;
                    LinkTile(fLoc.Parent.IntVector());
                    if (fLoc.GetOwner() != PlayerColor.NotPicked) Owners.Add(fLoc.GetOwner());
                }
            }
            Delete(former);
        }

        public void Debugger() {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            var log = "[" + Type + "#" + ID + "][" + Nodes + "/" + Edges + "][" + LinkedTiles.Count + "] " + s + "/" + vs;
            Debug.Log(log);
            if (Net.Game.IsOnline()) Net.Client.ChatMessage(log);
        }
    }
}