﻿using System.Collections.Generic;
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
            //Debug.Log("CELL " + construct.Parent.IntVector().XY());
            var LinkedConstruct = construct.Link;
            if (LinkedConstruct == ID) return;
            if (LinkedConstruct == -1) {
                construct.Link = ID;
                LinkedTile(construct.Parent.IntVector());
            } else {
                Merge(construct);
                Debug.logger.Log(LinkedConstruct + " = > " + ID);
            }
            if (HasOwner()) construct.PosFree = false;
        }

        public void SetOwner(FollowerLocation construct) {
            Owners.Add(construct.GetOwner());
        }

        protected virtual bool Equals(Area type) { return false; }

        protected virtual void Delete(Construction construct) {}

        protected virtual void Merge(FollowerLocation construct) {
            //base.Merge(construct);
            //base.Merge(Builder.Get<ConstructCollection>(construct), construct);
        }

        protected void Merge(Construction former, FollowerLocation formerLoc) {
            foreach (var tile in former.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (!Equals(loc.Type)) continue;
                    if (loc.Link != former.ID) continue;
                    Debug.logger.Log("MERGED " + loc.Link + " = > " + ID);
                    loc.Link = ID;
                    LinkedTile(loc.Parent.IntVector());
                    if (loc.GetOwner() != PlayerColor.NotPicked) Owners.Add(loc.GetOwner());
                }
            }
            Delete(former);
        }

        public void Debugger(Area type) {
            var s = Owners.Aggregate("", (current, t) => current + ", " + t);
            var vs = LinkedTiles.Aggregate(string.Empty, (current, v) => current + "(" + v.X + ";" + v.Y + ")");
            Debug.Log("[" + type + "#" + ID + "][" + LinkedTiles.Count + "] " + s + "/" + vs);
        }
    }
}