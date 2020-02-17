using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class EnemySpawnerScript : MonoBehaviour
{
	public PrefabInfo PrefabInfo;

	public bool SpawningEnabled = false;

	public float spawnRate = 1;
	public float timeBetweenEnemies = 4;
	public int totalEnemiesAllowedThisRound = 20;
	public int enemiesSpawned = 0;

	private float enemyHealthMultiplier = 1;

	public List<GameObject> EligableEnemies = new List<GameObject>();
	public List<GameObject> CurrentSpawnedEnemies = new List<GameObject>();

	public GameObject gmObject;
	public GameManagerScript gm;

	// Start is called before the first frame update
	void Start()
	{
		gmObject = GameObject.FindGameObjectWithTag("GameManager");
		gm = gmObject.GetComponent<GameManagerScript>();

		//if this player is not master client and is still connected to the server, then add the enemyscript on its own
		if (gm.netGm.isMasterClient == false && gm.netGm.isConnected == true)
		{
			gm.enemySpawnerObj = this.gameObject;
			gm.enemySpawner = this;
		}
			
	}

	// Update is called once per frame
	void Update()
	{
		
	}


	//subscribe to all the events
	private void OnEnable()
	{
		//MapCreationScript.OnFinishedMap += StartSpawning;
	}

	//unsubscribe from all the events
	private void OnDisable()
	{
		//MapCreationScript.OnFinishedMap -= StartSpawning;
	}

	IEnumerator SpawnEnemies()
	{
		Debug.Log("Started Spawning Enemies");

		//spawn all the enemies
		while (enemiesSpawned < totalEnemiesAllowedThisRound)
		{
			int enemyNum = Random.Range(0, EligableEnemies.Count);
			//now start spawning the things. but you will need prefabs in the list
			//GameObject enemy = (GameObject)Instantiate(EligableEnemies[enemyNum], transform.position, Quaternion.identity);
			GameObject enemy = gm.netGm.InstantiateOverNetwork(EligableEnemies[enemyNum], transform.position, EligableEnemies[enemyNum].transform.rotation);
			//add them to the list
			CurrentSpawnedEnemies.Add(enemy);
			enemiesSpawned++;

			//initiate the enemy with everything it needs to know
			EnemyUnitScript enemyScript = enemy.GetComponent<EnemyUnitScript>();
			enemyScript.gm = gm;
			enemy.GetComponent<EnemyUnitScript>().gmObject = gmObject;
			// tell the enemy its ok to path
			if (gm.isMultiplayer && gm.netGm.isMasterClient)
				PhotonView.Get(enemy.GetComponent<EnemyUnitScript>()).RPC("StartPathingRPC", RpcTarget.All);
			else
				enemy.GetComponent<EnemyUnitScript>().StartPathing();
			//enemy.GetComponent<EnemyUnitScript>().StartPathing();

			enemy.GetComponent<EnemyUnitScript>().Health = (int)((float)enemy.GetComponent<EnemyUnitScript>().Health * enemyHealthMultiplier);

			yield return new WaitForSeconds(timeBetweenEnemies / spawnRate);
		}

		//let everyone know that the wave ended
		Debug.Log("Wave Ended");
		if(gm.isMultiplayer == false)
			OnWaveEnded();
		else
		{
			////all this jazz should raise the event with event code 1 or Wave Ended
			//object[] eventContent = new object[] { };
			//RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
			//ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
			//PhotonNetwork.RaiseEvent(GlobalVariables.EventCodes.WaveEnded, eventContent, eventOptions, sendOptions);

			GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.WaveEnded, null);
		}

	}


	public void StartSpawning()
	{
        enemiesSpawned = 0;
        if (gm == null)
		{
			gmObject = GameObject.FindGameObjectWithTag("GameManager");
			gm = gmObject.GetComponent<GameManagerScript>();
		}

		if(gm.WaveNumber > 1 )
		{
			IncreaseDifficulty();
		}

		StartCoroutine("SpawnEnemies");
	}

	public void StopSpawning()
	{
		SpawningEnabled = false;
        enemiesSpawned = 0;

		StopCoroutine("SpawnEnemies");

		//tell all the enemies to stop pathing
		foreach(GameObject enemy in CurrentSpawnedEnemies)
		{
			EnemyUnitScript unitScript = enemy.GetComponent<EnemyUnitScript>();
			unitScript.StopPathing();
		}
	}

    //make a wave ended event
    public delegate void WaveEnded();
    public static event WaveEnded OnWaveEnded;



	private void IncreaseDifficulty()
	{
		Debug.Log("Increased difficulty");
		//minimum spawn rate is 0.8 of a second
		if(timeBetweenEnemies / spawnRate > 0.8)
		{
			//increase the spawn rate by 35 percent
			spawnRate = spawnRate * 1.35f;
		}
		//if the spawn rate is maxed, then start increasing the health of all spawned units
		else
		{
			enemyHealthMultiplier *= 1.15f;
		}
		

		//increase the amount of enemies that can spawn in that wave by 20%
		totalEnemiesAllowedThisRound = (int)(((float)totalEnemiesAllowedThisRound) * 1.2f);


	}
}
