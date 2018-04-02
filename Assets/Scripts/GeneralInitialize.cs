using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GeneralInitialize : MonoBehaviour {

    public class GunParameter
    {
        public string s_GunName;
        public float f_BulletSpeed;
        public float f_Damage;
        public string s_BulletName;

        public GunParameter(string gunName, float bulletSpeed, float damage, string bulletName)
        {
            s_GunName = gunName;
            f_BulletSpeed = bulletSpeed;
            f_Damage = damage;
            s_BulletName = bulletName;
        }
    }
    public class JobParameter
    {
        public string s_JobName;
        public float f_HP;
        public float f_Speed;

        public JobParameter(string jobName, float hp, float speed)
        {
            s_JobName = jobName;
            f_HP = hp;
            f_Speed = speed;
        }
    }

    public static GeneralInitialize instance = null;

    private string xml_Job = "Job";
    private string xml_Weapon = "Weapon";

    public enum JobEnum { Attacker, Tanker, Healer, Heavy };

    public List<GameObject> PlayableCharacterList = new List<GameObject>();
    public List<GunParameter> l_GunList = new List<GunParameter>();
    public List<JobParameter> l_JobList = new List<JobParameter>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance!= this)
        {
            Destroy(gameObject);
        }
        loadXML_Job(xml_Job);
    }

    private void loadXML_Job(string filename)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + filename);
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(txtAsset.text);
        XmlNodeList all_xml_Job_Table = xmlDoc.SelectNodes("Job/Character");
        foreach(XmlNode node in all_xml_Job_Table)
        {
            JobParameter temp = new JobParameter(node.SelectSingleNode("Name").InnerText, System.Convert.ToSingle(node.SelectSingleNode("Hp").InnerText), System.Convert.ToSingle(node.SelectSingleNode("Speed").InnerText));
            l_JobList.Add(temp);
        }
        loadCharacterList();

    }
    private void loadCharacterList()
    {
        foreach(var jparam in l_JobList)
        {
            GameObject tempObj;

            if(Resources.Load("Character/" + jparam.s_JobName) != null)
            {
                Debug.Log(jparam.s_JobName + " Created");
                tempObj = Resources.Load("Character/" + jparam.s_JobName) as GameObject;
                tempObj.GetComponent<Player>().n_hp = jparam.f_HP;
                tempObj.GetComponent<Player>().f_Speed = jparam.f_Speed;
                PlayableCharacterList.Add(tempObj);
            }
        }
    } 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
