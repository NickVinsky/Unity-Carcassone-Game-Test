using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.Network {
    public class PlayerSync : NetworkBehaviour {
        public static PlayerInformation PlayerInfo;

        [SyncVar]
        public string PlayerName;

        public override void OnStartLocalPlayer() {
            SetPlayerName();
        }

        [Client]
        void SetPlayerName() {
            PlayerName = GameObject.Find(GameRegulars.PlayerNameInputField).GetComponent<InputField>().text;
            PlayerInfo.PlayerName = PlayerName;
            CmdSendNameToServer(PlayerName);
        }

        [Command]
        void CmdSendNameToServer(string nameToSend) {
            RpcSetPlayerName(nameToSend);
        }

        [ClientRpc]
        void RpcSetPlayerName(string playerName) {
            PlayerName = playerName;
            //Debug.Log(playerName);
        }
    }
}
