using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManagerScript : MonoBehaviour
{
    [Header("Necessary Objects and Components")]
    public GameManagerScript gm;

    [Header("General Variables")]
    public int TotalCoins = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //events
    public delegate void NotEnoughMoney();
    public static NotEnoughMoney OnNotEnoughMoney;


    //getter for total coins
    public int GetTotalCoins()
    {
        return TotalCoins;
    }

    public void IncreaseCoins(int amount)
    {
        if(amount > 0)
        {
            TotalCoins += amount;
        }
            
        else
        {
            Debug.Log("Cant add the following amount of coins because it is less than 1: " + amount);
        }
    }

    public bool SpendCoins(int amount)
    {
        if (amount > 0)
        {
            //if the amount is less than the total coins, subtract the coins
            if(amount <= TotalCoins)
            {
                TotalCoins -= amount;
            }
            else
            {
                //fire the event to let others know you dont have enough money
                OnNotEnoughMoney();
                Debug.Log("Cant subtract the following amount of coins because it is greater than the total amount of coins: " + amount);
                return false;
            }


        }
        else
        {
            Debug.Log("Cant subtract the following amount of coins because it is less than 1: " + amount);
            return false;
        }

        //if it makes it here, the transaction was successful
        return true;
    }

    //subscribe to all the events
    private void OnEnable()
    {
        EnemyUnitScript.OnEnemyDied +=(sender, e, killReward) => CollectKillReward(sender, e, killReward);
        Building.OnBuildingPlaced += (sender, b, buildingCost) => ChargeBuildingCost(sender, b, buildingCost);
    }

    //unsubscribe from all the events
    private void OnDisable()
    {
        EnemyUnitScript.OnEnemyDied -= (sender, e, killReward) => CollectKillReward(sender, e, killReward);
        Building.OnBuildingPlaced -= (sender, b, buildingCost) => ChargeBuildingCost(sender, b, buildingCost);
    }

    //collect money when enemy is killed
    public void CollectKillReward(object sender, EventArgs e, int killReward)
    {
        //increase the users money by the amount that the enemy is worth
        IncreaseCoins(killReward);
    }

    public void ChargeBuildingCost(object sender, EventArgs b, int buildingCost)
    {
        SpendCoins(buildingCost);
    }


}
