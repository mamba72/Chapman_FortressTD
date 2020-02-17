using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//need this to get the UI to work
using UnityEngine.UI;
using Photon.Pun;

//this script is supposed to be placed on the GameManager Object
public class UIManager : MonoBehaviour
{
    [Header("The Other Scripts to pull info from")]
    //the other managers to show info from
    public GameManagerScript gm;
    public EconomyManagerScript econManager;

    [Header("The parent to all in game UI")]
    public GameObject InGameUI;
    [Header("The parent to all pre game UI")] 
    public GameObject PreGameUI;

    [Header("The General UI")]
    public Text CoinsText;
    public Image CoinsBackground;
    //grab the original color so spamming a building doesnt result in keeping the coins background red
    Color backgroundOrigColor;

    public Button MedBrickTowerButton;
    public Button MedRoundTowerButton;
    public Button MedWatchTowerButton;
    public Button HighlightedButton = null;
    public ColorBlock OriginalColor = ColorBlock.defaultColorBlock;
    public Text LivesText;

    [Header("Building upgrade UI")]
    public GameObject BuildingUpgradeParent;
    public Button UpgradeBuildingButton;
    public Text UpgradeBuildingCostText;

    private Building BuildingToUpgrade = null;

    [Header("Pre Game UI")]
    public Button PlayGameButton;
    public Button RandomizeMapButton;

    [Header("End Game UI")]
    public GameObject GameOverInfo;
    public Text EndRoundText;

    [Header("Wave UI")]
    public Text WaveCountdown;
    public Text ScreenWaveText;


    //event subscriptions
    private void OnEnable()
    {
        EconomyManagerScript.OnNotEnoughMoney += FlashCoinText;
    }

