using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{

    public GameObject Attacker;
    public GameObject Tanker;
    public GameObject Healer;
    public GameObject Heavy;

    // Use this for initialization
    void Start()
    {
        _InitSelectableCharacter(); // 화면 초기화시 가능한 캐릭터 고를 수 있도록
    }

    // Update is called once per frame
    void Update()
    {

    }
    // OnEvent (다른 사람이 고를 때)

    void _InitSelectableCharacter()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey("job") && !string.IsNullOrEmpty((string)player.CustomProperties["job"]))
            {

                string job = (string)player.CustomProperties["job"];
                switch (job)
                {
                    case Util.S_ATTACKER:
                        break;
                    case Util.S_TANKER:
                        break;
                    case Util.S_HEAVY:
                        break;
                    case Util.S_HEALER:
                        break;
                }
            }
        }
    }

    public void ChoiceAttacker()
    {
        SendCharacterSelectedMsg(Util.S_ATTACKER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceTanker()
    {
        SendCharacterSelectedMsg(Util.S_TANKER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceHealer()
    {
        SendCharacterSelectedMsg(Util.S_HEALER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceHeavy()
    {
        SendCharacterSelectedMsg(Util.S_HEAVY);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    private void SendCharacterSelectedMsg(string job)
    {
        byte evtCode = Events.SOMEONE_SELECTED_CHARACTER_EVT;    // Someone Selected Character & Weapon
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        bool reliable = true;

        PhotonNetwork.RaiseEvent(evtCode, job, reliable, options);
        Debug.Log("SendCharacterSelectedMsg called");
    }

}
