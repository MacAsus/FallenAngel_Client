using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionModal : MonoBehaviour
{
    public GameObject OptionModalObj;
    public Dropdown resolutionDropdown;
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    int selectedResolutionIndex;
    bool isFullScreen;

    public void SetVolume(float volume)
    {
        Debug.Log("volume: " + volume);
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool _isFullScreen)
    {
        isFullScreen = _isFullScreen;
    }

    // Use this for initialization
    void Start()
    {
        float a = 0;
        audioMixer.GetFloat("volume", out a);
        Debug.Log("audio: " + a);
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.height &&
            resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
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
		Resolution resolution = resolutions[selectedResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		Screen.fullScreen = isFullScreen;
        OptionModalObj.SetActive(false);
    }

	
}
