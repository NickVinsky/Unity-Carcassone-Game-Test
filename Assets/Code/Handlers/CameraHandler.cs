using Code.GUI;
using UnityEngine;

namespace Code.Handlers {
    public class CameraHandler : MonoBehaviour {
        private readonly MainGame _mainGame = new MainGame();
        private readonly InGameGUI _inGameGui = new InGameGUI();

        void Start () {
            //foreach (var go in GameObject.FindGameObjectsWithTag("EditorOnly")) Destroy(go);
//            Time.timeScale = 1;
            _mainGame.Init();
            _inGameGui.Make();
        }

        void Update () {
            _mainGame.Update();
        }

        private void FixedUpdate() {
            _mainGame.FixedUpdate();
        }
    }
}
