using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{

    [SerializeField]
    private float _speed;
    private float _angle;

    // Use this for initialization
    void Start()
    {
        _angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _angle += Time.deltaTime * _speed;
        this.transform.rotation = Quaternion.Euler(90, _angle, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            MissionManager.Misson.Through();
            Destroy(this.gameObject);
        }
    }
}
