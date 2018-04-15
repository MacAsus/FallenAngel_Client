using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkUtil {
    public static List<GameObject> PlayerList = new List<GameObject>();

    public static void SetPlayer() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            PlayerList.Add(player);
        }
    }    
}