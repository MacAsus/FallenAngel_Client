using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPSystem : MonoBehaviour {

    public Text hpText;
    public Slider hpSlider;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetBossHP(int val) {
        hpSlider.value = val;
    }

	public void SetBossText(string s) {
        hpText.text = s;
    }

	public void SetMaxBossHP(int val) {
        hpSlider.maxValue = val;
		hpSlider.minValue = 0;
    }

	public void DisableHpSlider() {
		hpSlider.gameObject.SetActive(false);
	}

	public void EnableHpSlider() {
        hpSlider.gameObject.SetActive(true);
    }
}
