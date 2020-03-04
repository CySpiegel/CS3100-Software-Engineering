using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.Group5.CardsAgainstHumanity
{
    public class StatusBarPanel : MonoBehaviour
    {
        private readonly string connectionStatusMessage = "    Connection Status: ";

        [Header("UI References")]
        public Text ConnectionStatusText;

        public void Update()
        {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }
    }
}