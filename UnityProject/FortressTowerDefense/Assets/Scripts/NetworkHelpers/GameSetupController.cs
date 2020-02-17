using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;

namespace Com.MyCompany.FortressTD.NetworkingHelpers
{
    public class GameSetupController : MonoBehaviourPunCallbacks//, IInRoomCallbacks
    {

        private GameManagerScript gm;
        public Text LeavingGameText;

        //multiplayer vars
        public bool isConnected;
        public bool isMasterClient;

        // Start is called before the first frame update
        void Start()
        {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();

            isConnected = PhotonNetwork.IsConnectedAndReady;
            isMasterClient = PhotonNetwork.IsMasterClient;
            CreatePlayer();

            LeavingGameText.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void CreatePlayer()
        {
            Debug.Log("Creating Player");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
        }


        //i did this shit
        //public void InstantiateOverNetwork(string PrefabName, Vector3 vector3, Quaternion rotation)
        //{

        //    string name = Path.Combine("PhotonPrefabs", PrefabName);
        //    PhotonNetwork.Instantiate(name, vector3, rotation);
        //}

        //instantiate using prefab name and category
        public GameObject InstantiateOverNetwork(string prefabCategory, string prefabName, Vector3 vector3, Quaternion rotation)
        {
            string name = Path.Combine("PhotonPrefabs", prefabCategory, prefabName);
            return PhotonNetwork.Instantiate(name, vector3, rotation);
        }

        //instantiate using prefab object
        public GameObject InstantiateOverNetwork(GameObject obj, Vector3 vector3, Quaternion rotation)
        {
            PrefabInfo info = obj.GetComponent<PrefabInfo>();

            if(info == null)
            {
                info = obj.GetComponentInChildren<PrefabInfo>();
            }
            return InstantiateOverNetwork(info.Category, info.PrefabName, vector3, rotation);
        }


        public void DestroyOverNetwork(GameObject obj)
        {
            //Debug.Log("Destroyed Object over network");

            if(isMasterClient || gm.isMultiplayer == false)
                PhotonNetwork.Destroy(obj);
            else
            {
                Debug.Log("Tried to destroy object, but we are not master client");
            }
        }

        public void LeaveRoom()
        {
            Debug.Log("Leaving room");
            PhotonNetwork.LeaveRoom();
            //PhotonNetwork.SendAllOutgoingCommands();
        }

        public void DisconnectFromGame()
        {
            PhotonNetwork.Disconnect();
        }

        //public void OnPlayerEnteredRoom(Player newPlayer)
        //{
        //    //throw new System.NotImplementedException();
        //}

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //throw new System.NotImplementedException();
            Debug.Log("Player Left room. Leaving Game");

            LeavingGameText.gameObject.SetActive(true);

            Invoke("QuitGame", 5);
        }

        //public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        //{
        //    //throw new System.NotImplementedException();
        //}

        //public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        //{
        //    //throw new System.NotImplementedException();
        //}

        //public void OnMasterClientSwitched(Player newMasterClient)
        //{
        //    //throw new System.NotImplementedException();
        //}

        //public void OnPhotonPlayerDisconnected(Player player)
        //{

        //}


        private void QuitGame()
        {
            gm.Quit();
        }
    }
}
