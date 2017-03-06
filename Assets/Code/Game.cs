﻿using System;
using Code.Handlers;
using Code.Network;
using Code.Tiles;
using UnityEngine;
using UnityEngine.UI;
using static Code.Tiles.TilesHandler;
using static Code.GameRegulars;

namespace Code {
    public class Game {

        // Use this for initialization
        private readonly KeyInputHandler _k = new KeyInputHandler();
        public static readonly Grid Grid = new Grid();

        //private float _timer;
        //private bool _timerTrigger;
        //public static bool TilePermit;

        private static float _cameraDistance = 5f;
        private static Vector3 _mousePosition;
        public enum State
        {
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

        public void Init() {
            if (!Net.Game.IsOnline()) {
                GameObject.Find("ChatPanel").transform.localScale = new Vector2(0f, 0f);
                GameObject.Find("PlayersPanel").transform.localScale = new Vector2(0f, 0f);
            }
            Grid.Make();
            DeckHandler.InitVanillaDeck();
            SetStartTile(20);
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

            #region Camera_Movement

            if (!MouseOnChat) {
                _cameraDistance -= Input.mouseScrollDelta.y * ScrollSpeed;
                _cameraDistance = Mathf.Clamp(_cameraDistance, CameraDistanceMin, CameraDistanceMax);
                Camera.main.orthographicSize = _cameraDistance;
            }

            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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


            if (Net.Game.IsOnline()) {
                Net.Game.LocalClientUpadate(_k);
                return;
            }
            #region Game_Logic_Local
            if (TileOnMouseExist()) AttachTileToMouse();
            if (LobbyInspector.ChatField.GetComponent<InputField>().isFocused) return;

            if ((Input.GetKeyDown(_k.RotateTileClockwise) || Input.GetMouseButtonDown(1)) && TileOnMouseExist()) RotateClockwise();
            if (Input.GetKeyDown(_k.RotateTileCounterClockwise) && TileOnMouseExist()) RotateCounterClockwise();
            if (Input.GetKeyDown(_k.PickTileFromDeck)) {
                if (!TileOnMouseExist() && !DeckHandler.DeckIsEmpty()) {
                    PickTileFromDeck();
                    AttachTileToMouse();
                }
                if (TileOnMouseExist()) RotateClockwise();
            }
            if (Input.GetKeyDown(_k.ReturnTileToDeck)) {
                if (TileOnMouseExist()) {
                    ReturnTileToDeck();
                }
            }
            #endregion
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

            if (Math.Abs(dragDelta.x) < 0.00001f && Math.Abs(dragDelta.y) < 0.00001f) return;

            MouseState = State.Dragging;
            Camera.main.transform.Translate(-dragDelta);
        }

        private void FinishDrag() {
            MouseState = State.None;
        }
        #endregion
    }
}
