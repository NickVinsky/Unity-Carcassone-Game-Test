using Code;
using UnityEngine;

public class OnChatPanel : MonoBehaviour {

    private void OnMouseOver() {
        MainGame.MouseOnChat = true;
    }

    private void OnMouseExit() {
        MainGame.MouseOnChat = false;
    }
}
