using Code.Game.Data;
using Code.Network;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class FollowerHook : MonoBehaviour {
        public byte Id;

        private void Update() {
        }

        void OnMouseDown() {
            Tile.GetParent(gameObject).AssignFollower(Id);
            Destroy(this);
        }

        void OnMouseOver() {
            GetComponent<SpriteRenderer>().color = Net.Color(PlayerSync.PlayerInfo.Color);
        }

        void OnMouseExit() {
            GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }
    }
}