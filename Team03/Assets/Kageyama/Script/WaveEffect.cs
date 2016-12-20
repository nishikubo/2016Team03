using UnityEngine;
using System.Collections;

public class WaveEffect : MonoBehaviour
{
    //消える時間
    [SerializeField]
    private float dethTime;
    //時間計測
    private float time;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if (time >= dethTime)
        {
            Destroy(this.gameObject);
        }
    }
}
