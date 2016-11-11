using UnityEngine;
using System.Collections;

public class WaveEffect : MonoBehaviour
{
    //メインカメラ
    private GameObject camera_;
    //消える時間
    [SerializeField]
    private float dethTime;
    //時間計測
    private float time;

    // Use this for initialization
    void Start ()
    {
        camera_ = GameObject.Find("Main Camera");
        //生成と同時にカメラを揺らす
        iTween.ShakePosition(camera_, iTween.Hash("x", 0.3f, "y", 0.3f, "time", 0.5f));
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
