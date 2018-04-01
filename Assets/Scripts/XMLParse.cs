using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XMLParse : MonoBehaviour
{

    public string xml_Job = "Job";
    public string xml_Weapon = "Weapon";

	// Use this for initialization
	void Start ()
    {
        LoadXML_Job(xml_Job);
        LoadXML_Weapon(xml_Weapon);
	}
	
    private void LoadXML_Job(string fileName)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + fileName);
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        //하나씩 가져오기
        XmlNodeList xml_Job_Table = xmlDoc.GetElementsByTagName("Name");
        foreach (XmlNode Name in xml_Job_Table)
        {
            Debug.Log("[one by one] Job Name : " + Name.InnerText);
        }

        //전체 가져오기
        XmlNodeList all_xml_Job_Table = xmlDoc.SelectNodes("Job/Character");
        foreach(XmlNode node in all_xml_Job_Table)
        {
            Debug.Log("[all] Job Name : " + node.SelectSingleNode("Name").InnerText);
            Debug.Log("[all] Job Hp : " + node.SelectSingleNode("Hp").InnerText);
            Debug.Log("[all] Job Speed : " + node.SelectSingleNode("Speed").InnerText);
        }
    }

	private void LoadXML_Weapon(string fileName)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + fileName);
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        //하나씩 가져오기
        XmlNodeList xml_Weapon_Table = xmlDoc.GetElementsByTagName("Name");
        foreach (XmlNode Name in xml_Weapon_Table)
        {
            Debug.Log("[one by one] Weapon Name : " + Name.InnerText);
        }
        
        //전체 가져오기
        XmlNodeList all_xml_Weapon_Table = xmlDoc.SelectNodes("Main/Weapon");
        foreach (XmlNode node in all_xml_Weapon_Table)
        {
            Debug.Log("[all] Weapon Name : " + node.SelectSingleNode("Name").InnerText);
            Debug.Log("[all] Weapon Bulletspeed : " + node.SelectSingleNode("Bulletspeed").InnerText);
            Debug.Log("[all] Weapon Damage : " + node.SelectSingleNode("Damage").InnerText);
        }
    }
}