using Code.Game;
using Code.Game.Data;
using Code.Network;
using UnityEngine;
using static Code.Game.Data.GameRegulars;
using static Code.Game.Deck;

namespace Code.GUI {
    public class InGameGUI : CoreGUI {
        private readonly GUIElement _deckButton;
        private readonly GUIElement _deckCounter;

        public InGameGUI() {
            _deckButton = new GUIElement {
                Name = DeckButton,
                Action = DeckClick
            };
            _deckCounter = new GUIElement {
                Name = DeckCounter,
                PostInitEvent = EventOnAddOrRemove
            };
/*
            _deckButton = new GUIElement {
                Name = DeckButton,
                Parent = OverlayMiddle,
                Texture = "tileShirt",
                Layer = 5,
                Width = TileSizeX,
                Height = TileSizeY,
                X = Screen.width / 2.0f - TileSizeX / 2.0f - PercWidth(2.0f),
                Y = - Screen.height / 2.0f + TileSizeY / 2.0f + PercHeight(3.3f),
                Action = DeckClick
            };
            _deckCounter = new GUIElement {
                Name = DeckCounter,
                Parent = OverlayMiddle,
                Font = "lb_font",
                FontSize = 52,
                Layer = 5,
                Width = TileSizeX,
                Height = TileSizeY,
                X = _deckButton.X - TileSizeX - 1.0f,
                Y = _deckButton.Y,
                Action = DeckCounterClick,
                PostInitEvent = EventOnAddOrRemove
            };*/
        }

        public void Make() {
            //InitLabel(_deckCounter);
            //InitButton(_deckButton);
            AddListener(_deckButton);
            //AddListener(_deckCounter);
        }

        #region Actions
        private static void DeckClick() {
            if (Net.Game.IsOnline) {
                Net.Game.DeckClick(Camera.main.transform.position, GameObject.Find(DeckButton).GetComponent<RectTransform>().anchoredPosition);
                return;
            }
            if (MainGame.Player.Stage != GameStage.Start) return;
            if (IsEmpty) return;
            Tile.Pick();
            Tile.AttachToMouse();
            MainGame.Player.Stage = GameStage.PlacingTile;
        }
        private static void DeckCounterClick() {

        }
        #endregion
    }
}
