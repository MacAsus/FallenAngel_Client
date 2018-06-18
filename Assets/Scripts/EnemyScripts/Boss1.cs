using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : UnityEvent<int, string, float>
{

}

public class Boss1 : EnemyGeneral {

    public AnimEvent animEvent;



    string[] s_CurAnim = { " ", " ", " " };
    bool b_IsSpin = false;

	// Use this for initialization
	void Start () {
        InitializeParam();
        if(spine_CharacterAnim == null)
        {
            return;
        }

        spine_CharacterAnim.state.Start += delegate (Spine.TrackEntry entry)
        {
            if (spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Shot")
            {
                int temp = Random.Range(1, 3);
                string animName = "Robot_Attack" + temp;
                spine_CharacterAnim.state.SetAnimation(1, animName, true);
            }
            if(spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Dead")
            {
                b_IsSpin = false;
                spine_CharacterAnim.state.ClearTrack(2);
                spine_CharacterAnim.state.SetAnimation(1, "Robot_Dead", true);
            }
            if(spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Idle")
            {
                if (spine_CharacterAnim.state.GetCurrent(2) != null)
                {
                    b_IsSpin = false;
                    spine_CharacterAnim.state.GetCurrent(2).loop = false;
                }
                spine_CharacterAnim.state.SetAnimation(1, "Robot_Idle1", true);
            }
        };
        spine_CharacterAnim.state.Complete += delegate (Spine.TrackEntry entry)
        {
            if (spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Spin")
            {
                if (b_IsSpin)
                {
                    return;
                }
                else
                {
                    spine_CharacterAnim.state.ClearTrack(entry.TrackIndex);
                }
            }
        };

        if(animEvent == null)
        {
            animEvent = new AnimEvent();
        }
        //animEvent.AddListener(setAnimation);
	}




    protected override void InitializeParam()
    {
        base.InitializeParam();
    }

    // Update is called once per frame
    void Update () {


        if (Input.GetKey(KeyCode.Q))
        {
            
            setAnimation(0, "Shot", true, 1);
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (b_IsSpin)
            {
                b_IsSpin = false;
            }
            else
            {
                b_IsSpin = true;
                setAnimation(2, "Spin", true, 1.25f);
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            setAnimation(0, "Idle", true, 1);
        }
        if (Input.GetKey(KeyCode.R))
        {
            setAnimation(0, "Dead", true, 1);
        }
    }

    void setAnimation(int track, string animName, bool loop, float speed)
    {
        if(spine_CharacterAnim.state.GetCurrent(track) != null)
        {
            if (spine_CharacterAnim.state.GetCurrent(track).Animation.name != animName)
            {
                s_CurAnim[track] = " ";
            }
        }else
        {
            s_CurAnim[track] = " ";
        }
        if (s_CurAnim[track] == animName)
        {
            return;
        }
        else
        {
            s_CurAnim[track] = animName;
            spine_CharacterAnim.state.SetAnimation(track, animName, loop).timeScale = speed;
        }
    }

    //void setAnimation(int track, string animName, float speed)
    //{
    //    spine_CharacterAnim.state.SetAnimation(track, animName, true).TimeScale = speed;
    //}

    void handleEvent(Spine.TrackEntry entry, Spine.Event e) 
    {
        
    }



    
}
