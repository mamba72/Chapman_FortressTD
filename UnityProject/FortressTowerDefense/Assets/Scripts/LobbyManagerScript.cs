using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

using Com.MyCompany.FortressTD.NetworkingHelpers;
using HelperClasses;
using System;

public class LobbyManagerScript : MonoBehaviour
{
    bool isMultiplayer = false;

    //UI elements we need
    public GameObject OpenToMultiplayerObj;
    public GameObject PlaySinglePlayerButtonObj;
    public GameObject PlayMultiplayerButtonObj;
    public GameObject QuickCancelButtonObj;
    public Text RoomNumberText;

    public List<Text> PlayerNameTextList = new List<Text>();

    //networking we need
    public GameObject LauncherObj;
    private Launcher launcher;

    public GameObject RoomControllerObj;
    private NetworkingGameManager roomController;

    [Header("Game Options UI")]
    public InputField XDimInput;
    public InputField YDimInput;

    // Start is called before the first frame update
    void Start()
    {
        PlayMultiplayerButtonObj.SetActive(false);
        QuickCancelButtonObj.SetActive(false);
        //grab the launcher script off the launcher object
        launcher = LauncherObj.GetComponent<Launcher>();
        roomController = RoomControllerObj.GetComponent<NetworkingGameManager>();

        DisplayBoardBounds();
        OnDimentionsWereChanged();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //leavae game button kicks the player out to the main menu
    public void QuitButton()
    {
        if(isMultiplayer)
            LeaveLobby();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SinglePlayerButton()
    {
        //SetBoardBounds();
        OnDimentionsWereChanged();
        Settings.SetIsMultiplayer(false);
        SceneManager.LoadScene("Level 1");
    }

    public void PlayMultiplayerButton()
    {
        //SetBoardBounds();
        OnDimentionsWereChanged();
        Settings.SetIsMultiplayer(true);
        roomController.StartGame();
    }

    //opens the lobby to multiplayer and removes the option to do single player
    public void OpenLobby()
    {
        Settings.SetIsMultiplayer(true);
        isMultiplayer = true;

        //change the UI
        PlaySinglePlayerButtonObj.SetActive(false);
        QuickCancelButtonObj.SetActive(true);
        PlayMultiplayerButtonObj.SetActive(false);

        //connect to the server
        launcher.Connect();


    }

    public void LeaveLobby()
    {
        launcher.LeaveRoom();
        launcher.DisconnectFromGame();
    }

    public void CancelRoomCreation()
    {
        QuickCancelButtonObj.SetActive(false);
        launcher.QuickCancel();
    }


    //event subscriptions
    private void OnEnable()
    {
        Launcher.OnHasJoinedRoom += ShowReadyToPlay;
        Launcher.OnSomeoneJoined += ShowJoinedPlayers;
        
    }

    private void OnDisable()
    {
        Launcher.OnHasJoinedRoom -= ShowReadyToPlay;
        Launcher.OnSomeoneJoined -= ShowJoinedPlayers;
    }

    private void ShowReadyToPlay()
    {
        QuickCancelButtonObj.SetActive(false);
        OpenToMultiplayerObj.SetActive(false);

        RoomNumberText.text = "Room Number: " + launcher.CurrentRoomNumber;

        launcher.progressLabel.GetComponentInChildren<Text>().text = "Connected!\nWaiting for enough players";

        if (launcher.isMasterClient == false)
        {
            PlayMultiplayerButtonObj.GetComponent<Button>().interactable = false;
        }
    }

    private void ShowJoinedPlayers(Dictionary<int, Player> Players)
    {
        if (Players.Count < 1)
            return;

        Debug.Log("Found " + Players.Count + " Player(s)");

        //assign all the players to places on the screen
        foreach(int id in Players.Keys)
        {
            PlayerNameTextList[id - 1].text = Players[id].NickName;
        }

        if (Players.Count >= 2)
            EnoughPlayersAreIn();

        if(isMultiplayer == true)
        {
            if(launcher.isMasterClient)
            {
                XDimInput.readOnly = false;
                YDimInput.readOnly = false;
            }
            else
            {
                XDimInput.readOnly = true;
                YDimInput.readOnly = true;
            }
        }
        else
        {
            XDimInput.readOnly = false;
            YDimInput.readOnly = false;
        }
    }

    private void EnoughPlayersAreIn()
    {
        PlayMultiplayerButtonObj.SetActive(true);

        launcher.progressLabel.GetComponentInChildren<Text>().text = "Ready To Play!";
    }

    //this function sets the board bounds to the bounds given in the lobby scene
    public void SetBoardBounds()
    {
        int x = Convert.ToInt32(XDimInput.text);
        int y = Convert.ToInt32(YDimInput.text);
        Settings.SetMapDimentions(x, y);
    }

    //this function will occupy the bounds on the screen to show the currently set bounds
    public void DisplayBoardBounds()
    {
        BoardBounds dimentions = Settings.GetMapDimentions();
        XDimInput.text = "" + dimentions.x;
        YDimInput.text = "" + dimentions.y;
    }

    public void OnDimentionsWereChanged()
    {
        SetBoardBounds();

        if(isMultiplayer == true && launcher.isMasterClient == true)
        {
            BoardBounds bounds = Settings.GetMapDimentions();

            object[] contents = new object[] { bounds.x, bounds.y };
            GlobalVariables.Functions.RaiseEvent(GlobalVariables.EventCodes.GameDimentionsChanged, contents);
        }
    }
}
