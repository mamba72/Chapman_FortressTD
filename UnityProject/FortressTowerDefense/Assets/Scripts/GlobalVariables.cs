using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;

using Photon.Pun;
using Photon.Realtime;

public static class GlobalVariables
{

    public static readonly string SettingsFileName = "Settings.txt";
    public static BoardBounds BoardBounds;

    

    //all the photon event codes need to go in here. they range from 0 to 199
    public static class EventCodes
    {
        //game manager:
        public const byte WaveEnded = 1;
        public const byte GameOver = 5;

        //Buildings
        public const byte BuildingUpgraded = 2;

        //Economy manager
        public const byte SpentCoins = 3;
        public const byte IncreaseCoins = 4;

        //lobby Manager
        public const byte GameDimentionsChanged = 6;
        public const byte PathTileCreated = 7;
    }


    public class Functions
    {
        public static void RaiseEvent(byte eventCode, object[] eventContent, RaiseEventOptions eventOptions = null)
        {
            //all this jazz should raise the event with event code 1 or Wave Ended
            //object[] eventContent = new object[] { };
            if(eventOptions == null)
                eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventCode, eventContent, eventOptions, sendOptions);
        }
    }
}
