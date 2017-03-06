using Code;
using UnityEngine;

public class OnChatPanel : MonoBehaviour {

    private void OnMouseOver() {
        Game.MouseOnChat = true;
    }

    private void OnMouseExit() {
        Game.MouseOnChat = false;
    }
}
