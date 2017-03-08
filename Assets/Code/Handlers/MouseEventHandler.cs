using Code.Game;
using Code.GameComponents;
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
            if (Tile.Nearby.CanBeAttachedTo(gameObject)) {
                GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                return;
            }
            if (Tile.OnMouse.Exist()) {
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
            if (Tile.Nearby.CanBeAttachedTo(gameObject) && MouseState != State.Dragging) Tile.OnMouse.Put(gameObject);
        }

        // Update is called once per frame
        private void Update () {
        }
    }
}
