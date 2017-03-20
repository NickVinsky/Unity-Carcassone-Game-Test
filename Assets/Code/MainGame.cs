using System;
using Code.Game;
using Code.Game.Building;
using Code.Game.Data;
using Code.GUI;
using Code.Handlers;
using Code.Network;
using UnityEngine;
using UnityEngine.UI;
using static Code.Game.Data.GameRegulars;

namespace Code {
    public class MainGame {

        // Use this for initialization
        private readonly KeyInputHandler _k = new KeyInputHandler();
        public static readonly Grid Grid = new Grid();

        public static PlayerInfo Player = new PlayerInfo();

        //public static GameStage Stage;

        //private float _timer;
        //private bool _timerTrigger;
        //public static bool TilePermit;

        private static float _cameraDistance = 5f;
        private const float DragDelta = 0.0000001f; // default = 0.00001f
        //private static Vector3 _mousePosition;
        public enum State {
            None,
            PreDragging,
            Dragging
        }
        public static State MouseState;
        private Vector3 _dragStartPos;

        private static void MoveCamera(bool x, bool y, int acc) {
            Vector3 position = Camera.main.transform.position;
            if (x) position.x += acc * CamMoveSpeed;
            if (y) position.y += acc * CamMoveSpeed;
            Camera.main.transform.position = position;
        }

        public static bool MouseOnChat;

        public static void ChangeGameStage(GameStage stage) {
            Player.Stage = stage;
        }

        public void Init() {
            Grid.Make();
            Deck.InitVanillaDeck();
            Tile.SetStarting(20);
            Builder.Init();

            if (Net.Game.IsOnline()) {
                Net.Game.Init();
                return;
            }

            Player.Stage = GameStage.Start;
            if (Player.PlayerName == null)
                Player.PlayerName = MainMenuGUI.GetRandomName();
            Player.Color = (PlayerColor) UnityEngine.Random.Range(1, Net.ColorsCount);
            GameObject.Find("ChatPanel").transform.localScale = new Vector2(0f, 0f);
            //GameObject.Find("PlayersPanel").transform.localScale = new Vector2(0f, 0f);
            UpdateLocalPlayer();
        }

        public static void UpdateLocalPlayer() {
            var isMoving = "1";
            var l = (int) Player.Color + isMoving + Player.MeeplesQuantity.ToString("D1") +
                    Player.Score.ToString("D4") + Player.PlayerName;
            Net.Client.RefreshInGamePlayersList(l);
        }

        // Update is called once per frame
        public void Update() {
            #region KeyDetector

            /*
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                    Debug.Log("KeyCode down: " + kcode);
            }*/

            #endregion

            #region CameraMovement

            if (!MouseOnChat) {
                _cameraDistance -= Input.mouseScrollDelta.y * ScrollSpeed;
                _cameraDistance = Mathf.Clamp(_cameraDistance, CameraDistanceMin, CameraDistanceMax);
                Camera.main.orthographicSize = _cameraDistance;
            }

            //_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKey(KeyCode.Escape)) Application.Quit();

            if (!LobbyInspector.ChatField.GetComponent<InputField>().isFocused) {
                if (Input.GetKey(_k.MoveCameraUp)) MoveCamera(false, true, 1);
                if (Input.GetKey(_k.MoveCameraDown)) MoveCamera(false, true, -1);
                if (Input.GetKey(_k.MoveCameraRight)) MoveCamera(true, false, 1);
                if (Input.GetKey(_k.MoveCameraLeft)) MoveCamera(true, false, -1);
            }

            if (MouseState == State.None && Input.GetMouseButtonDown(0) && !MouseOnChat) InitDrag();
            if ((MouseState == State.PreDragging || MouseState == State.Dragging) && Input.GetMouseButton(0)) MoveCamera();
            if ((MouseState == State.PreDragging || MouseState == State.Dragging) && Input.GetMouseButtonUp(0)) FinishDrag();
            #endregion

            #region NetGame
            if (Net.Game.IsOnline()) {
                Net.Game.LocalClientUpadate(_k);
                return;
            }
            #endregion

            //if (Input.GetKeyDown(KeyCode.F)) ScoreCalc.Final();
            //if (Input.GetKeyDown(KeyCode.L)) Tile.LastPlaced().HideAll();
            //if (Input.GetKeyDown(KeyCode.K)) Tile.LastPlaced().ShowPossibleMeeplesLocations();

            #region Game_Logic_Local
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;
            switch (Player.Stage) {
                case GameStage.Wait:
                    break;
                case GameStage.Start:
                    if (Input.GetKeyDown(_k.PickTileFromDeck)) {
                        if (!Deck.IsEmpty()) {
                            Tile.Pick();
                            Tile.AttachToMouse();
                            Player.Stage = GameStage.PlacingTile;
                        }
                    }
                    break;
                case GameStage.PlacingTile:
                    Tile.AttachToMouse();
                    if (Input.GetKeyDown(_k.RotateTileClockwise) || Input.GetMouseButtonDown(1))
                        Tile.Rotate.Clockwise();
                    if (Input.GetKeyDown(_k.RotateTileCounterClockwise)) Tile.Rotate.CounterClockwise();
                    if (Input.GetKeyDown(_k.PickTileFromDeck)) Tile.Rotate.Clockwise();
                    if (Input.GetKeyDown(_k.ReturnTileToDeck)) {
                        Tile.Return();
                        Player.Stage = GameStage.Start;
                    }
                    //Stage = GameStage.PlacingFollower; // In MouseEventHandler.OnMouseUp()
                    break;
                case GameStage.PlacingFollower:
                    if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(_k.ReturnTileToDeck)) {
                        Tile.LastPlaced().HideAll();
                        Player.Stage = GameStage.Finish;
                    }
                    break;
                case GameStage.Finish:
                    UpdateLocalPlayer();
                    if (Deck.IsEmpty()) {
                        Player.Stage = GameStage.End;
                        ScoreCalc.Final();
                    }
                    Player.Stage = GameStage.Start;
                    break;
                case GameStage.End:
                    break;
                default:
                    //throw new ArgumentOutOfRangeException();
                    Debug.Log("ArgumentOutOfRangeException");
                    break;
            }

            #endregion
        }

        public void FixedUpdate() {
            if (Net.Game.IsOnline()) {
                Net.Game.FixedUpdate(_k);
            }
        }

        #region Click&Drag
        private void InitDrag() {
            //_dragStartPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            _dragStartPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            MouseState = State.PreDragging;
        }

        private void MoveCamera() {
            //Vector3 actualPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            Vector3 actualPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector3 dragDelta = actualPos - _dragStartPos;

            if (Math.Abs(dragDelta.x) < DragDelta && Math.Abs(dragDelta.y) < 0.00001f) return;

            MouseState = State.Dragging;
            Camera.main.transform.Translate(-dragDelta);
        }

        private void FinishDrag() {
            MouseState = State.None;
        }
        #endregion
    }
}
