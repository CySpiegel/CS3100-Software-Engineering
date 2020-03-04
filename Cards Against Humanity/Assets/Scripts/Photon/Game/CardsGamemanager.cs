using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;


namespace com.Group5.CardsAgainstHumanity
{
    namespace com.Group5.CardsAgainstHumanity
    {
        public class CardsGameManager : MonoBehaviourPunCallbacks
        {

            public static CardsGameManager Instance = null;

            public Text InfoText;

            //public GameObject[] whitecardprefab
            public void Awake()
            {
                Instance = this;
            }

            //

            public override void OnEnable()
            {
                base.OnEnable();
                CountdownTimer.OnCountdownTimerHasExpired += OnCountDownTimerExpired;
            }

            public override void OnDisable()
            {
                base.OnDisable();
                CountdownTimer.OnCountdownTimerHasExpired -= OnCountDownTimerExpired;
            }

            // Start is called before the first frame update
            void Start()
            {
                InfoText.text = "Waiting for all players to load...";
                Hashtable properties = new Hashtable
            {
                {
                    CardsAgainstHumanityGame.PLAYER_LOADED_LEVEL, true
                }
            };
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
            }

            // Update is called once per frame
            void Update()
            {

            }


            private void StartGame()
            {
                //start game by selecting card zar
            }


            private void OnCountDownTimerExpired()
            {
                StartGame();
            }
        }
    }
}