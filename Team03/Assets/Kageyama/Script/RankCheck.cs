using UnityEngine;
using System.Collections;

public class RankCheck : MonoBehaviour
{
    private int _rank;
    [SerializeField]
    private GameObject[] _rankObject;

	// Use this for initialization
	void Start ()
    {
        GameObject ClearCheck = GameObject.Find("MissionClearChecker");
        if (ClearCheck == null) return;
        _rank = ClearCheck.GetComponent<MissonClearCheck>().GetClear();
        _rankObject[_rank - 1].SetActive(true);
        Destroy(ClearCheck);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
