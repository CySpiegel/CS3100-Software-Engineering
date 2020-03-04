using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class CardInfo : MonoBehaviourPunCallbacks
{
    //private StreamReader reader = null;
    public Button leaveGame;
    public List<string> whiteCards = new List<string>();
    public List<string> blackCards = new List<string>();
    public List<GameObject> cardInPlay = new List<GameObject>();
    public List<GameObject> playersHand = new List<GameObject>();
    public GameObject cardPrefab;
    public PhotonView myPhotonView;
    private string lastCardSelected;
    private int thisCardIndex;
    public bool preStopSubmission;
    public int lastCardSubmitter;
    public bool ableToSubmit;
    private int cardTsarCount;
    public int currentCardTsar;// Used for stopping players from submitting multiple cards, set to false after one card submission, set to true after winning card selected (needs photon to complete)
    public TextAsset textFile;
    public bool isConnected;
    // Start is called before the first frame update
    void Awake()
    {
        leaveGame.onClick.AddListener(LeaveGameFunc);
        isConnected = true;
        cardTsarCount = -1;
        ableToSubmit = true;
        preStopSubmission = false;
        myPhotonView = GetComponent<PhotonView>();
        //string path = "/AllCards.txt";
        //var textFile = Resources.Load<TextAsset>(path);
        var splitFile = new char[] { '\r', '\n' };
        var cardLines = textFile.text.Split(splitFile);
        bool cardSwitch = false;
        string text = " ";
        //reader = new StreamReader(textFile);
        for(int i = 0; i < cardLines.Length; i++)
        {
            text = cardLines[i];
            if(text == "")
            {

            }
            else if (text == "--BREAK--")
            {
                cardSwitch = true;
            }
            else if (cardSwitch)
            {
                whiteCards.Add(text);
                //Debug.LogError(text);
            }
            else
            {
                blackCards.Add(text);
                //Debug.LogError(text);
            }
        };
        myPhotonView.RPC("UpdateCardTsar", RpcTarget.All);
    }


    void Start()
    {
        //Create Player Hand
        for (int x = 0; x < 7; x++)
        {
            GameObject newCard = Instantiate(cardPrefab, GameObject.Find("BlackCard").transform.position, Quaternion.identity);
            playersHand.Add(item: newCard);
            playersHand[x].transform.Translate(new Vector3(4 * (x + 1), -5));
            playersHand[x].GetComponent<CardController>().newCard();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom == false)
            PhotonNetwork.LoadLevel("MainMenu");
    }

    [PunRPC]
    public void DisplayCard(string incomingCardText,bool submissionPossible,int submitterID) //Returns true if selection okay, false if not. will instantiate new card prefab 3*(number of cards selected) units from black card, 
        //they will be half the size of a normal card and have the text (usually the selected cards text) placed on them
    {
        thisCardIndex = cardInPlay.Count;
        lastCardSelected = incomingCardText;
        lastCardSubmitter = submitterID;
        if (cardInPlay.Count < PhotonNetwork.PlayerList.Length && submissionPossible && PhotonNetwork.IsMasterClient)
        {  
            PhotonNetwork.InstantiateSceneObject("cardSpritePrefab", GameObject.Find("BlackCard").transform.position + new Vector3((thisCardIndex+1)*4f+3,0,0), Quaternion.identity);
        }
    }
    [PunRPC]
    public void DisplayCardNotMaster()
    {
        cardInPlay[thisCardIndex].GetComponent<CardController>().displayOnCard(lastCardSelected);
        cardInPlay[thisCardIndex].GetComponent<CardController>().isSelectable = true;
    }
    [PunRPC]
    public void UpdateCardTsar()
    {
        cardTsarCount++;
        if (cardTsarCount > PhotonNetwork.PlayerList.Length-1)
            cardTsarCount = 0;
        currentCardTsar = PhotonNetwork.PlayerList[cardTsarCount].ActorNumber;

    }
    [PunRPC]
    public void removeCard(string card)
    {
        whiteCards.Remove(card);
    }

    [PunRPC]
    public void GameOver(int winnerID)
    {
        isConnected = false;
        Debug.Log("GameOver");
        LeaveGameFunc();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }


    void LeaveGameFunc()
    {
        PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
    }


}
