using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    
    private enum Mission
    {
        Enemy_Down,
        Enemy_Shot_Up,
        Building_Shot_Up,
        Through
    }

    private Dictionary<Mission, bool> _dicMission = new Dictionary<Mission, bool>();
    [SerializeField]
    private Mission _m;

    private bool _clearFrag;
    public GameObject _clear;
	// Use this for initialization
	void Start ()
    {
        _clearFrag = false;

        //_dicMission.Add(Mission.a, MissionManager.Misson.GET_Enemy_Down());
        //_dicMission.Add(Mission.b, MissionManager.Misson.GET_Enemy_Shot_Up());
        //_dicMission.Add(Mission.c, MissionManager.Misson.GET_Building_Shot_Up());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_clearFrag == false)
        {
            if (_m == Mission.Enemy_Down && MissionManager.Misson.GET_Enemy_Down() == true)
            {
                Clear();
            }
            else if (_m == Mission.Enemy_Shot_Up && MissionManager.Misson.GET_Enemy_Shot_Up() == true)
            {
                Clear();
            }
            else if (_m == Mission.Building_Shot_Up && MissionManager.Misson.GET_Building_Shot_Up() == true)
            {
                Clear();
            }
            else if (_m == Mission.Through && MissionManager.Misson.GET_Through() == true)
            {
                Clear();
            }
        }
	}

    void Clear()
    {
        this.GetComponent<Button>().interactable = false;
        _clear.SetActive(true);
        _clearFrag = true;
    }
}
