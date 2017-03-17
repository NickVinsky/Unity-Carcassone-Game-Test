using UnityEngine;

namespace Code.Network.Commands {
    public class MeepleColorHook : MonoBehaviour {
        //[SerializeField]
        public int LineId;

        void OnMouseDown() {
            if (LineId != MainGame.Player.MySlotNumber) return;
            if (!MainGame.Player.IsRegistred || MainGame.Player.WaitingForColorUpgrade) return;

            Net.Client.Send(NetCmd.FreeColorRequest, Net.Client.MyInfo());
            MainGame.Player.WaitingForColorUpgrade = true;
        }

        void OnMouseOver() {
        }
    }
}
