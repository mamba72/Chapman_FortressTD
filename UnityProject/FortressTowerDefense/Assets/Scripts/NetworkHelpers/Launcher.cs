using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

namespace Com.MyCompany.FortressTD.NetworkingHelpers
{

	public class Launcher : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		#region Private Serializable Fields

		/// <summary>
		/// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
		/// </summary>
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
		[SerializeField]
		private byte maxPlayersPerRoom = 2;

		#endregion


		#region Private Fields

		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		/// <summary>
		/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
		/// </summary>
		string gameVersion = "1";

		//[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		//[SerializeField]
		//private GameObject controlPanel;
		[Tooltip("The UI Label to inform the user that the connection is in progress")]
		[SerializeField]
		public GameObject progressLabel;


		#endregion

		public int RandomRoomNumber = -1;
		public int NumberOfPlayersConnected = 0;
		public bool isConnected = false;
		public bool isInRoom = false;
		public bool isMasterClient = false;

		public int CurrentRoomNumber = -1;

		public LobbyManagerScript lmScript;

		#region MonoBehaviour CallBacks


		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake()
		{
			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;
		}


		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start()
		{
			//Connect();
			progressLabel.SetActive(false);
			//controlPanel.SetActive(true);
		}

		private void Update()
		{
			

		}


		#endregion

		//events
		public delegate void HasJoinedRoom();
		public static event HasJoinedRoom OnHasJoinedRoom;

		public delegate void SomeoneJoined(Dictionary<int, Player> Players);
		public static event SomeoneJoined OnSomeoneJoined;


		#region Public Methods


		/// <summary>
		/// Start the connection process.
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
			PhotonNetwork.NickName = Settings.GetUsername();

			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = PhotonNetwork.ConnectUsingSettings();

			progressLabel.SetActive(true);
			//controlPanel.SetActive(false);
			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.IsConnected)
			{
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
			}
			else
			{
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = gameVersion;
			}


		}


		#endregion


		#region MonoBehaviourPunCallbacks Callbacks


		public override void OnConnectedToMaster()
		{
			Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

			// we don't want to do anything if we are not attempting to join a room.
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
				PhotonNetwork.JoinRandomRoom();
				isConnecting = false;
			}

			isConnected = true;
		}


		public override void OnDisconnected(DisconnectCause cause)
		{
			progressLabel.SetActive(false);
			//controlPanel.SetActive(true);
			Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
		}

		//Youtube tutorial didnt have these: 

		//public override void OnJoinRandomFailed(short returnCode, string message)
		//{
		//	Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		//	// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		//	PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
		//}

		//public override void OnJoinedRoom()
		//{
		//	Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
		//	// #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
		//	if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		//	{
		//		Debug.Log("We load the 'Room for 1' ");


		//		// #Critical
		//		// Load the Room Level.
		//		PhotonNetwork.LoadLevel("Room for 1");
		//	}
		//}


		#endregion

		#region Things From Youtube Tutorial

		void CreateRoom()
		{
			Debug.Log("Creating Room Now");
			int randomRoomNumber = UnityEngine.Random.Range(0, 10000); //craetes a random name for the room
			RandomRoomNumber = randomRoomNumber;
			RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayersPerRoom };

			PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //attempting to create a new room
			Debug.Log("Random Room Number: " + randomRoomNumber);
		}

		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			//this is because if we fail to make a room, its most likely because that room name already exists, so we create a new one
			Debug.Log("Failed to create room... trying again");
			CreateRoom();
		}

		public void QuickCancel()
		{
			PhotonNetwork.LeaveRoom();
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("Failed to join a room. Creating our own");
			CreateRoom();
		}

		#endregion


		public override void OnJoinedRoom()
		{
			isMasterClient = PhotonNetwork.IsMasterClient;
			isInRoom = true;

			CurrentRoomNumber = Convert.ToInt32(PhotonNetwork.CurrentRoom.Name.Replace("Room", ""));

			OnHasJoinedRoom();
			OnSomeoneJoined(PhotonNetwork.CurrentRoom.Players);
		}


		public override void OnPlayerEnteredRoom(Player player)
		{
			//Debug.Log("OnPlayerEnteredRoom was called");
			OnSomeoneJoined(PhotonNetwork.CurrentRoom.Players);
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

		public void OnEvent(EventData photonEvent)
		{
			int eventCode = photonEvent.Code;

			object[] data = (object[])photonEvent.CustomData;

			switch(eventCode)
			{
				case GlobalVariables.EventCodes.GameDimentionsChanged:

					int x = (int)data[0];
					int y = (int)data[1];

					Settings.SetMapDimentions(x, y);

					lmScript.DisplayBoardBounds();

					break;

			}
		}
	}
}
