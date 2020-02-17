using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperClasses;

public class MapCreationScript : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject groundTilePrefab;
	public GameObject pathTilePrefab;
	public GameObject EnemySpawnerPrefab;
	public GameObject EnemyDestroyerPrefab;
    public GameObject castlePrefab;
	private GameObject castleObject = null;

	[Header("GameObjects")]
	public GameManagerScript gm;
	public GameObject MapObject;

	public float tileWidth;

	[Header("Map Creation Coordinates")]
	//things to help with the path creation
	int startX;
	int startY;
	int endX;
	int endY;

	int maxPathLength;

	//private shit
	bool CurrentlyGeneratingMap = false;

	// Start is called before the first frame update
	void Start()
	{
		//gm = gmObject.GetComponent<GameManagerScript>();
		tileWidth = groundTilePrefab.transform.localScale.x;
		maxPathLength = (gm.BoardBounds.x + gm.BoardBounds.y) * 2;//just for testing?


		////if we are the master client, then run the map creation script
		//if (gm.netGm.isMasterClient == true)
		//{
		//	Debug.Log("We are master client. Building map...");
		//	//start the map generation
		//	Invoke("CreateMap", 1);
		//}
		//else
		//{
		//	Debug.Log("We are not master client, not creating map");
		//}

	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void CreateMap()
	{
		if(gm.isMultiplayer == true)
		{
			if (gm.netGm.isMasterClient == false || gm.netGm.isConnected == false)
			{
				Debug.Log("We are not master client, not creating map");
				return;
			}
		}
		

		int enterTilePos = Random.Range(1, gm.BoardBounds.y - 1);
		//Debug.Log("Position of enter point: " + enterTilePos);

		int exitTilePos = Random.Range(1, gm.BoardBounds.y - 1);
		//Debug.Log("Position of exit point: " + exitTilePos);

		//place all the rows
		for (int row = 0; row < gm.BoardBounds.x; ++row)
		{
			//instantiate the object
			for(int column = 0; column < gm.BoardBounds.y; ++column)
			{
				GameObject square;
				//if it is on the starter point, then make a path rather than a ground tile
				if (row == enterTilePos && column == 0)
				{
					//create the start tile
					square = InstantiateTile(pathTilePrefab, new Vector3(column, 0, row), Quaternion.identity);
					//add the start tile to the game manager
					gm.StartTile = square;
					gm.PathTileOrder.Add(square);
					square.name = "Path_" + row + "_" + column;

					startY = row;
					startX = column;

					//create the spawner and tell the game manager about it
					//the column - 1 is to offset it off of the map to the left of the starter block
					gm.enemySpawnerObj = InstantiateTile(EnemySpawnerPrefab, new Vector3(column - 1, 0, row), Quaternion.identity);
					gm.enemySpawner = gm.enemySpawnerObj.GetComponent<EnemySpawnerScript>();
				}
				else if(row == exitTilePos && column == gm.BoardBounds.y - 1)
				{
					//create the end tile
					square = InstantiateTile(pathTilePrefab, new Vector3(column, 0, row), Quaternion.identity);
					gm.EndTile = square;
                    castleObject = InstantiateTile(castlePrefab, new Vector3(gm.EndTile.transform.position.x + 46.3f , 0, gm.EndTile.transform.position.z - 62), castlePrefab.transform.rotation);
                    castleObject.SetActive(true);
                    square.name = "Path_" + row + "_" + column;

					endY = row;
					endX = column;

					//create the goal and tell the game manager about it
					//the column + 1 is to offset it off of the map to the right of the ending block
					gm.enemyGoalObj = InstantiateTile(EnemyDestroyerPrefab, new Vector3(column + 1, 0, row), Quaternion.identity);
					gm.enemyGoal = gm.enemyGoalObj.GetComponent<EnemyGoalScript>();
				}
				else
				{
					//if its not the start or end point, make it a grass tile
					square = InstantiateTile(groundTilePrefab, new Vector3(column, 0, row), Quaternion.identity);
					square.name = "Ground_" + row + "_" + column;
				}


				//square = (GameObject)Instantiate(groundTilePrefab, new Vector3(column, 0, row), Quaternion.identity);

				

				//tell that ground tile what its x and y are
				square.GetComponentInChildren<GroundTileScript>().coordinates.x = row;
				square.GetComponentInChildren<GroundTileScript>().coordinates.y = column;

				square.transform.SetParent(MapObject.transform);


				object[] squareName = new object[] { square.name };
				GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.PathTileCreated, squareName, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others });

				gm.TileArray[column, row] = square;
			}
		}
		//next, create a random path from the start to the end point
		CreatePath();

		//map is complete fire the event letting all the other scripts know
		//OnFinishedMap();
	}


	private void CreatePath()
	{
		bool reachedEnd = false;
		GameObject prevTile = null;
		int currX = startX;
		int currY = startY;
		int pathLength = 0;
		while(reachedEnd == false)
		{

			pathLength++;

			//if the path length is too much, then break
			if (pathLength >= maxPathLength)
				break;

			//0 is up, 1 is right, 2 is down
			int direction = Random.Range(0, 3);
			int offsetX = 0;
			int offsetY = 0;
			switch(direction)
			{
				case 0:
					offsetY = 1;
					break;
				case 1:
					offsetX = 1;
					break;
				case 2:
					offsetY = -1;
					break;
			}

			//Debug.Log("NextTile coords: " + (int)(currX + offsetX) + ", " + (int)(currY + offsetY));

			//if the offset makes the index become out of range, re randomize
			if (currX + offsetX >= gm.BoardBounds.x || currY + offsetY >= gm.BoardBounds.y)
			{
				//Debug.Log("Either x or y were too large");
				continue;
			}
				
			//if the offset makes the index less than zero, re randomize
			if (currX + offsetX < 0 || currY + offsetY < 0)
			{
				//Debug.Log("Either x or y were less than 0");
				continue;
			}

			GameObject nextTile = gm.TileArray[currX + offsetX, currY + offsetY];

			//int nextX = currX + offsetX;
			//int nextY = currY + offsetY;
			
			//if()
			//{

			//}

			//check to see if the next tile is the ending tile. if so, then end the while loop
			if(nextTile == gm.EndTile)
			{
				//Debug.Log("Reached End");
				reachedEnd = true;
				break;
			}

			GroundTileScript nextTileScript = nextTile.GetComponentInChildren<GroundTileScript>();
			//if the next tile it's looking at is already a path, then just re randomize
			if (nextTileScript.isPath == true)
			{
				//Debug.Log("That tile is already a path");
				continue;
			}

			//if it makes it here, then the nex tile should be a valid tile
			//convert this next tile to a path tile
			

			//swap over to the path prefab
			prevTile = nextTile;
			//Debug.Log("Previous Tile Name: " + prevTile.name);
			string oldName = prevTile.name;
			//create the new tile
			nextTile = InstantiateTile(gm.pathTilePrefab, prevTile.transform.position, Quaternion.identity);
			//assign its parent
			nextTile.transform.SetParent(MapObject.transform);
			//tell that ground tile what its x and y are
			nextTile.GetComponentInChildren<GroundTileScript>().coordinates.x = currX + offsetX;
			nextTile.GetComponentInChildren<GroundTileScript>().coordinates.y = currY + offsetY;

			

			gm.TileArray[currX + offsetX, currY + offsetY] = nextTile;
			//ensure it has the correct name
			nextTile.name = "Path_" + (int)(currX + offsetX) + "_" + (int)(currY + offsetY);

			//done changing that tile to a path tile. now adjust the coordinates as needed
			currX = currX + offsetX;
			currY = currY + offsetY;
			prevTile = nextTile;

			//now add that tile to the PathTileOrder list
			gm.PathTileOrder.Add(nextTile);

			//remove the previous tile
			//if(prevTile.GetComponentInChildren<GroundTileScript>().isPath == false)
			//{
			//	prevTile.SetActive(false);
			//	Destroy(prevTile);

			//}

			GameObject oldTile = GameObject.Find(oldName);
			oldTile.SetActive(false);
			//Destroy(oldTile);
			gm.netGm.DestroyOverNetwork(oldTile);
				
			//if(pathLength >= 40)
				//reachedEnd = true;
		}



		//vadd the ending tile to the tile list in gm
		gm.PathTileOrder.Add(gm.EndTile);
		//add the goal tile to the path tile order as the last tile
		gm.PathTileOrder.Add(gm.enemyGoalObj);
	}


	private GameObject InstantiateTile(GameObject obj, Vector3 position, Quaternion quaternion)
	{
		//GameObject newTile = (GameObject)Instantiate(obj, position, quaternion);

		GameObject newTile = gm.netGm.InstantiateOverNetwork(obj, position, quaternion);

		return newTile;
	}


	public void StartMapCreation()
	{
		if(CurrentlyGeneratingMap == false)
		{
			//clear the map
			for (int x = 0; x < gm.BoardBounds.x; ++x)
			{
				for (int y = 0; y < gm.BoardBounds.y; ++y)
				{
					GameObject obj = gm.TileArray[x, y];
					if (obj != null)
						gm.netGm.DestroyOverNetwork(obj);
				}
			}

			if (gm.enemySpawnerObj != null)
				gm.netGm.DestroyOverNetwork(gm.enemySpawnerObj);
			if (gm.enemyGoalObj != null)
				gm.netGm.DestroyOverNetwork(gm.enemyGoalObj);
			if (castleObject != null)
				gm.netGm.DestroyOverNetwork(castleObject);

			//clear the lists and arrays of the tiles
			gm.PathTileOrder = new List<GameObject>();
			gm.TileArray = new GameObject[gm.BoardBounds.x, gm.BoardBounds.y];


			//Invoke("CreateMap", 1);
			StartCoroutine("MapCreationCoroutine");
		}
	}


	//events
	public delegate void MapFinished();
	public static event MapFinished OnFinishedMap;


	#region Coroutines that should help performance

	IEnumerator MapCreationCoroutine()
	{
		CurrentlyGeneratingMap = true;
		yield return new WaitForSeconds(0.5f);
		CreateMap();
		CurrentlyGeneratingMap = false;
	}

	#endregion

}