    private void OnDisable()
    {
        EconomyManagerScript.OnNotEnoughMoney -= FlashCoinText;
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundOrigColor = new Color(CoinsBackground.color.r, CoinsBackground.color.g, CoinsBackground.color.b, CoinsBackground.color.a);
        UpdateAllTowerCosts();

        //hide the in game UI and show pre game UI
        InGameUI.SetActive(false);
        PreGameUI.SetActive(true);
        //if this current player is not the master client, then make their buttons non interactable
        if(gm.isMultiplayer == true && gm.netGm.isMasterClient == false)
        {
            PlayGameButton.interactable = false;
            RandomizeMapButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //make sure the coin display up to date
        CoinsText.text = "Coins: " + econManager.TotalCoins;
        CheckBuildingCost();
        //display the wave number on the screen
        if (gm.WaveNumber != 0)
        {
            ScreenWaveText.text = "Wave # " + gm.WaveNumber;
        }

        //if player right clicks
        if(Input.GetMouseButtonDown(1))
        {
            //if there is a building currently selected
            if (gm.SelectedBuilding != null)
            {
                gm.SelectedBuilding = null;

                UnHighlightBuilding();
            }
           
        }
    }

    //this assigns the currently selected building to the game manager 
    public void SelectBuilding(GameObject building)
    {
        gm.SelectedBuilding = building;
    }

    public void HighlightSelectedBuilding(GameObject BuildingButton)
    {
        //grabs the button script on the button
        Button HighlightedBuilding = BuildingButton.GetComponent<Button>();
        if(HighlightedButton != HighlightedBuilding)
        {
            //unhighlight previous selected button
            UnHighlightBuilding();
            //highlight the new building 
            HighlightedButton = HighlightedBuilding;
            OriginalColor = HighlightedBuilding.colors;
            ColorBlock colors = HighlightedBuilding.colors;
            colors.normalColor = Color.yellow;
            HighlightedBuilding.colors = colors;
        }
    }

    public void UnHighlightBuilding()
    {
        if (HighlightedButton != null)
        {
            Debug.Log("unhighlight running");
            HighlightedButton.colors = OriginalColor;
            //set previous values to null
            HighlightedButton = null;
            OriginalColor = ColorBlock.defaultColorBlock;
        }
    }

    public void CheckBuildingCost()
    {
        //MedBrickTowerButton
        if (gm.EconomyManager.TotalCoins < gm.PlaceableBuildings[1].GetComponent<StationaryTowerScript>().BuildingCost)
        {
            MedBrickTowerButton.interactable = false; 
        }
        else
        {
            MedBrickTowerButton.interactable = true;
        }
        if (gm.EconomyManager.TotalCoins < gm.PlaceableBuildings[2].GetComponent<StationaryTowerScript>().BuildingCost)
        {
            MedRoundTowerButton.interactable = false;
        }
        else
        {
            MedRoundTowerButton.interactable = true;
        }
        if (gm.EconomyManager.TotalCoins < gm.PlaceableBuildings[0].GetComponent<StationaryTowerScript>().BuildingCost)
        {
            MedWatchTowerButton.interactable = false;
        }
        else
        {
            MedWatchTowerButton.interactable = true;
        }

    }

    private void UpdateAllTowerCosts()
    {
        MedBrickTowerButton.GetComponentInChildren<Text>().text = "" + (gm.PlaceableBuildings[1].GetComponent<StationaryTowerScript>().BuildingCost);
        MedRoundTowerButton.GetComponentInChildren<Text>().text = "" + (gm.PlaceableBuildings[2].GetComponent<StationaryTowerScript>().BuildingCost);
        MedWatchTowerButton.GetComponentInChildren<Text>().text = "" + (gm.PlaceableBuildings[0].GetComponent<StationaryTowerScript>().BuildingCost);
    }


    public void FlashCoinText()
    {
        StartCoroutine("FlashRed");
    }

    IEnumerator FlashRed()
    {
        //switch the color back and forth a couple times
        for (int i = 0; i < 3; ++i)
        {
            //change it to red
            CoinsBackground.color = new Color(1, 0, 0, 1);
            //wait
            yield return new WaitForSeconds(0.1f);
            //change it back to original color
            CoinsBackground.color = backgroundOrigColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ShowBuildingUpgradeUI(Building building)
    {
        BuildingUpgradeParent.SetActive(true);

        BuildingToUpgrade = building;

        UpgradeBuildingCostText.text = "" + building.UpgradeCost;

        if (building.CanUpgrade())
        {
            UpgradeBuildingButton.interactable = true;
        }
        else
        {
            UpgradeBuildingButton.interactable = false;
        }
    }

    public void HideBuildingUpgradeUI()
    {
        BuildingUpgradeParent.SetActive(false);
        BuildingToUpgrade = null;
    }


    public void ClickedUpgrade()
    {
        
        if(BuildingToUpgrade.CanUpgrade())
        {
            BuildingToUpgrade.UpgradeBuilding();

            //send the index of the building to look at
            int buildingIndex = gm.PlacedDownBuildings.IndexOf(BuildingToUpgrade.gameObject);
            object[] contents = new object[] { buildingIndex };
            
            GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.BuildingUpgraded, contents, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others });
        }


        ShowBuildingUpgradeUI(BuildingToUpgrade);
    }


    public void PlayGame()
    {
        if(gm.isMultiplayer)
        {
            Debug.Log("Called RPC play game");
            gm.PhotonView.RPC("InitiatePlayGame", RpcTarget.All);
        }
        else
        {
            InitiatePlayGame();
        }
        

    }

    public void RandomizeMap()
    {
        if(gm.isMultiplayer)
        {
            gm.PhotonView.RPC("InstantiateRandomizeMap", RpcTarget.All);
        }
        else
        {
            InstantiateRandomizeMap();
        }
    }

    [PunRPC]
    private void InitiatePlayGame()
    {
        PreGameUI.SetActive(false);
        InGameUI.SetActive(true);

        gm.StartGame();
    }

    [PunRPC]
    private void InstantiateRandomizeMap()
    {
        //PreGameUI.SetActive(false);
        //InGameUI.SetActive(true);

        gm.CreateMap();
    }
}
