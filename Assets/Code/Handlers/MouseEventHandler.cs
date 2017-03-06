using Code.Network;
using UnityEngine;
using static Code.Game;
using static Code.Tiles.TilesHandler;

namespace Code.Handlers {
    public class MouseOnGrid : MonoBehaviour {

        private void OnMouseEnter(){

        }
        private void OnMouseOver() {
            if (Net.Game.IsOnline()) {
                Net.Game.OnMouseOver(gameObject);
                return;
            }
            if (TileCanBeAttachedTo(gameObject)) {
                GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                return;
            }
            if (TileOnMouseExist()) {
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
            if (TileCanBeAttachedTo(gameObject) && MouseState != State.Dragging) PutTileFromMouse(gameObject);
        }

        // Update is called once per frame
        private void Update () {
        }
    }
}
