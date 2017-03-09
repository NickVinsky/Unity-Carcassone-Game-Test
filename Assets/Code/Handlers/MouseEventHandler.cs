using Code.Game;
using Code.Network;
using UnityEngine;
using static Code.MainGame;

namespace Code.Handlers {
    public class MouseOnGrid : MonoBehaviour {

        private void OnMouseEnter(){

        }
        private void OnMouseOver() {
            if (Net.Game.IsOnline()) {
                Net.Game.OnMouseOver(gameObject);
                return;
            }
            if (Stage == GameStage.PlacingTile) {
                if (Tile.Nearby.CanBeAttachedTo(gameObject)) {
                    GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                    return;
                }
                GetComponent<SpriteRenderer>().color = GameRegulars.CantAttachlColor;
                return;
            }
            GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }
        private void OnMouseExit() {
            if (Net.Game.IsOnline()) {
                Net.Game.OnMouseExit(gameObject);
                return;
            }
            GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
        }

        private void OnMouseDown() {
        }

        private void OnMouseUp() {
            if (Net.Game.IsOnline()) {
                Net.Game.OnMouseUp(gameObject);
                return;
            }
            if (Stage != GameStage.PlacingTile) return;
            if (!Tile.Nearby.CanBeAttachedTo(gameObject) || MouseState == State.Dragging) return;
            Tile.OnMouse.Put(gameObject);
            Stage = GameStage.PlacingFollower;
        }

        // Update is called once per frame
        private void Update () {
        }
    }
}
