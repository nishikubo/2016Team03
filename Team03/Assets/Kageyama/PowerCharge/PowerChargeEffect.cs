using UnityEngine;
using System.Collections;

public class PowerChargeEffect : MonoBehaviour
{
    private float _charge = 1;
    [SerializeField]
    private float _add;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        _charge += _add;
        Mathf.Clamp(_charge, 3.1f, 1);
        if (_charge <= 3)
        {
            this.GetComponent<ParticleSystem>().startSize = _charge;
        }
	}
}
