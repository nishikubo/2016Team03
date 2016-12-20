using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleLogo_Rotate : MonoBehaviour
{
    [SerializeField]
    private float _rotateZ;
    private float _angleZ;

	// Use this for initialization
	void Start ()
    {
        _angleZ = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        _angleZ +=Time.deltaTime * _rotateZ;
        this.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, _angleZ); 
        
	}
}
