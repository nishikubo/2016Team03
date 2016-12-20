using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{
    public bool _resumption;

	// Use this for initialization
	void Start ()
    {
        _resumption = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_resumption == false)
            {
                this.GetComponent<RectTransform>().localPosition = new Vector2(-200, -100);
                _resumption = true;
            }

            else
            {
                this.GetComponent<RectTransform>().localPosition = new Vector2(200, -100);
                _resumption = false;
            }
        }

	}
}
