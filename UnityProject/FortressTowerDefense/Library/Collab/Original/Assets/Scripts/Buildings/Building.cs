using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;
using System;

//this script is the parent of all buildings
public abstract class Building : MonoBehaviour
{
    //private vars

    //public member variables
    public GameManagerScript gm;
    public string Name;
    public Coordinates coordinates;
    public int BuildingCost;
    public int Range;
    public GameObject target;
    //list of the enemies in the range of a tower
    public List<GameObject> NearbyEnemies = new List<GameObject>();

    public GameObject LightObject;
    
  
    //public bool PlaceBuilding(GameObject groundTile)
    //{
    //    GroundTileScript tileScript = groundTile.GetComponent<GroundTileScript>();

    //    coordinates = tileScript.coordinates;




    //    return true;
    //}


    // Start is called before the first frame update
    public void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public abstract void Shoot();

    //building placed event 
    public delegate void BuildingPlaced(object sender, EventArgs b, int buildingCost);
    public static event BuildingPlaced OnBuildingPlaced;


    //this is the function to be called to place a building on the given transform
    private static GameObject PlaceBuilding(GameObject building, Transform transform, GameManagerScript gm)
    {
        GameObject placedBuilding = (GameObject)Instantiate(building, transform);
        gm.PlacedDownBuildings.Add(placedBuilding);
        //spend the coins
        gm.EconomyManager.SpendCoins(building.GetComponent<Building>().BuildingCost);
        return placedBuilding;
    }


    //this script will have to place the building, then notify the other user(s) 
    public static GameObject LocalPlaceBuilding(GameObject building, Transform tileTransform, GameManagerScript gm)
    {
        //place the building
        GameObject placedBuilding = PlaceBuilding(building, tileTransform, gm);

        //then notify the other users


        //then return the original building object
        return placedBuilding;
    }

    public void CheckNearbyEnemies()
    {
        float MaxX = -100;
        //make sure the overall enemy list is not empty
        if(gm.AllEnemies.Count == 0)
        {
            return;
        }
        for(int i = 0; i<gm.AllEnemies.Count; ++i)
        {
            GameObject enemy = gm.AllEnemies[i];
            //find the distance between the enemy and the tower
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance <= Range)
            {
                //add the enemy to the in range list
                NearbyEnemies.Add(enemy);
                //set the enemy furthest along the path to the target
                if(enemy.transform.position.x > MaxX)
                {
                    target = enemy;
                    MaxX = enemy.transform.position.x;
                }
            }
        }
    }

    public abstract void ShootTarget();

    
}
