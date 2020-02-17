using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
	public PrefabInfo PrefabInfo;

	public bool SpawningEnabled = false;

	public float spawnRate = 1;
	public float timeBetweenEnemies = 4;
	public int totalEnemiesAllowedThisRound = 20;
	public int enemiesSpawned = 0;

	public List<GameObject> EligableEnemies = new List<GameObject>();
	public List<GameObject> CurrentSpawnedEnemies = new List<GameObject>();

	public GameObject gmObject;
	public GameManagerScript gm;

	// Start is called before the first frame update
	void Start()
	{
		gmObject = GameObject.FindGameObjectWithTag("GameManager");
		gm = gmObject.GetComponent<GameManagerScript>();
	}

	// Update is called once per frame
	void Update()
	{
		
	}


	//subscribe to all the events
	private void OnEnable()
	{
		MapCreationScript.OnFinishedMap += StartSpawning;
	}

	//unsubscribe from all the events
	private void OnDisable()
	{
		MapCreationScript.OnFinishedMap -= StartSpawning;
	}

	IEnumerator SpawnEnemies()
	{
		Debug.Log("Started Spawning Enemies");

		//spawn all the enemies
		while (enemiesSpawned < totalEnemiesAllowedThisRound)
		{
			int enemyNum = Random.Range(0, EligableEnemies.Count);
			//now start spawning the things. but you will need prefabs in the list
			GameObject enemy = (GameObject)Instantiate(EligableEnemies[enemyNum], transform.position, Quaternion.identity);
			//add them to the list
			CurrentSpawnedEnemies.Add(enemy);
			enemiesSpawned++;

			//initiate the enemy with everything it needs to know
			enemy.GetComponent<EnemyUnitScript>().gm = gm;
			enemy.GetComponent<EnemyUnitScript>().gmObject = gmObject;
			// tell the enemy its ok to path
			enemy.GetComponent<EnemyUnitScript>().StartPathing();

			yield return new WaitForSeconds(timeBetweenEnemies / spawnRate);
		}

	}


	public void StartSpawning()
	{
		
		if(gm == null)
		{
			gmObject = GameObject.FindGameObjectWithTag("GameManager");
			gm = gmObject.GetComponent<GameManagerScript>();
		}

		StartCoroutine("SpawnEnemies");
	}

	public void StopSpawning()
	{
		SpawningEnabled = false;

		StopCoroutine("SpawnEnemies");

		//tell all the enemies to stop pathing
		foreach(GameObject enemy in CurrentSpawnedEnemies)
		{
			EnemyUnitScript unitScript = enemy.GetComponent<EnemyUnitScript>();
			unitScript.StopPathing();
		}
	}



}
