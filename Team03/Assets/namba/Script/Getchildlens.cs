using UnityEngine;
using System.Collections.Generic;

public class Getchildlens : MonoBehaviour {


    public GameObject 親;
    public GameObject[] 配列;

	// Use this for initialization
	void Start () {

        int 数 = 親.transform.childCount;
        Debug.Log(数);
        配列 = new GameObject[数];

        for(int i = 0; i < 数; i++)
        {
            Debug.Log(数);
            配列[i] = 親.transform.GetChild(i).gameObject;
        }
    }
}
