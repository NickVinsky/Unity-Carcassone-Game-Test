using Code.GUI;
using UnityEngine;

namespace Code.Handlers {
    public class CameraHandler : MonoBehaviour {
        readonly Game _game = new Game();
        readonly InGameGUI _inGameGui = new InGameGUI();

        void Start () {
            _game.Init();
            _inGameGui.Make();
        }

        void Update () {
            _game.Update();
        }
    }
}
