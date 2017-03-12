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

        protected Construction(int id) {
            ID = id;
            Finished = false;
            Owners = new List<PlayerColor>();
            TilesMerged = 1;
        }

        protected bool HasOwner() { return Owners.Count > 0; }

        public void Add(FollowerLocation construct) {
            var LinkedConstruct = construct.Link;
            if (LinkedConstruct == ID) return;
            if (LinkedConstruct == -1) {
                TilesMerged++;
                construct.Link = ID;
            } else {
                Merge(construct);
            }
            if (HasOwner()) construct.PosFree = false;
        }

        public void Correction(int correction) {
            TilesMerged -= correction;
            TilesMerged++;
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
                    TilesMerged++;
                }
            }
            Delete(child);
        }

        public void Debugger(Area type) {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t);
            Debug.Log("[" + type + "#" + ID + "][" + TilesMerged + "] " + s);
        }
    }
}