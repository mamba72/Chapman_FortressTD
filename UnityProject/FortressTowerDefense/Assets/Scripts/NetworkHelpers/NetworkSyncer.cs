using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using HelperClasses;

public class NetworkSyncer : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameManagerScript gm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(gm.BoardBounds.x);
            stream.SendNext(gm.BoardBounds.y);
            stream.SendNext(gm.EconomyManager.TotalCoins);
            
            //stream.SendNext(gm.PathTileOrder);

            stream.SendNext(gm.WaveNumber);

            stream.SendNext(gm.NumberOfLives);
        }
        else
        {
            gm.BoardBounds.x = (int)stream.ReceiveNext();
            gm.BoardBounds.y = (int)stream.ReceiveNext();
            gm.EconomyManager.TotalCoins = (int)stream.ReceiveNext();
            //gm.PathTileOrder = (List<GameObject>)stream.ReceiveNext();

            gm.WaveNumber = (int)stream.ReceiveNext();
            gm.NumberOfLives = (int)stream.ReceiveNext();
        }
    }
}
