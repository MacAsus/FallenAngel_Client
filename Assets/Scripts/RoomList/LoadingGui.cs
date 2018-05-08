using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingGui : MonoBehaviour {

	public Slider PercentageSlider;
	public Text PercentageText;
	public Text StatusText;
	public GameObject SliderGameObj;
	public GameObject ScreenGameObj;

	private Vector3 currentPosition;

	void Start() {
		// 다른곳에 가져다가 놨다가 풀링
		currentPosition = ScreenGameObj.transform.localPosition;
		ScreenGameObj.transform.localPosition = new Vector3 (1000, 1000);
	}

	public void SetTextStatus(string text) {
		StatusText.text = text;
	}

	public void SetSliderPercentage(int val) {
		PercentageText.text = val + "%";
		PercentageSlider.value = (float)val / 100;
	}

	public void DisableSlider() {
		SliderGameObj.SetActive(false);
	}

	public void EnableScreen() {
		// 원위치로 가져옴
		ScreenGameObj.transform.localPosition = currentPosition;
	}
}
