using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    //吹き飛んでいるかどうか
    [SerializeField]
    private bool blowoff_frag_;
    private float myScale_;
    // Use this for initialization
    void Start()
    {
        blowoff_frag_ = false;
        myScale_ = transform.localScale.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "PlayerWave")
        {
            blowoff_frag_ = true;
        }

        if (blowoff_frag_ == false) return;
        Vector3 instance_position = transform.position;
        instance_position.y -= myScale_ - 0.1f;
        if (collision.collider.tag == "Floor")
        {
            WaveSetting.Setting.Instance(5, instance_position, "EnemyWave");
            blowoff_frag_ = false;
        }
    }
}
