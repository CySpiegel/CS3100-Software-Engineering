using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Group5.CardsAgainstHumanity
{
    public class ListOfRoomEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;
        public Button JoinGameButton;

        private string roomName;

        public void Start()
        {
            JoinGameButton.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }
                PhotonNetwork.JoinRoom(roomName);
            });
        }

        public void Initialize(string theRoomName, byte currentPlayers, byte maxPlayers)
        {
            roomName = theRoomName;
            RoomNameText.text = theRoomName;
            RoomPlayersText.text = currentPlayers + " : " + maxPlayers;
        }

    }
}