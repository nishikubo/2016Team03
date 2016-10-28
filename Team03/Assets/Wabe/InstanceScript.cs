using UnityEngine;
using System.Collections;

public class InstanceScript : MonoBehaviour {

    public GameObject wabe;
    public GameObject effect;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(wabe, new Vector3(0, 0, 0), new Quaternion(0, 0, 0,0));
            Instantiate(effect, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }	
	}
}
