using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace com.Group5.CardsAgainstHumanity
{
    public class CardController : MonoBehaviour, IPunInstantiateMagicCallback
    {
        public Button cardSelected;
        public Text cardText;
        private CardInfo gameManager;
        public bool isSelectable;
        private PhotonView myPhotonView;
        public int submitterID;

        void Awake()
        {
            myPhotonView = GetComponent<PhotonView>();
            gameManager = GameObject.Find("GameManager").GetComponent<CardInfo>();
            cardSelected.onClick.AddListener(cardSelectedFunc);
            //newCard();
        }

        void Start()
        {
            //setup button listeners




        }
        //
        // Update is called once per frame
        void Update()
        {

        }
        public void newCard()
        {
            string cardDrawn = gameManager.whiteCards[Random.Range(0, gameManager.whiteCards.Count)];
            gameManager.whiteCards.Remove(cardDrawn); //need to make rpc to remove card from all players hands
            cardText.text = cardDrawn;
            gameManager.myPhotonView.RPC("removeCard", RpcTarget.All, cardDrawn);
            //  Debug.Log(message: $"drew card (removed) {cardText.text}");
            //  Debug.Log("Set as not selectable");
        }
        public void displayOnCard(string cardSelectedText)
        {
            //made workaround for "drawing" extra card when moving a card from the hand to the selection area, by "reshuffling" the card accidentally drawn back into the deck. if you have a solution please let me know
            gameManager.whiteCards.Add(cardText.text);
            cardText.text = cardSelectedText;
        }
        //When card is selected/clicked
        void cardSelectedFunc()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.Log(gameManager.currentCardTsar);
            if (isSelectable && PhotonNetwork.LocalPlayer.ActorNumber == gameManager.currentCardTsar) // isSelectable set to true in CardInfo script in order to make card be selectable for winning the round, set to false by default in Start()
            {
                Debug.Log($"Winner selected: {cardText.text}");
                myPhotonView.RPC("clearCards", RpcTarget.All, submitterID);
                gameManager.myPhotonView.RPC("UpdateCardTsar", RpcTarget.All);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber != gameManager.currentCardTsar)
            {

                if (gameManager.ableToSubmit == true)
                {
                    gameManager.myPhotonView.RPC("DisplayCard", RpcTarget.AllViaServer, cardText.text, gameManager.ableToSubmit, PhotonNetwork.LocalPlayer.ActorNumber);
                    newCard();
                    gameManager.preStopSubmission = true;
                }

            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            FindObjectOfType<CardInfo>().cardInPlay.Add(this.gameObject);
            gameManager.GetComponent<PhotonView>().RPC("DisplayCardNotMaster", RpcTarget.AllViaServer);
            if (gameManager.preStopSubmission == true)
                gameManager.ableToSubmit = false;
            submitterID = gameManager.lastCardSubmitter;
        }
        [PunRPC]
        void clearCards(int winnerID)
        {
            PhotonNetwork.PlayerList[winnerID - 1].AddScore(1);
            gameManager.cardInPlay.ForEach(Destroy);
            gameManager.cardInPlay.Clear();
            gameManager.ableToSubmit = true;
            gameManager.preStopSubmission = false;
            GameObject.Find("BlackCard").GetComponent<BlackCardScript>().drawNewBlackCard();
            if (PhotonNetwork.PlayerList[winnerID - 1].GetScore() >= 7)
            {
                gameManager.myPhotonView.RPC("GameOver", RpcTarget.AllViaServer, winnerID - 1);
            }
            else
            {
                gameManager.myPhotonView.RPC("UpdateCardTsar", RpcTarget.All);
            }
        }
    }
}