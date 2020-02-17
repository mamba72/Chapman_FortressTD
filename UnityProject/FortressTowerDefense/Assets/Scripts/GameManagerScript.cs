using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;
using Com.MyCompany.FortressTD.NetworkingHelpers;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManagerScript : MonoBehaviour, IOnEventCallback
{
	[Header("Tile Prefabs")]
	public GameObject groundTilePrefab;
	public GameObject pathTilePrefab;

	//public GameObject GameBoard;

	public GameObject StartTile;
	public GameObject EndTile;
	public List<GameObject> PathTileOrder;
    //list of all of the enemies
    //public List<GameObject> AllEnemies = new List<GameObject>();

	

	[Header("List of all Buildings you can place")]
	public List<GameObject> PlaceableBuildings = new List<GameObject>();
	//what building is currently selected
	public GameObject SelectedBuilding = null;
	//list of buildings already placed
	public List<GameObject> PlacedDownBuildings = new List<GameObject>();

	[Header("Enemy Spawner Managing")]
	public GameObject enemySpawnerObj;
	public EnemySpawnerScript enemySpawner;
	public GameObject enemyGoalObj;
	public EnemyGoalScript enemyGoal;

	

	[Header("General Game Management")]
	public int WaveNumber = 0;
	public int NumberOfLives = 10;
    public Text WaveText;
	public float TimeBeforeGameStarts = 5;
	public float TimeBetweenWaves = 10;

	public EconomyManagerScript EconomyManager;
	public MapCreationScript MapCreator;
	public UIManager UIManager;

	//the array that makes up the board
	public GameObject[,] TileArray;
	public BoardBounds BoardBounds;


	[Header("NETWORKING MANAGEMENT")]
	public bool isMultiplayer = false;
	public GameObject NetworkGameManager;
	public GameSetupController netGm;

	[HideInInspector]
	public Photon.Pun.PhotonView PhotonView;

	//game manager events
	


	//event subscriptions
	private void OnEnable()
	{
        //subscribing to the wave event
        EnemySpawnerScript.OnWaveEnded += WaitForWave;
		PhotonNetwork.AddCallbackTarget(this);
	}
	private void OnDisable()
	{
        EnemySpawnerScript.OnWaveEnded -= WaitForWave;
		PhotonNetwork.RemoveCallbackTarget(this);
    }

	private void Awake()
	{
		//grab the photon view from this object
		PhotonView = Photon.Pun.PhotonView.Get(this);
	}

	// Start is called before the first frame update
	void Start()
	{
		isMultiplayer = Settings.GetIsMultiplayer();
		netGm = NetworkGameManager.GetComponent<GameSetupController>();

		PathTileOrder = new List<GameObject>();

		BoardBounds lobbySetBounds = Settings.GetMapDimentions();

		BoardBounds.x = lobbySetBounds.x;
		BoardBounds.y = lobbySetBounds.y;

		TileArray = new GameObject[BoardBounds.x, BoardBounds.y];

        UIManager.LivesText.text = "Lives: " + NumberOfLives;
        UIManager.WaveCountdown.gameObject.SetActive(false);
        WaveText.gameObject.SetActive(false);
        CreateMap();
	}

	// Update is called once per frame
	void Update()
	{
		UIManager.LivesText.text = "Lives: " + NumberOfLives;
	}

	public void LostLife()
	{
		NumberOfLives--;
        //UIManager.LivesText.text = "Lives: " + NumberOfLives;

		//if the number of lives is 0, then the game is over
		if(NumberOfLives == 0)
		{
            if (isMultiplayer && netGm.isMasterClient)
                GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.GameOver, null);
            else
                GameOver();
		}
	}

    //text to tell the user the game is over
    public void GameOver()
    {
		//all the leaderboard stuff
		
		Settings.SetHighestWaves(WaveNumber);

        UIManager.GameOverInfo.SetActive(true);
        UIManager.EndRoundText.text += "" + WaveNumber;
        enemySpawner.StopAllCoroutines();
        enemySpawner.StopSpawning();
		//UIManager.InGameUI.SetActive(false);
		//Invoke("Quit", 5);
    }

    public void WaitForWave()
    {
		
		WaveText.gameObject.SetActive(true);
        if (WaveNumber != 0)
        {
            //change the wave text
            WaveText.text = "Wave " + WaveNumber + " Ended";
        }
        //make the text disspear after a certain amount of time
        Invoke("DisableText", 5.0f);
        //start a timer until the next wave 
        StartCoroutine("WaveTimer");
    }

    public void startNewWave()
    {
		Settings.AddWaves(1);
		//increase wave number
		WaveNumber++;
		WaveText.gameObject.SetActive(true);
		WaveText.text = "Wave " + WaveNumber + " Started";
        //make the text disspear after a certain amount of time
        Invoke("DisableText", 5.0f);

		if (isMultiplayer == false || netGm.isMasterClient == true)
		{
			enemySpawner.StartSpawning();
		}
		
    }

    public void DisableText()
    {
		//WaveText.text = "";
		WaveText.gameObject.SetActive(false);
    }
    
    
    IEnumerator WaveTimer()
    {
        UIManager.WaveCountdown.gameObject.SetActive(true);
        for(int i = 0; i < TimeBetweenWaves; ++i)
        {
            UIManager.WaveCountdown.text = "Next Wave In " + (TimeBetweenWaves -i);
            yield return new WaitForSeconds(1);
        }
        //yield return new WaitForSeconds(TimeBetweenWaves);
        //WaveText.text = "new wave started";
        //enemySpawner.StartSpawning();
        startNewWave();
        UIManager.WaveCountdown.gameObject.SetActive(false);
    }

	public void CreateMap()
	{
		MapCreator.StartMapCreation();
	}

    public void Quit()
    {
        if (Settings.GetIsMultiplayer())
		{
			netGm.LeaveRoom();
			netGm.DisconnectFromGame();
		}
            
        SceneManager.LoadScene("MainMenuScene");
    }


	public void StartGame()
	{
		Debug.Log("Starting Game!");
		//only start the spawning script if the player is the host or if they are in single player
		if(isMultiplayer == false || netGm.isMasterClient == true)
			StartCoroutine(GameIsReadyStartSpawning());

		
	}

	private IEnumerator GameIsReadyStartSpawning()
	{
		yield return new WaitForSeconds(TimeBeforeGameStarts);
		WaitForWave();
	}



	//this will be called every time an event is fired through Photon
	public void OnEvent(EventData photonEvent)
	{
		byte eventCode = photonEvent.Code;
		object[] data;

		switch(eventCode)
		{
			//wave ended event
			case GlobalVariables.EventCodes.WaveEnded:
				//Debug.Log("The Wave Ended Event was received");
				WaitForWave();
				break;

			//spending coins event
			case GlobalVariables.EventCodes.SpentCoins:
				//Debug.Log("The Spent coins event was received");
				data = (object[])photonEvent.CustomData;
				int cost = (int)data[0];
				EconomyManager.SpendCoins(cost);
				break;

			//other player upgraded tower
			case GlobalVariables.EventCodes.BuildingUpgraded:
				//Debug.Log("The Upgraded Building event was received");
				data = (object[])photonEvent.CustomData;
				int buildingIndex = (int)data[0];

				Building building = PlacedDownBuildings[buildingIndex].GetComponent<Building>();
				//ignore the cost to upgrade since it was already taken out of bank
				building.UpgradeBuilding(ignoreCost: true);

				break;

			//game is over
			case GlobalVariables.EventCodes.GameOver:
				GameOver();
				break;

			case GlobalVariables.EventCodes.PathTileCreated:

				Debug.Log("Event Received. Added new tile");
				data = (object[]) photonEvent.CustomData;
				GameObject newPath = GameObject.Find((string)data[0]);

				PathTileOrder.Add(newPath);


				break;
		}
	}
}
