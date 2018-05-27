using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionModal : MonoBehaviour
{
    public GameObject OptionModalObj;
    public Dropdown resolutionDropdown;
    public Toggle FullScreenToggle;
    public Dropdown GraphicDropdown;
    public Toggle VoiceChatToggle;
    public Toggle SoundToggle;
    public Slider VolumeSlider;
    public PhotonVoiceRecorder rec;
    public PhotonVoiceSpeaker speaker;
    public static bool IsActive = false;

    public AudioMixer audioMixer;
    Resolution[] resolutions;


    int selectedResolutionIndex;
    bool isFullScreen;
    bool isVolumeEnable;
    float volumeLevel;
    int selectedGraphicLevel;
    bool isVoiceChatEnable;


    public void SetVolume(float volume)
    {
        volumeLevel = volume;
    }

    public void SetQuality(int qualityIndex)
    {
        selectedGraphicLevel = qualityIndex;
    }

    public void SetFullScreen(bool _isFullScreen)
    {
        isFullScreen = _isFullScreen;
    }

    public void SetVoiceChat(bool _isVoiceChatEnable) {
        isVoiceChatEnable = _isVoiceChatEnable;
    }

    public void SetVolume(bool _isVolumeEnable) {
        isVolumeEnable = _isVolumeEnable;
    }

    // Use this for initialization
    public void Init()
    {
        InitValue();
        SetResolutionList();
        SetUIbyValue();
    }

    void SetResolutionList() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int selectedResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.height &&
            resolutions[i].height == Screen.currentResolution.height)
            {
                selectedResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
    }

	public void SetResolution (int resolutionIndex) {
        selectedResolutionIndex = resolutionIndex;
    }

    // Update is called once per frame
    void Update()
    {

    }

	// ESC 누르면 이전값으로 돌아감 --> 보관해둔 값으로 변경
	// SaveAndQuit 눌러야 저장 --> userPref에 넣어둠
	public void SaveAndQuit() {
        SaveValueToPlayerPrefs(); // Save To PlayerPrefs
        OptionModalObj.SetActive(false);
    }

    void InitValue() {
        isVolumeEnable = PlayerPrefs.GetInt(Settings.VolumeEnable, 1) != 0;
        isVoiceChatEnable = PlayerPrefs.GetInt(Settings.VoiceChatEnable, 1) != 0;
        isFullScreen = PlayerPrefs.GetInt(Settings.FullScreen, 1) != 0;
        selectedGraphicLevel = PlayerPrefs.GetInt(Settings.GraphicLevel, 5);
        volumeLevel = PlayerPrefs.GetFloat(Settings.VolumeLevel, 0);
        selectedResolutionIndex = PlayerPrefs.GetInt(Settings.Resolution, 0);
    }

    void SaveValueToPlayerPrefs() {
        PlayerPrefs.SetInt(Settings.VolumeEnable, isVolumeEnable? 1 : 0);
        PlayerPrefs.SetInt(Settings.VoiceChatEnable, isVoiceChatEnable? 1 : 0);
        PlayerPrefs.SetInt(Settings.FullScreen, isFullScreen? 1 : 0);
        PlayerPrefs.SetInt(Settings.GraphicLevel, selectedGraphicLevel);
        PlayerPrefs.SetFloat(Settings.VolumeLevel, volumeLevel);
        PlayerPrefs.SetInt(Settings.Resolution, selectedResolutionIndex);
    }

    void ApplySettings() {
        /*
        int selectedResolutionIndex; V
        bool isFullScreen; V
        bool isVolumeEnable; V
        float volumeLevel; V
        int selectedGraphicLevel; V
        bool isVoiceChatEnable; Need To Be Implemented
        */
        Resolution resolution = resolutions[selectedResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		Screen.fullScreen = isFullScreen;
        audioMixer.SetFloat("volume", volumeLevel);
        QualitySettings.SetQualityLevel(selectedGraphicLevel);

        if(isVolumeEnable) {
            UnMuteVolume();
        } else {
            MuteVolume();
        }

        if(isVoiceChatEnable) {
            InGame.Player.GetComponent<PhotonVoiceSpeaker>().GetComponent<GameObject>().SetActive(true);
            InGame.Player.GetComponent<PhotonVoiceRecorder>().GetComponent<GameObject>().SetActive(true);
        } else {
            InGame.Player.GetComponent<PhotonVoiceSpeaker>().GetComponent<GameObject>().SetActive(false);
            InGame.Player.GetComponent<PhotonVoiceRecorder>().GetComponent<GameObject>().SetActive(false);
        }

        IsActive = false;
    }

    void MuteVolume() {
        AudioListener.pause = true;
        AudioListener.volume = 0;
    }

    void UnMuteVolume() {
        AudioListener.pause = false;
        AudioListener.volume = 1;
    }

    void SetUIbyValue() {
        FullScreenToggle.isOn = isFullScreen;
        SoundToggle.isOn = isVolumeEnable;
        VolumeSlider.value = volumeLevel;
        GraphicDropdown.value = selectedGraphicLevel;
        VoiceChatToggle.isOn = isVoiceChatEnable;
        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

	
}
