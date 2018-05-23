using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Demo_Option_SwitchBtn : MonoBehaviour {

    public Sprite[] changeImage;
    private Image m_Slider_BackImg;
    private Slider m_slider;

    void Start()
    {
        m_slider = this.GetComponent<Slider>();
        m_Slider_BackImg = this.gameObject.transform.Find("Back_Image").GetComponent<Image>();
    }

    public void OnSwitchBtn()
    {
        Debug.Log(m_slider.value);

        if (m_slider.value == 0)
        {
            m_Slider_BackImg.sprite = changeImage[0];


        }
        else
        {
            m_Slider_BackImg.sprite = changeImage[1];

        }

    }

}
