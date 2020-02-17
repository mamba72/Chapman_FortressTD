using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;
using Com.MyCompany.FortressTD.NetworkingHelpers;

public class GameManagerScript : MonoBehaviour
{
	//both tile prefabs
	public GameObject groundTilePrefab;
	public GameObject pathTilePrefab;

	//public GameObject GameBoard;

	public GameObject StartTile;
	public GameObject EndTile;
	public List<GameObject> PathTileOrder;
    //list of all of the enemies
    //public List<GameObject> AllEnemies = new List<GameObject>();

	//the array that makes up the board
	public GameObject[,] TileArray;
	public int maxX = 10;
	public int maxY = 10;
	public BoardBounds BoardBounds;

	//list of all Buildings you can place
	public List<GameObject> PlaceableBuildings = new List<GameObject>();
	//what building is currently selected
	public GameObject SelectedBuilding = null;
	//list of buildings already placed
	public List<GameObject> PlacedDownBuildings = new List<GameObject>();

	//enemy spawner managing
	public GameObject enemySpawnerObj;
	public EnemySpawnerScript enemySpawner;
	public GameObject enemyGoalObj;
	public EnemyGoalScript enemyGoal;

	//general game management
	public int WaveNumber = 0;
	public int NumberOfLives = 10;
    public Text WaveText;

	public EconomyManagerScript EconomyManager;

    //game over variables 
    public GameObject GameOverInfo;


	//NETWORKING MANAGEMENT
	public bool isMultiplayer = false;
	public GameObject NetworkGameManager;
	public GameSetupController netGm;    

	//game manager events
	

	//event subscriptions
	private void OnEnable()
	{
        //subscribing to the wave event
        EnemySpawnerScript.OnWaveEnded += WaitForWave;
	}
	private void OnDisable()
	{
        EnemySpawnerScript.OnWaveEnded -= WaitForWave;
    }


	// Start is called before the first frame update
	void Start()
	{

		netGm = NetworkGameManager.GetComponent<GameSetupController>();

		PathTileOrder = new List<GameObject>();
		BoardBounds.x = maxX;
		BoardBounds.y = maxY;

		TileArray = new GameObject[maxX, maxY];

		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void LostLife()
	{
		NumberOfLives--;
	}

    //text to tell the user the game is over
    public void GameOver()
    {
        GameOverInfo.SetActive(true);
    }

    public void WaitForWave()
    {
        //increase wave number
        WaveNumber++;
        //change the wave text
        WaveText.text = "Wave Ended";
        //start a timer until the next wave 
        StartCoroutine("WaveTimer");
    }

    public void startNewWave()
    {
        Debug.Log("in start new wave function");
        WaveText.text = "New Wave Started";
        enemySpawner.StartSpawning();
    }

    IEnumerator WaveTimer()
    {
        Debug.Log("started wave timer");
        yield return new WaitForSeconds(5.0f);
        //WaveText.text = "new wave started";
        //enemySpawner.StartSpawning();
        startNewWave();
        Debug.Log("wave timer ended");
    }
}
