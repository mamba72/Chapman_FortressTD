using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("All the UI for the scene")]
    public Text HighestWaveText;
    public Text TotalWavesText;
    public Text TotalEnemiesKilledText;

    // Start is called before the first frame update
    void Start()
    {
        HighestWaveText.text += "" + Settings.GetHighestWaves();
        TotalWavesText.text += "" + Settings.GetTotalWaves();
        TotalEnemiesKilledText.text += "" + Settings.GetNumberOfEnemiesKilled();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Quit()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
