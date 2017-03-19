using Code.Game.Data;
using Code.Network;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class FollowerHook : MonoBehaviour {
        public byte Id;
        public Follower Type;

        private void Update() {
        }

        void OnMouseDown() {
            Tile.GetParent(gameObject).AssignFollower(Id, Type);
            Destroy(this);
        }

        void OnMouseOver() {
            GetComponent<SpriteRenderer>().color = Net.Color(MainGame.Player.Color);
        }

        void OnMouseExit() {
            GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }
    }
}