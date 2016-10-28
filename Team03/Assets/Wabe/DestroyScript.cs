using UnityEngine;
using System.Collections;

public class DestroyScript : MonoBehaviour {

    [SerializeField]
    private float dethTime;

    private float time;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += 1;

        if(time >= dethTime)
        {
            Destroy(this.gameObject);
        }
	}
}
