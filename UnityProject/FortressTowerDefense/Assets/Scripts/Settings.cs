using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


using HelperClasses;
//bridge between game and files
public class Settings : MonoBehaviour
{

    #region Private Constants


    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";
    const string highestWavePrefKey = "HighestWave";
    const string totalWavePrefKey = "TotalWaves";
    const string totalEnemiesKilledPrefKey = "TotalEnemiesKilled";
    const string isMultiplayerPrefKey = "IsMultiplayer";
    const string isSoundEnabledPrefKey = "IsSoundEnabled";

    const string MapDimentionsXPrefKey = "MapDimentionsX";
    const string MapDimentionsYPrefKey = "MapDimentionsY";

    #endregion

    //returns a boolean representing whether the user name exists
    public static bool HasUsername()
    {
        return PlayerPrefs.HasKey(playerNamePrefKey);
    }

    //returns the actual username in file. if they dont have one, it will return an empty string
    public static string GetUsername()
    {
        return PlayerPrefs.GetString(playerNamePrefKey, string.Empty);
    }

    //sets the username to whatever is given as long as given is not empty
    public static void SetUsername(string newName)
    {
        newName = newName.Trim();
        if (newName.Equals(string.Empty))
            return;

        PlayerPrefs.SetString(playerNamePrefKey, newName);
    }

	#region Leaderboard Methods

	public static bool HasTotalWaves()
    {
        return PlayerPrefs.HasKey(totalWavePrefKey);
    }

    public static void AddWaves(int waveCount)
    {
        int amount = GetTotalWaves();
        amount += waveCount;

        PlayerPrefs.SetInt(totalWavePrefKey, amount);
    }

    public static int GetTotalWaves()
    {
        return PlayerPrefs.GetInt(totalWavePrefKey, 0);
    }

    public static int GetHighestWaves()
    {
        return PlayerPrefs.GetInt(highestWavePrefKey, 0);
    }

    public static void SetHighestWaves(int waveCount)
    {
        if(waveCount > GetHighestWaves())
        {
            PlayerPrefs.SetInt(highestWavePrefKey, waveCount);
        }
    }

    public static int GetNumberOfEnemiesKilled()
    {
        return PlayerPrefs.GetInt(totalEnemiesKilledPrefKey, 0);
    }

    public static void AddToKilledEnemies()
    {
        int total = GetNumberOfEnemiesKilled();
        total++;

        PlayerPrefs.SetInt(totalEnemiesKilledPrefKey, total);
    }

    //gets the board bounds. returns a board bounds struct with the x and y of the saved values
    public static BoardBounds GetMapDimentions()
    {
        BoardBounds bounds = new BoardBounds();
        bounds.x = PlayerPrefs.GetInt(MapDimentionsXPrefKey, 20);
        bounds.y = PlayerPrefs.GetInt(MapDimentionsYPrefKey, 20);

        return bounds;
    }

    //minimum values of x and y are 5
    public static void SetMapDimentions(int x, int y)
    {
        if (x < 5)
            x = 5;
        if (y < 5)
            y = 5;

        PlayerPrefs.SetInt(MapDimentionsXPrefKey, x);
        PlayerPrefs.SetInt(MapDimentionsYPrefKey, y);
    }

	#endregion

	#region Multiplayer Methods
	public static bool GetIsMultiplayer()
    {
        int isMultNum = PlayerPrefs.GetInt(isMultiplayerPrefKey);

        if(isMultNum == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SetIsMultiplayer(bool isMult)
    {
        int multNum;

        if (isMult)
            multNum = 1;
        else
            multNum = 0;

        PlayerPrefs.SetInt(isMultiplayerPrefKey, multNum);
    }

    #endregion

    //returns whether the user has their sound effects enabled. If they never set one, they are enabled by default
    public static bool GetIsSoundEnabled()
    {
        return PlayerPrefs.GetInt(isSoundEnabledPrefKey, 1) == 1;
    }

    //this is the function to change whether the sound effects are enabled
    public static void SetIsSoundEnabled(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt(isSoundEnabledPrefKey, 1);
        else
            PlayerPrefs.SetInt(isSoundEnabledPrefKey, 0);
    }


    private void OnApplicationQuit()
    {
        SetIsMultiplayer(false);
    }



    public InputField UserNameInput;
    public Toggle SoundEffectsToggle;


  



    // Start is called before the first frame update
    void Start()
    {
        ReadSettingsFromFile();
    }

    public void BackButton()
    {
        WriteSettingsToFile();
        SceneManager.LoadScene("MainMenuScene");
    }

    //this function reads all the settings from file and puts them in their correct positions on screen
    private void ReadSettingsFromFile()
    {
        //read in the user info
        if(HasUsername())
            UserNameInput.placeholder.GetComponent<Text>().text = GetUsername();

        SoundEffectsToggle.isOn = GetIsSoundEnabled();


    }

    public void WriteSettingsToFile()
    {
        //player prefs
        SetUsername(UserNameInput.text);

        SetIsSoundEnabled(SoundEffectsToggle.isOn);

    }

}
