using UnityEngine;
using System.Collections;

public class SizeChangeScript : MonoBehaviour
{
    private Transform size_;
    private float a, b;
    private float myMaterial_;


	// Use this for initialization
	void Start ()
    {
        a = 0;
        b = 0;
        size_ = this.gameObject.GetComponent<Transform>();
        myMaterial_ = 1.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, myMaterial_);
        size_.transform.localScale = new Vector3(a, 1.0f, b);
        if(myMaterial_ < 0)
        {
            Destroy(this.gameObject);
        }
        a += 0.05f;
        b += 0.05f;
        myMaterial_ -= 0.03f;

    }
}
