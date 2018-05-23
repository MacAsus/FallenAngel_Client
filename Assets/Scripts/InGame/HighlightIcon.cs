using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightIcon : MonoBehaviour
{
    private Canvas canvas;

    [SerializeField]
    public PhotonVoiceRecorder recorder;

    [SerializeField]
    private PhotonVoiceSpeaker speaker;

    [SerializeField]
    public Image recorderSprite;

    [SerializeField]
    private Image speakerSprite;

    [SerializeField]
    private Text bufferLagText;

    private bool showSpeakerLag;

    // Use this for initialization
    void Start()
    {

    }

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null) { canvas.worldCamera = Camera.main; }
    }


    // Update is called once per frame
    private void Update()
    {
        // recorderSprite.enabled = (recorder != null && recorder.IsTransmitting &&
                // PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined);
        bufferLagText.enabled = showSpeakerLag && speaker.IsPlaying && speaker.IsVoiceLinked;
        bufferLagText.text = string.Format("{0}", speaker.CurrentBufferLag);

		Debug.Log("recorder.IsTransmitting is" + recorder.IsTransmitting);
    }

    private void LateUpdate()
    {
        if (canvas == null || canvas.worldCamera == null) { return; } // should not happen, throw error
        transform.rotation = Quaternion.Euler(0f, canvas.worldCamera.transform.eulerAngles.y, 0f); //canvas.worldCamera.transform.rotation;
    }
}
