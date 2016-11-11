using UnityEngine;
using System.Collections;

public class WaveSizeChange : MonoBehaviour
{
    //波のサイズ
    private Transform size_;
    //波の一時的なサイズ
    private float objectSize_;
    //波の最大のサイズの設定
    private float maxSize_;
    //波の大きくなる値
    private float objectAdd_;


    // Use this for initialization
    void Start ()
    {
        objectSize_ = 0;
        size_ = this.gameObject.GetComponent<Transform>();
        maxSize_ = WaveSetting.Setting.WaveMaxSize();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //波のサイズの変更
        size_.transform.localScale = new Vector3(objectSize_, objectSize_, objectSize_);
        //波のサイズが最大値を超えたら消去
        if(objectSize_ > maxSize_)
        {
            Destroy(this.gameObject);
        }
        objectSize_ += 0.05f;
    }    
}
