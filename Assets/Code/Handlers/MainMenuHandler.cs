using Code.GUI;
using UnityEngine;

namespace Code.Handlers {
    public class MainMenuHandler : MonoBehaviour {
        readonly MainMenuGUI _inGameGUI = new MainMenuGUI();

        void Start () {
            _inGameGUI.Make();
        }

        void Update() {

        }
    }
}
