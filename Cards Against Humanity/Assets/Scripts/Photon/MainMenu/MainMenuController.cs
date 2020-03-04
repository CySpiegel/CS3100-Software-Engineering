using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Group5.CardsAgainstHumanity
{


    public class MainMenuController : MonoBehaviourPunCallbacks
    {
        /*
         * cachedServerBrowserList Contains a dictionary of all available games during last check
         * 
         * 
         */
        static private bool logedInFirst;
        private Dictionary<string, RoomInfo> cachedServerBrowserList;
        private Dictionary<string, GameObject> ListOfGameRooms;
        private Dictionary<int, GameObject> ListOfPlayers;

        public GameObject SelectedPanel;

        public static MainMenuController lobby;
        //NavRibbon
        public GameObject ProfileIconButton;
        public GameObject ProfilePageButton;
        public GameObject MultiPlayerIconButton;
        public GameObject MultiPlayerButton;


        //LoginPanel
        public GameObject LoginScreenPanel;
        public GameObject LoginButton;
        public InputField PlayerNameInput;

        //MultiplayerSelectionPanel
        public GameObject MultiplayerSelectionPanel;
        public GameObject CreateGameButton;
        public GameObject ServerBrowserButton;
        public GameObject RandomServerButton;

        //ServerBrowserPanel
        public GameObject ServerBrowserPanel;
        public GameObject ServerBrowserContent;
        public GameObject ServerBrowserBackButton;

        //CreateRoomPanel
        public GameObject CreateRoomPanel;
        public GameObject CreateRoomButton;
        public GameObject CancelRoomButton;
        public InputField RoomNameInput;
        public InputField MaxPlayerCountInput;


        //RoomLobbyPanel
        public GameObject RoomPanel;
        public GameObject JoiningRandomPanel;
        public GameObject LaunchGameButton;
        public GameObject PlayerListContent;


        public GameObject PlayerListEntryPrefab;
        public GameObject RoomListEntryPrefab;


        private byte maxPlayersPerRoom = 4;
        // Start is called before the first frame update

        /*
         * Start is called before the first frame update
         * Connects to Master Photon Server and displays
         * a message in the top ribbon of the program.
         */
        void Start()
        {
            logedInFirst = false;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void Awake()
        {
            
            PhotonNetwork.AutomaticallySyncScene = true;
            cachedServerBrowserList = new Dictionary<string, RoomInfo>();
            ListOfGameRooms = new Dictionary<string, GameObject>();
            //random PlayerName generation
            PlayerNameInput.text = "Player " + Random.Range(1000, 100000);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LeaveGameAction()
        {
            ClearRoomListView();
            PhotonNetwork.LeaveRoom();
            SelectionPanel(MultiplayerSelectionPanel.name);
        }

        /*
         * Reagions are awsome
         * allows for segmentation of code to into blocks of
         * functionality. This contains all events as a
         * result of a button click
         * 
         */
        #region ButtonClick()

        public void OnLoginButtonClicked()
        {
            Debug.Log("Login button was clicked");
            //LoginScreenPanel.SetActive(false);
            

            //getting player name from input box
            string nameOfPlayer = PlayerNameInput.text;
            //error out if input is blank
            if (!nameOfPlayer.Equals(""))
            {
                logedInFirst = true;
                PhotonNetwork.LocalPlayer.NickName = nameOfPlayer;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player name is invalid. Must Type a name!");
            }

            //change to next Panel
            SelectionPanel(MultiplayerSelectionPanel.name);
        }


        public void OnCreateGameButtonClicked()
        {
            Debug.Log("Create Game button was clicked");
            //Sets CreateRoomPanel to active
            SelectionPanel(CreateRoomPanel.name);
            //TODO: Functionality Complete
        }



        public void OnServerBrowserButtonClicked()
        {
            Debug.Log("Server Browser button was clicked");
            ClearRoomListView();
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SelectionPanel(ServerBrowserPanel.name);
        }

        public void OnRandomServerButtonClicked()
        {
            Debug.Log("Join Game button was clicked");
            PhotonNetwork.JoinRandomRoom(); //Trying to join a random room
            SelectionPanel(RoomPanel.name);
            //TODO: Launch into multiplayer lobby
        }

        public void OnLaunchGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Cards");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
            }
        }

        public void OnJoinButtonClicked()
        {
            ServerBrowserPanel.SetActive(false);
        }

        public void OnLeaveGameButtonClicked()
        {
            ClearRoomListView();
            PhotonNetwork.LeaveRoom();
            if(PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            SelectionPanel(MultiplayerSelectionPanel.name);
        }

        public void OnMultiPlayerButtonClicked()
        {

            if (logedInFirst.Equals(false))
            {
                string nameOfPlayer = PlayerNameInput.text;
                //error out if input is blank
                if (!nameOfPlayer.Equals(""))
                {
                    logedInFirst = true;
                    PhotonNetwork.LocalPlayer.NickName = nameOfPlayer;
                    PhotonNetwork.ConnectUsingSettings();

                    SelectionPanel(MultiplayerSelectionPanel.name);
                    ClearRoomListView();
                    PhotonNetwork.LeaveRoom();
                    if (PhotonNetwork.InLobby)
                    {
                        PhotonNetwork.LeaveLobby();
                    }
                }
                else
                {
                    Debug.LogError("Player name is invalid. Must Type a name!");
                }
            }
            else
            {

                SelectionPanel(MultiplayerSelectionPanel.name);
                ClearRoomListView();
                PhotonNetwork.LeaveRoom();
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }
            }
        }

        public void OnProfileButtonClicked()
        {
            SelectionPanel(LoginScreenPanel.name);
            ClearRoomListView();
            PhotonNetwork.LeaveRoom();
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
        }

        public void OnCreateRoomButtonClicked()
        {
            Debug.Log("Create room button was clicked");
            string roomName = RoomNameInput.text;
            if (roomName.Equals(string.Empty))
            {
                roomName = "Room " + Random.Range(1000, 100000);
            }
            
            byte maxPlayers;
            byte.TryParse(MaxPlayerCountInput.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 7);
            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
            PhotonNetwork.CreateRoom(roomName, options, null);
            SelectionPanel(RoomPanel.name);
        }

        #endregion


        /*
         * Allows me to do multiple panel activations when
         * activating a particular panel. saves time in having
         * to relist all panel activations. reads incomming 
         * panel name and activates it while setting all others 
         * to false.
         * 
         */
        private void SelectionPanel(string GoingToPanel)
        {
            LoginScreenPanel.SetActive(GoingToPanel.Equals(LoginScreenPanel.name));
            MultiplayerSelectionPanel.SetActive(GoingToPanel.Equals(MultiplayerSelectionPanel.name));
            CreateRoomPanel.SetActive(GoingToPanel.Equals(CreateRoomPanel.name));
            ServerBrowserPanel.SetActive(GoingToPanel.Equals(ServerBrowserPanel.name));
            ServerBrowserPanel.SetActive(GoingToPanel.Equals(ServerBrowserPanel.name));
            RoomPanel.SetActive(GoingToPanel.Equals(RoomPanel.name));
        }

        void CreateRoom()
        {
            Debug.Log("Trying to create Room");
            int randomRoomName = Random.Range(1000, 10000);
            //Grab max players from input

            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayersPerRoom };
            PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
        }

        #region Overrided Functions

        public override void OnConnectedToMaster()
        {
            Debug.Log("Player has connected to the Photon master server");
            //Player is now connected to servers, enable battlebutton
            LoginButton.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to join a random game but failed. There must be no open games available");
            string roomName = "Room " + Random.Range(1000, 1000000);
            RoomOptions options = new RoomOptions { MaxPlayers = 9 };
            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SelectionPanel(MultiplayerSelectionPanel.name);
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject playerEntry = Instantiate(PlayerListEntryPrefab);
            playerEntry.transform.SetParent(PlayerListContent.transform);
            playerEntry.transform.localScale = Vector3.one;
            playerEntry.GetComponent<ListOfPlayerEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
            ListOfPlayers.Add(newPlayer.ActorNumber, playerEntry);
            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(ListOfPlayers[otherPlayer.ActorNumber].gameObject);
            ListOfPlayers.Remove(otherPlayer.ActorNumber);
            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
        }

        public override void OnLeftRoom()
        {
            SelectionPanel(MultiplayerSelectionPanel.name);

            foreach (GameObject entry in ListOfPlayers.Values)
            {
                Destroy(entry.gameObject);
            }
            ListOfPlayers.Clear();
            ListOfPlayers = null;
        }

        public override void OnLeftLobby()
        {
            ListOfGameRooms.Clear();
            ClearRoomListView();
        }


        public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProperties)
        {
            if (ListOfPlayers == null)
            {
                ListOfPlayers = new Dictionary<int, GameObject>();
            }
            GameObject newEntry;
            if (ListOfPlayers.TryGetValue(target.ActorNumber, out newEntry))
            {
                object isReady;
                if (changedProperties.TryGetValue(CardsAgainstHumanityGame.PLAYER_READY, out isReady))
                {
                    newEntry.GetComponent<ListOfPlayerEntry>().SetPlayerReady((bool)isReady);
                }
            }
            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Successfully joined a room");
            SelectionPanel(RoomPanel.name);

            if (ListOfPlayers == null)
            {
                ListOfPlayers = new Dictionary<int, GameObject>();
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<ListOfPlayerEntry>().Initialize(player.ActorNumber, player.NickName);

                object isPlayerReady;
                if (player.CustomProperties.TryGetValue(CardsAgainstHumanityGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<ListOfPlayerEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                ListOfPlayers.Add(player.ActorNumber, entry);
            }

            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());

            Hashtable props = new Hashtable
            {
                {CardsAgainstHumanityGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }


        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to create new room but failed, their must already be a room with the same name");
            SelectionPanel(MultiplayerSelectionPanel.name);
        }
        #endregion



        #region private Functions
        private void ClearRoomListView()
        {
            foreach (GameObject entry in ListOfGameRooms.Values)
            {
                Destroy(entry.gameObject);
            }
            ListOfGameRooms.Clear();
        }

        private void UpdateCachedRoomList(List<RoomInfo> listOfRooms)
        {
            foreach (RoomInfo info in listOfRooms)
            {
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedServerBrowserList.ContainsKey(info.Name))
                    {
                        cachedServerBrowserList.Remove(info.Name);
                    }
                    continue;
                }

                if (cachedServerBrowserList.ContainsKey(info.Name))
                {
                    cachedServerBrowserList[info.Name] = info;
                }
                else
                {
                    cachedServerBrowserList.Add(info.Name, info);
                }
            }
        }

        private bool PlayerReadyStatusCheck()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object isReadyStatus;
                if (player.CustomProperties.TryGetValue(CardsAgainstHumanityGame.PLAYER_READY, out isReadyStatus))
                {
                    if (!(bool)isReadyStatus)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void LocalPlayerReadyCheck()
        {
            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedServerBrowserList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(ServerBrowserContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<ListOfRoomEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);
                ListOfGameRooms.Add(info.Name, entry);
            }
        }

        public void LocalPlayerPropertiesUpdated()
        {
            LaunchGameButton.gameObject.SetActive(PlayerReadyStatusCheck());
        }

        #endregion
    }
}