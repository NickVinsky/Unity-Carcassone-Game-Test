using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using UnityEngine;

namespace Code.Game.Building {
    public class Construction {
        public int ID { get; }
        protected int TilesMerged;
        protected List<PlayerColor> Owners;
        protected bool Finished { get; }
        protected List<Cell> LinkedTiles;

        protected Construction(int id, Cell v) {
            ID = id;
            Finished = false;
            Owners = new List<PlayerColor>();
            LinkedTiles = new List<Cell>();
            LinkedTiles.Add(v);
            TilesMerged = 1;
        }

        protected bool HasOwner() { return Owners.Count > 0; }

        protected bool HasCell(Cell cell) { return Enumerable.Contains(LinkedTiles, cell); }

        protected void LinkedTile(Cell cell) {
            if (!HasCell(cell)) LinkedTiles.Add(cell);
        }

        public void Add(FollowerLocation construct) {
            Debug.Log("CELL " + construct.Parent.IntVector().XY());
            var LinkedConstruct = construct.Link;
            if (LinkedConstruct == ID) return;
            if (LinkedConstruct == -1) {
                construct.Link = ID;
                LinkedTile(construct.Parent.IntVector());
            } else {
                Merge(construct);
            }
            if (HasOwner()) construct.PosFree = false;
        }

        public void SetOwner(FollowerLocation construct) {
            Owners.Add(construct.GetOwner());
        }

        protected virtual bool Equals(Area type) { return false; }

        protected virtual void Delete(Construction construct) {}
        protected virtual void Merge(FollowerLocation construct) {}
        protected void Merge(Construction child) {
            foreach (var tile in GameObject.FindGameObjectsWithTag(GameRegulars.TileTag)) {
                foreach (var loc in Tile.Get(tile).GetLocations()) {
                    if (!Equals(loc.Type)) continue;
                    if (loc.Link != child.ID) continue;
                    loc.Link = ID;
                    if (loc.GetOwner() != PlayerColor.NotPicked) Owners.Add(loc.GetOwner());
                }
            }
            Delete(child);
        }

        public void Debugger(Area type) {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            Debug.Log("[" + type + "#" + ID + "][" + LinkedTiles.Count + "] " + s + "/" + vs);
        }
    }
}