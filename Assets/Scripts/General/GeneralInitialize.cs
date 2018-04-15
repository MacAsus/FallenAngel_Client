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
        public float f_Magazine;
        public string tag;
        public Sprite GunImage;
        public Sprite BulletImage;

        public GunParameter(string gunName, float bulletSpeed, float damage, string bulletName, float magazine)
        {
            s_GunName = gunName;
            f_BulletSpeed = bulletSpeed;
            f_Damage = damage;
            s_BulletName = bulletName;
            f_Magazine = magazine;
            if(Resources.Load("GunImage/" + s_GunName) != null)
                GunImage = Resources.Load("GunImage/" + s_GunName) as Sprite;
            if (Resources.Load<Sprite>("BulletImage/" + bulletName) != null)
            {
                BulletImage = Resources.Load<Sprite>("BulletImage/" + bulletName);
            }
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
    public class BulletParameter
    {
        public string s_Host;
        public float f_Damage;

        public BulletParameter(string host, float damage)
        {
            s_Host = host;
            f_Damage = damage;
        }
    }


    public static GeneralInitialize instance = null;

    private string xml_Job = "Job";
    private string xml_Weapon = "Weapon";

    public enum JobEnum { Attacker, Tanker, Healer, Heavy };

    public List<GameObject> PlayableCharacterList = new List<GameObject>();
    //public List<GunParameter> l_GunList = new List<GunParameter>();
    public List<JobParameter> l_JobList = new List<JobParameter>();

    public List<GunParameter> l_ARList = new List<GunParameter>();
    public List<GunParameter> l_HGList = new List<GunParameter>();
    public List<GunParameter> l_MELEEList = new List<GunParameter>();
    public List<GunParameter> l_SMGList = new List<GunParameter>();
    public List<GunParameter> l_SRList = new List<GunParameter>();
    public List<GunParameter> l_HEALList = new List<GunParameter>();
    public List<GunParameter> l_MGList = new List<GunParameter>();
    public List<GunParameter> l_ROCKETList = new List<GunParameter>();

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
        loadXML_Gun(xml_Weapon);
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
    private void loadXML_Gun(string filename)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + filename);
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(txtAsset.text);
        XmlNodeList all_xml_Weapon_Table = xmlDoc.SelectNodes("Main/Weapon");
        foreach (XmlNode node in all_xml_Weapon_Table)
        {
            GunParameter temp = new GunParameter(node.SelectSingleNode("Name").InnerText, System.Convert.ToSingle(node.SelectSingleNode("Bulletspeed").InnerText), System.Convert.ToSingle(node.SelectSingleNode("Damage").InnerText), node.SelectSingleNode("BulletName").InnerText, System.Convert.ToSingle(node.SelectSingleNode("Magazine").InnerText));
            string[] tempName = node.SelectSingleNode("Name").InnerText.Split('_');
            if(tempName[0] == "Ar")
            { 
                l_ARList.Add(temp);
            }
            else if(tempName[0] == "Hg")
            {
                l_HGList.Add(temp);
            }
            else if(tempName[0] == "Melee")
            {
                l_MELEEList.Add(temp);
            }
            else if(tempName[0] == "Smg")
            {
                l_SMGList.Add(temp);
            }
            else if(tempName[0] == "Sr")
            {
                l_SRList.Add(temp);
            }
            else if(tempName[0] == "Heal")
            {
                l_HEALList.Add(temp);
            }
            else if(tempName[0] == "Mg")
            {
                l_MGList.Add(temp);
            }
            else if(tempName[0] == "Rocket")
            {
                l_ROCKETList.Add(temp);
            }

        }
        //loadGunList();

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
                tempObj.GetComponent<Player>().s_jobname = jparam.s_JobName;
                tempObj.GetComponent<Player>().n_hp = jparam.f_HP;
                tempObj.GetComponent<Player>().f_Speed = jparam.f_Speed;
                PlayableCharacterList.Add(tempObj);
            }
        }
    }

    public void selectGun(GameObject tempPlayer, GunParameter weapon1, GunParameter weapon2)
    {
        tempPlayer.GetComponent<Player>().Weapon1 = weapon1;
        tempPlayer.GetComponent<Player>().Weapon2 = weapon2;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
