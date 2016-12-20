using UnityEngine;
using System.Collections;

public class WaveSetting : MonoBehaviour {

    protected static WaveSetting waveSizeChange;
    //波のオブジェクト
    public GameObject wave_;
    //エフェクトのオブジェクト
    public GameObject effect_;
    //波のあたり判定
    public GameObject waveCollider_;
    //波の最大のサイズを設定
    private float waveMaxSize_;
    //波のα値を下げていく値
    private float wabeAlphaSub_;
    //波の色を設定
    public Color color_;
    //波の色を返す値(赤、緑、青 のみ)
    private Vector3 waveColor_;

    //どこでも参照可
    public static WaveSetting Setting
    {
        get
        {
            if (waveSizeChange == null)
            {
                waveSizeChange = (WaveSetting)FindObjectOfType(typeof(WaveSetting));
                if (waveSizeChange == null)
                {
                    Debug.LogError("SceneChange Instance Error");
                }
            }

            return waveSizeChange;
        }
    }

    // Use this for initialization
    void Start ()
    {
        WaveSizeSet(1.7f);
        SettingColor(color_);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            SettingColor(color_);
            Instance(5, new Vector3(0, 0.01f, 0), "PlayerWave");
        }	
	}

    /// <summary>
    /// 波とエフェクトを作成
    /// </summary>
    /// <param name="position">座標</param>
    /// <param name="tagname">タグの名前</param>
    public void Instance(Vector3 position, string tagname)
    {
        //波を生成(一つ)
        WaveInstance(position, tagname);
        //パーティクルの生成
        EffectInstance(position);
    }

    /// <summary>
    /// 波とエフェクトを作成
    /// </summary>
    /// <param name="num">衝撃波の個数</param>
    /// <param name="position">座標</param>
    /// <param name="tagname">タグの名前</param>
    public void Instance(int num, Vector3 position, string tagname)
    {
        //生成(複数)
        StartCoroutine(WaveInstance(num, position, tagname));
        //パーティクルの生成
        EffectInstance(position);
    }

    /// <summary>
    /// 波の作成
    /// </summary>
    /// <param name="position">座標</param>
    /// <param name="tagname">タグの名前</param>
    public void WaveInstance(Vector3 position, string tagname)
    {
        waveCollider_.tag = tagname;
        wave_.tag = tagname;
        //当たり判定の作成
        Instantiate(waveCollider_, position, new Quaternion(0, 0, 0, 0));
        Instantiate(wave_, position, new Quaternion(0, 0, 0, 0));
    }

    /// <summary>
    /// 波だけを生成(当たり判定なし)
    /// </summary>
    /// <param name="position">座標</param>
    /// <param name="tagname">タグの名前</param>
    public void WaveOnlyInstance(Vector3 position, string tagname)
    {
        wave_.tag = tagname;
        Instantiate(wave_, position, new Quaternion(0, 0, 0, 0));
    }

    /// <summary>
    /// 波の生成(複数)
    /// </summary>
    /// <param name="num">衝撃波の個数</param>
    /// <param name="position">座標</param>
    /// <param name="tagname">タグの名前</param>
    /// <returns></returns>
    public IEnumerator WaveInstance(int num, Vector3 position, string tagname)
    {
        waveCollider_.tag = tagname;
        //当たり判定の作成
        Instantiate(waveCollider_, position, new Quaternion(0, 0, 0, 0));
        for (int i = 0; i < num; i++)
        {
            wave_.tag = tagname;
            Instantiate(wave_, position, new Quaternion(0, 0, 0, 0));
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// エフェクトの作成
    /// </summary>
    /// <param name="position">座標</param>
    public void EffectInstance(Vector3 position)
    {
        Instantiate(effect_, position, new Quaternion(0, 0, 0, 0));
    }

    /// <summary>
    /// 波の大きくなる最大値を設定
    /// </summary>
    /// <param name="size">波の最大値</param>
    public void WaveSizeSet(float size)
    {
        waveMaxSize_ = size;
        //波の最大の大きさから引いていくα値を計算
        wabeAlphaSub_ = 1.0f / (waveMaxSize_ / 0.05f);
    }

    /// <summary>
    /// 波の最大の大きさを返す
    /// </summary>
    /// <returns></returns>
    public float WaveMaxSize()
    {
        return waveMaxSize_;
    }

    /// <summary>
    /// 波の下げていく値を返す
    /// </summary>
    /// <returns></returns>
    public float WaveAlphaSub()
    {
        return wabeAlphaSub_;
    }

    /// <summary>
    /// 波の色の設定
    /// </summary>
    /// <param name="color"></param>
    public void SettingColor(Color color)
    {
        waveColor_ = new Vector3(color.r, color.g, color.b);
    }

    /// <summary>
    /// 波の色を返す
    /// </summary>
    /// <returns>波の色</returns>
    public Vector3 WaveColor()
    {
        return waveColor_;
    }
}
