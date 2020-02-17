using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;
using System;

//this script is the parent of all buildings
public abstract class Building : MonoBehaviour
{
    public PrefabInfo PrefabInfo;
    //private vars
    public GameObject RangeCircle;

    //public member variables
    public GameManagerScript gm;
    public string Name;
    public Coordinates coordinates;
    public int BuildingCost;
    public int Range;
    public GameObject target;

    public float damageModifier = 1;
    //list of the enemies in the range of a tower
    //public List<GameObject> NearbyEnemies = new List<GameObject>();

    public GameObject LightObject;

    public float timeBetweenShots = 0.5f;

    //upgrading info
    public int upgradeLevel = 0;
    public List<Rigidbody> Projectiles = new List<Rigidbody>();
    public int UpgradeCost = 25;

    //public bool PlaceBuilding(GameObject groundTile)
    //{
    //    GroundTileScript tileScript = groundTile.GetComponent<GroundTileScript>();

    //    coordinates = tileScript.coordinates;




    //    return true;
    //}


    // Start is called before the first frame update
    public void Start()
    {
        PrefabInfo.Category = "Buildings";

        gm = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //Automatically add this building to the game manager's list of buildings
        gm.PlacedDownBuildings.Add(this.gameObject);

        //function that scales the range of the buildings 
        ScaleRangeCircle();
        HideRangeCircle();

        StartCoroutine("CheckForEnemiesTask");
    }

    // Update is called once per frame
    protected void Update()
    {
        //CheckNearbyEnemies();
        //if(NearbyEnemies.Count != 0)
        //{
        //    ShootTarget();
        //}
    }

    //public abstract void Shoot();

    //building placed event 
    public delegate void BuildingPlaced(object sender, EventArgs b, int buildingCost);
    public static event BuildingPlaced OnBuildingPlaced;

    //scales the range of the buildings
    public void ScaleRangeCircle()
    {
        Debug.Log("range: " + Range);
        RangeCircle.transform.localScale += new Vector3(Range +2.3f, 0, Range+2.3f);
        //RangeCircle.transform.position.Scale(new Vector3(Range, 1, Range));
    }

    //this is the function to be called to place a building on the given transform
    private static GameObject PlaceBuilding(GameObject building, Transform transform, GameManagerScript gm)
    {

        //get the location of the placement tile, but not its exact transform
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //GameObject placedBuilding = (GameObject)Instantiate(building, transform);
        GameObject placedBuilding = gm.netGm.InstantiateOverNetwork(building, newPos, Quaternion.identity);
        //gm.PlacedDownBuildings.Add(placedBuilding);
        //spend the coins
        //gm.EconomyManager.SpendCoins(building.GetComponent<Building>().BuildingCost);
        return placedBuilding;
    }


    //this script will have to place the building, then notify the other user(s). if the user tries to place a building but doesnt have the cash to do so, then it will return null
    public static GameObject LocalPlaceBuilding(GameObject building, Transform tileTransform, GameManagerScript gm)
    {
        int cost = building.GetComponent<Building>().BuildingCost;
        if (gm.EconomyManager.SpendCoins(cost))
        {
            
            //place the building
            GameObject placedBuilding = PlaceBuilding(building, tileTransform, gm);

            GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.SpentCoins, new object[] { cost }, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others });

            //then notify the other users


            //then return the original building object
            return placedBuilding;
        }
        return null;
    }

    public void CheckNearbyEnemies()
    {
        float MaxX = -100;
        //make sure the overall enemy list is not empty
        if(gm.enemySpawner.CurrentSpawnedEnemies.Count == 0)
        {
            return;
        }
        for(int i = 0; i<gm.enemySpawner.CurrentSpawnedEnemies.Count; ++i)
        {
            try
            {
                GameObject enemy = gm.enemySpawner.CurrentSpawnedEnemies[i];
                //find the distance between the enemy and the tower
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= Range)
                {
                    //add the enemy to the in range list
                    //NearbyEnemies.Add(enemy);
                    //set the enemy furthest along the path to the target
                    if (enemy.transform.position.x > MaxX)
                    {
                        target = enemy;
                        MaxX = enemy.transform.position.x;
                    }
                }
            }
            catch(MissingReferenceException e)
            {
                Debug.Log("That target was already destroyed. Skipping to the next one and removing it from the list");
                continue;
            }
            
        }
        if(target != null)
        {
            ShootTarget();
        }
    }

    IEnumerator CheckForEnemiesTask()
    {
        while(true)
        {
            CheckNearbyEnemies();
            yield return new WaitForSeconds(timeBetweenShots);
        }
        
    }

    public abstract void ShootTarget();

    

    public void ShowRangeCircle()
    {
        RangeCircle.SetActive(true);
    }

    public void HideRangeCircle()
    {
        RangeCircle.SetActive(false);
    }

    //this function will upgrade the building to fire a different projectile
    public void UpgradeBuilding(bool ignoreCost = false)
    {
        if(CanUpgrade())
        {
            //change the upgrade level (which changes the projectile fired)
            upgradeLevel++;
            
            //ignore cost is only used for the event
            if(ignoreCost == false)
            {
                //spend the coins to upgrade
                gm.EconomyManager.SpendCoins(UpgradeCost);

                if (gm.isMultiplayer)
                    GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.SpentCoins, new object[] { UpgradeCost }, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others });
            }
            
            //make it harder to upgrade
            UpgradeCost *= 2;
        }


    }

    //returns whether the building has more upgrading to do in the first place
    public bool CanUpgrade()
    {
        if (upgradeLevel != Projectiles.Count - 1 && UpgradeCost <= gm.EconomyManager.TotalCoins)
        {
            return true;
        }
        else
            return false;
    }
}
