using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour
{
    [SerializeField, TooltipAttribute("流したいBGMの番号")]
    private int _number = 0;
    [SerializeField, TooltipAttribute("BGMをフェードインさせる")]
    private bool _BGM_FadeIn = false;

	// Use this for initialization
	void Start ()
    {
        if (_BGM_FadeIn == true)
        {
            SoundManger.Instance.FadeInBGM(_number);
        }
        else
        {
            SoundManger.Instance.PlayBGM(_number);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
