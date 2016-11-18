using UnityEngine;
using System.Collections;

public class BuilBlow : MonoBehaviour
{
    private Blowoff _Blowoff;
    private Rigidbody _rd;

    public void Start()
    {
        _Blowoff = GetComponent<Blowoff>();
        _rd = GetComponent<Rigidbody>();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerWave")
        {
            Vector3 vec = (transform.position - col.transform.position).normalized;
            vec /= 2;
            vec.y = 1;
            _Blowoff.blowoff(_rd, vec, 10);
        }
    }
}
