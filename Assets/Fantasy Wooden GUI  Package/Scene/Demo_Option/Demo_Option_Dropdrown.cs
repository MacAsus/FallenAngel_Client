using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Demo_Option_Dropdrown :  EventTrigger {


    public Sprite[] ArrowImages;
    private Image m_ArrowImage;
    private Dropdown m_Dropdown;
	// Use this for initialization
	void Start () {
        m_ArrowImage = this.transform.Find("Arrow").GetComponent<Image>();
        m_Dropdown = this.GetComponent<Dropdown>();
    }
    public override void OnPointerClick(PointerEventData data)
    {
        m_ArrowImage.sprite = ArrowImages[1];
    }

    public override void OnSelect(BaseEventData data)
    {
        m_ArrowImage.sprite = ArrowImages[0];
    }


}
