using Code.GUI;
using UnityEngine;

namespace Code.Handlers {
    public class CameraHandler : MonoBehaviour {
        private readonly MainGame _mainGame = new MainGame();
        private readonly InGameGUI _inGameGui = new InGameGUI();

        void Start () {
            _mainGame.Init();
            _inGameGui.Make();
        }

        void Update () {
            _mainGame.Update();
        }
    }
}
