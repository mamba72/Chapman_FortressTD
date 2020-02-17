using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{

    public List<AudioClip> MusicList = new List<AudioClip>();
    public AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        if(MusicList.Count >1)
        {
            //shuffle the songs in the list
            for (int i = 0; i < MusicList.Count; i++)
            {
                AudioClip temp = MusicList[i];
                int randomIndex = Random.Range(i, MusicList.Count);
                MusicList[i] = MusicList[randomIndex];
                MusicList[randomIndex] = temp;
            }
        }
        
        if (tag == "MainCamera")
        {
            PlaySongs();
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySongs()
    {
        if(Settings.GetIsSoundEnabled())
        {
            for (int i = 0; i < MusicList.Count; ++i)
            {
                audiosource.clip = MusicList[i];
                audiosource.Play();
            }
        }
    }
}
