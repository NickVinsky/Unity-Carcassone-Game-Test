﻿using System;
using UnityEngine;

namespace Code.Game.TileSubs {
    public class RotateHandler {

        public sbyte Random() {
            return (sbyte) UnityEngine.Random.Range(0, 4);
        }

        public float GetAngle(GameObject tile) {
            var rotates = tile.GetComponent<TileInfo>().Rotates;
            return Convert.ToSingle(rotates) * 90;
        }

        public sbyte Set(sbyte turnsToDo, sbyte rotates) {
            sbyte r = Convert.ToSByte(rotates + turnsToDo);
            while (r > 3) r -= 4;
            while (r < 0) r += 4;
            return r;
        }

        public int Set(int n) {
            int r = n;
            while (r > 3) r -= 4;
            while (r < 0) r += 4;
            return r;
        }

        public void Sprite(int r, GameObject o) {
            var angle = 0f;
            switch (r) {
                case 0:
                    break;
                case 1:
                    angle = -90f;
                    break;
                case 2:
                    angle = -180f;
                    break;
                case 3:
                    angle = 90f;
                    break;
            }
            o.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void Clockwise() {
            Tile.OnMouse.GetSprite().transform.Rotate(Vector3.back * 90);
            Tile.OnMouse.GetTile().Rotates = Set(1, Tile.OnMouse.GetTile().Rotates);
        }

        public void Clockwise(GameObject tile) {
            tile.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 90);
            tile.GetComponent<TileInfo>().Rotates = Set(1, tile.GetComponent<TileInfo>().Rotates);
        }

        public void CounterClockwise(){
            Tile.OnMouse.GetSprite().transform.Rotate(Vector3.back * -90);
            Tile.OnMouse.GetTile().Rotates = Set(-1, Tile.OnMouse.GetTile().Rotates);
        }

        public void CounterClockwise(GameObject tile){
            tile.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * -90);
            tile.GetComponent<TileInfo>().Rotates = Set(-1, tile.GetComponent<TileInfo>().Rotates);
        }
    }
}