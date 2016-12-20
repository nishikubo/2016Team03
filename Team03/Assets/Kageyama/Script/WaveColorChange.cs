using UnityEngine;
using System.Collections;

public class WaveColorChange : MonoBehaviour
{
    //マテリアル
    private float myMaterial_;
    //α値を下げていく値
    private float alphaSub_;
    //α値を抜いた色の設定(x=赤 y=緑 z=青  それぞれ最低0、最大1)
    private Vector3 color_rgb_;

    [SerializeField]
    private Color _playerColor, _enemyColor, _objectColor;

    // Use this for initialization
    void Start ()
    {
        //α値を初期化
        myMaterial_ = 1.0f;
        if(this.gameObject.tag == "PlayerWave")
        {
            color_rgb_ = new Vector3(_playerColor.r, _playerColor.g, _playerColor.b);
        }
        else if (this.gameObject.tag == "EnemyWave")
        {
            color_rgb_ = new Vector3(_enemyColor.r, _enemyColor.g, _enemyColor.b);
        }
         else
        {
            color_rgb_ = new Vector3(_objectColor.r, _objectColor.g, _objectColor.b);
        }
        //下げていくα値を設定
        alphaSub_ = WaveSetting.Setting.WaveAlphaSub();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //波の色の変更
        this.gameObject.GetComponent<Renderer>().material.color = new Color(color_rgb_.x, color_rgb_.y, color_rgb_.z, myMaterial_);
        myMaterial_ -= alphaSub_;
    }
}
