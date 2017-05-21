using UnityEngine;

namespace Code.Handlers {
    public class KeyInputHandler {
        public KeyCode MoveCameraLeft { get; set; }
        public KeyCode MoveCameraRight { get; set; }
        public KeyCode MoveCameraDown { get; set; }
        public KeyCode MoveCameraUp { get; set; }
        public KeyCode ZoomCameraUp { get; set; }
        public KeyCode ZoomCameraDown { get; set; }
        public KeyCode RotateTileClockwise { get; set; }
        public KeyCode RotateTileCounterClockwise { get; set; }
        public KeyCode PickTileFromDeck { get; set; }
        public KeyCode ReturnTileToDeck { get; set; }
        public KeyCode CursorStreaming { get; set; }
        public KeyCode ShowLastPlacedTile { get; set; }
        public KeyCode FocusOnTileOnMouse { get; set; }
        public KeyCode ResetCameraPosition { get; set; }

        public KeyInputHandler() {
            MoveCameraLeft = KeyCode.A;
            MoveCameraRight = KeyCode.D;
            MoveCameraDown = KeyCode.S;
            MoveCameraUp = KeyCode.W;
            ZoomCameraUp = KeyCode.KeypadMinus;
            ZoomCameraDown = KeyCode.KeypadPlus;
            RotateTileClockwise = KeyCode.E;
            RotateTileCounterClockwise = KeyCode.Q;
            PickTileFromDeck = KeyCode.Space;
            ReturnTileToDeck = KeyCode.R;
            CursorStreaming = KeyCode.LeftControl;
            ShowLastPlacedTile = KeyCode.LeftAlt;
            FocusOnTileOnMouse = KeyCode.LeftShift;
            ResetCameraPosition = KeyCode.C;
        }
    }
}
