using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class BlackCardScript : MonoBehaviour
{
    public Text cardText;
    private CardInfo allCardsList;
    private PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        allCardsList = GameObject.Find("GameManager").GetComponent<CardInfo>(); 
        drawNewBlackCard();
         
    }

    [PunRPC]
    void getBlackText(string blackText)
    {
        cardText.text = blackText;
    }

    public void drawNewBlackCard()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string cardDrawn = allCardsList.blackCards[Random.Range(0, allCardsList.blackCards.Count)];
            allCardsList.blackCards.Remove(cardDrawn);
            photonView.RPC("getBlackText", RpcTarget.All, cardDrawn);
        }
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    /*
    public void updateCard()
    {
        GetComponent<PhotonView>().RPC(
            "updateCardMethod",
            PhotonNetwork.All);
    }
    [PunRPC]
    void updateCardMethod()
    {
       string cardDrawn = allCardsList.blackCards[Random.Range(0, allCardsList.blackCards.Count)];
        allCardsList.blackCards.Remove(cardDrawn);
        cardText.text = cardDrawn;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
   */
}
