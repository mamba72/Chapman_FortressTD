using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//allows us to access the UI
using UnityEngine.UI;
public class MainMenuScript : MonoBehaviour
{

    //the play button that takes you to the game lobby
    public GameObject PlayButtonObj;
    public GameObject GoMakeUsernameObj;
    public Text WelcomeText;


    // Start is called before the first frame update
    void Start()
    {
        if (Settings.HasUsername() == false)
        {
            PlayButtonObj.SetActive(false);
            GoMakeUsernameObj.SetActive(true);
            //welcome text
            WelcomeText.text = "Welcome New Player!";
        }
        else
        {
            PlayButtonObj.SetActive(true);
            GoMakeUsernameObj.SetActive(false);
            WelcomeText.text = "Welcome Back " + Settings.GetUsername() + "!";
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreditsButton()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void LeaderBoardButton()
    {
        SceneManager.LoadScene("LeaderBoardScene");
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
    }
}
