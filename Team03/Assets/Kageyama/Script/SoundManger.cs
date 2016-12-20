using UnityEngine;
using System.Collections;
using System;

public class SoundManger : MonoBehaviour {

    protected static SoundManger instance;

    public static SoundManger Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (SoundManger)FindObjectOfType(typeof(SoundManger));
                if(instance == null)
                {
                    Debug.LogError("SoundManager Instance Error");
                }
            }

            return instance;
        }
    }

    //音量
    public SoundVolume volume = new SoundVolume();


    //AudioSource

    //BGM
    private AudioSource BGMsource;
    //SE
    private AudioSource[] SEsources = new AudioSource[16];
    //Voice
    private AudioSource[] VoiceSources = new AudioSource[16];

    //AudioClip

    //BGM
    public AudioClip[] BGM;
    //SE
    public AudioClip[] SE;
    //Voice
    public AudioClip[] Voice;

    [SerializeField, TooltipAttribute("音をフェードインさせる際の速さ(最大:0.9 最小:0.1)")]
    private float _fadeInTime;
    [SerializeField, TooltipAttribute("音をフェードアウトさせる際の速さ(最大:0.9 最小:0.1)")]
    private float _fadeOutTime;


    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManager");
        if (obj.Length > 1)
        {
            //既に存在しているなら削除
            Destroy(gameObject);
        }
        else
        {
            //音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }
        //全てのオーディオコンポーネントを追加する

        //BGM AudioSource
        BGMsource = gameObject.AddComponent<AudioSource>();
        //BGMはループを有効にする
        BGMsource.loop = true;

        //SEsource
        for(int i = 0; i < SEsources.Length; i++)
        {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        for (int i = 0; i < VoiceSources.Length; i++)
        {
            VoiceSources[i] = gameObject.AddComponent<AudioSource>();
        }

        //音声source
        for(int i = 0; i < VoiceSources.Length; i++)
        {
            VoiceSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        //ミュート設定
        BGMsource.mute = volume.Mute;
        foreach(AudioSource source in SEsources)
        {
            source.mute = volume.Mute;
        }
        foreach(AudioSource source in VoiceSources)
        {
            source.mute = volume.Mute;
        }

        //ボリューム設定
        BGMsource.volume = volume.BGM;
        foreach(AudioSource source in SEsources)
        {
            source.volume = volume.SE;
        }
        foreach(AudioSource source in VoiceSources)
        {
            source.volume = volume.Voice;
        }
    }


    //BGM再生

    //BGM再生
    public void PlayBGM(int index)
    {
        volume.BGM = 1.0f;
        if (0 > index || BGM.Length <= index)
        {
            return;
        }

        //同じBGMの場合何もしない
        if (BGMsource == BGM[index])
        {
            return;
        }
        BGMsource.Stop();
        BGMsource.clip = BGM[index];
        BGMsource.Play();
    }

    public void FadeInBGM(int index)
    {
        volume.BGM = 0.0f;
        if (0 > index || BGM.Length <= index)
        {
            return;
        }

        //同じBGMの場合何もしない
        if (BGMsource == BGM[index])
        {
            return;
        }
        BGMsource.Stop();
        BGMsource.clip = BGM[index];
        BGMsource.Play();
        StartCoroutine(VolumeUP());
    }

    //BGMの音の大きさを徐々に大きくする
    IEnumerator VolumeUP()
    {
        while (volume.BGM <= 1.0f)
        {
            yield return new WaitForSeconds(0.1f);
            volume.BGM += _fadeInTime;
            print(volume.BGM);
        }
    }

    public void StopBGM()
    {
        BGMsource.Stop();
        BGMsource.clip = null;
    }

    public void FadeOutBGM()
    {
        StartCoroutine(VolumeDown());
    }

    //BGMの音の大きさを徐々に小さくする
    IEnumerator VolumeDown()
    {
        while (volume.BGM >= 0)
        {
            yield return new WaitForSeconds(0.1f);
            volume.BGM -= _fadeOutTime;
            if(volume.BGM <= 0)
            {
                BGMsource.Stop();
                BGMsource.clip = null;
            }
        }
    }

    //SE再生
    public void PlaySE(int index) 
    {
        if(0 > index || SE.Length <= index)
        {
            return;
        }

        //再生中で無いAudioSourceで鳴らす
        foreach(AudioSource source in SEsources)
        {
            if(false == source.isPlaying)
            {
                source.clip = SE[index];
                source.Play();
                return;
            }
        }
    }

    //SE停止
    public void StopSE()
    {
        //全てのSE用のAudioSourceを停止する
        foreach(AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //音声再生
    public void PlayVoice(int index)
    {
        if( 0 > index || Voice.Length <= index )
        {
            return;
        }

        //再生中で無いAudioSourceで鳴らす
        foreach(AudioSource source in SEsources)
        {
            if( false == source.isPlaying)
            {
                source.clip = Voice[index];
                source.Play();
                return;
            }
        }
    }

    //音声停止
    public void StopVoice()
    {
        //全ての音声用のAudioSourceを停止する
        foreach(AudioSource source in VoiceSources)
        {
            source.Stop();
            source.clip = null;
        }
    }
}

//音量クラス
[Serializable]
public class SoundVolume
{
    public float BGM = 1.0f;
    public float Voice = 1.0f;
    public float SE = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        BGM = 1.0f;
        Voice = 1.0f;
        SE = 1.0f;
        Mute = false;
    }
}
