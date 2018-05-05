using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGame : MonoBehaviour
{
    public GameObject PingLabel;
    public static GameObject Player;

    // Use this for initialization
    void Start()
    {
        Player = PhotonNetwork.Instantiate("Character/Attacker", new Vector3(0f, 0.426f, 0f), Quaternion.identity, 0);
        StartDailogue();
    }

    // Update is called once per frame
    void Update()
    {
        PingLabel.GetComponent<Text>().text = "Ping: " + PhotonNetwork.GetPing();
    }

    void StartDailogue()
    {
        string name = "Hi";
        string[] sentences = { "Hello My Name is Fallen Angel", "It Is Awesome Game And Playful!" };

        Dialogue newDialogue = new Dialogue("player", sentences);
        DialogueTrigger.TriggerDialogue(newDialogue);
    }
}