using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCharacter : Photon.PunBehaviour
{

    public GameObject AttackerPanel;
    public GameObject TankerPanel;
    public GameObject HealerPanel;
    public GameObject HeavyPanel;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnStartEvent;
        _InitSelectableCharacter(); // 화면 초기화시 가능한 캐릭터 고를 수 있도록
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= this.OnStartEvent;
    }

    void _InitSelectableCharacter()
    {
        AttackerPanel.GetComponent<Image>().color = Colors.PureBLACK;
        TankerPanel.GetComponent<Image>().color = Colors.PureBLACK;
        HealerPanel.GetComponent<Image>().color = Colors.PureBLACK;
        HeavyPanel.GetComponent<Image>().color = Colors.PureBLACK;
        
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey("job") && !string.IsNullOrEmpty((string)player.CustomProperties["job"]))
            {

                string job = (string)player.CustomProperties["job"];
                switch (job)
                {
                    case Util.S_ATTACKER:
                        AttackerPanel.GetComponent<Image>().color = Colors.PureRED;
                        break;
                    case Util.S_TANKER:
                        TankerPanel.GetComponent<Image>().color = Colors.PureRED;
                        break;
                    case Util.S_HEAVY:
                        HealerPanel.GetComponent<Image>().color = Colors.PureRED;
                        break;
                    case Util.S_HEALER:
                        HeavyPanel.GetComponent<Image>().color = Colors.PureRED;
                        break;
                }
            }
        }
    }

    // OnEvent (다른 사람이 고를 때)
    private void OnStartEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == Events.SOMEONE_SET_PLAYER_CUSTOM_PROPERTIES_EVT)
        {
            Debug.Log("다른 사람이 선택!");
            _InitSelectableCharacter();
        }
    }

    public void ChoiceAttacker()
    {
        if(IsAnotherPersonSelected(Util.S_ATTACKER)) {
            ShowPleaseSelectAnotherJobAlert();
            return;
        }
        SendCharacterSelectedMsg(Util.S_ATTACKER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceTanker()
    {
        if(IsAnotherPersonSelected(Util.S_TANKER)) {
            ShowPleaseSelectAnotherJobAlert();
            return;
        }
        SendCharacterSelectedMsg(Util.S_TANKER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceHealer()
    {
        if(IsAnotherPersonSelected(Util.S_HEALER)) {
            ShowPleaseSelectAnotherJobAlert();
            return;
        }
        SendCharacterSelectedMsg(Util.S_HEALER);
        SceneManager.UnloadSceneAsync("SelectCharacter");
    }

    public void ChoiceHeavy()
    {
        if(IsAnotherPersonSelected(Util.S_HEAVY)) {
            ShowPleaseSelectAnotherJobAlert();
            return;
        }
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

    bool IsAnotherPersonSelected(string _job) {
        bool res = false;
        foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
        {
            if (player.CustomProperties.ContainsKey("job") && !string.IsNullOrEmpty((string)player.CustomProperties["job"]))
            {
                string job = (string)player.CustomProperties["job"];
                if(_job == job) {
                    res = true;
                }
            }
        }
        return res;
    }

    void ShowPleaseSelectAnotherJobAlert() {
        // Hack: AlertTriggers.TriggerAlert(alert); 을 사용할 수 없음. 첫 번째 Object를 찾기 때문에 Scene hierarchy에서 CharacterSelect가 아니라 앞에 있는 RoomWaiting에서 찾으려고 함
        GameObject[] goArray = SceneManager.GetSceneByName("SelectCharacter").GetRootGameObjects(); 
        foreach(GameObject gameobj in goArray) {
            if(gameobj.name == "GameObject") {
                Alert alert = new Alert("Alert", "Another Player Selected This Job. Please Select Another Job!");
                gameobj.GetComponent<AlertManager>().StartAlert(alert);
            }
        }
    }

}
