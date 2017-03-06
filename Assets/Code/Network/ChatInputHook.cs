using UnityEngine;

namespace Code.Network {
    public class ChatInputHook : MonoBehaviour {

        // Use this for initialization
        void Start () {
            LobbyInspector.Init();
        }
	
        // Update is called once per frame
        void Update () {
            LobbyInspector.Update();
        }
    }
}
