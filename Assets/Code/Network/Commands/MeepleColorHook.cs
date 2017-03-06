using UnityEngine;

namespace Code.Network.Commands {
    public class MeepleColorHook : MonoBehaviour {
        //[SerializeField]
        public int CurLine;

        void OnMouseDown() {
            //var curLine = Convert.ToInt32(name.Substring(name.Length - 2).Substring(0, 1));
            //if (transform.FindChild("Name").GetComponent<Text>().text != PlayerSync.PlayerInfo.PlayerName) return;
            if (CurLine != PlayerSync.PlayerInfo.MySlotNumber) return;
            if (!PlayerSync.PlayerInfo.IsRegistred || PlayerSync.PlayerInfo.WaitingForColorUpgrade) return;
            Net.Client.Send(NetCmd.FreeColorRequest, Net.Client.MyInfo());
            PlayerSync.PlayerInfo.WaitingForColorUpgrade = true;
        }

        void OnMouseOver() {
        }
    }
}
