using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to enable PUN
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.FortressTD.NetworkingHelpers
{
    public class NetworkingGameManager : MonoBehaviourPunCallbacks
    { 
        [SerializeField]
        private string multiplayerSceneName; // the number for the build index to the multiplay scene
        

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room");
        }


        public void StartGame()
        {
            
            Debug.Log("Is MasterClient: " + PhotonNetwork.IsMasterClient);
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Starting Game");
                PhotonNetwork.LoadLevel(multiplayerSceneName);
            }
        }


       

    }
}


