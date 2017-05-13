using Code.GUI;
using UnityEngine;

namespace Code.Handlers {
    public class MainMenuHandler : MonoBehaviour {
        readonly MainMenuGUI _inGameGUI = new MainMenuGUI();

        void Start ()
        {
//            Time.timeScale = 1;
            _inGameGUI.Make();
        }

        void Update() {

        }
    }
}
