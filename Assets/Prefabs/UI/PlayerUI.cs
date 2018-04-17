using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Tooltip("UI Text to display Player's Name")]
    public Text PlayerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    public Slider PlayerHealthSlider;

    [Tooltip("Pixel offset from the player target")]
    public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

    Player _target;
    float _characterControllerHeight = 0f;
    Transform _targetTransform;
    Vector3 _targetPosition;
    // Use this for initialization
    void Start()
    {
        // Debug.Log("PlayerUI Started");
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (_target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        
        // Reflect the Player Health
        if (PlayerHealthSlider != null)
        {
            PlayerHealthSlider.value = _target.n_hp;
            // Debug.Log("현재 체력" + _target.n_hp);
        }
    }

    void Awake()
    {
        // Debug.Log("Player UI Awake");
        this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
    }

    void LateUpdate()
    {
        // #Critical
        // Follow the Target GameObject on screen.
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;
            _targetPosition.y += _characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + ScreenOffset;
        }
    }

    public void SetTarget(Player target)
    {
        if (target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        _target = target;
        if (PlayerNameText != null)
        {
            PlayerNameText.text = _target.photonView.ownerId + "";
            // Debug.Log("_target.photonView.ownerId: " + _target.photonView.ownerId);
        }

        CharacterController _characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            _characterControllerHeight = _characterController.height;
        }

        _targetTransform = _target.transform;
        PlayerHealthSlider.minValue = 0;
        PlayerHealthSlider.maxValue = _target.n_hp;
    }

}
