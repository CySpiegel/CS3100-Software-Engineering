using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Group5.CardsAgainstHumanity
{
    public class PlayerOverviewPanel : MonoBehaviourPunCallbacks
    {
        public GameObject PlayerOverviewEntryPrefab;
        public Dictionary<int, GameObject> listOfPlayers;
        private CardInfo gameManager;
        public void Awake()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<CardInfo>();
            listOfPlayers = new Dictionary<int, GameObject>();
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject PlayerEntry = Instantiate(PlayerOverviewEntryPrefab);
                PlayerEntry.transform.SetParent(gameObject.transform);
                PlayerEntry.transform.localScale = Vector3.one;
                PlayerEntry.GetComponent<Text>().color = CardsAgainstHumanityGame.GetColor(player.GetPlayerNumber());
                PlayerEntry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}", player.NickName, player.GetScore());
                listOfPlayers.Add(player.ActorNumber, PlayerEntry);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(listOfPlayers[otherPlayer.ActorNumber].gameObject);
            listOfPlayers.Remove(otherPlayer.ActorNumber);
        }




        public void Update()
        {
            int playerID = 0;
            if (gameManager.isConnected == true)
            {
                foreach (int x in listOfPlayers.Keys)
                {
                    if (PhotonNetwork.PlayerList[playerID].ActorNumber == gameManager.currentCardTsar)
                        listOfPlayers[x].GetComponent<Text>().color = Color.red;
                    else
                        listOfPlayers[x].GetComponent<Text>().color = Color.blue;
                    listOfPlayers[x].GetComponent<Text>().text = string.Format("{0}\nScore: {1}", PhotonNetwork.PlayerList[playerID].NickName, PhotonNetwork.PlayerList[playerID].GetScore());
                    playerID++;
                }
            }
            else
            {
                PhotonNetwork.LoadLevel("MainMenu");
            }
        }
    }
}