using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    
    private enum Mission
    {
        Enemy_Down,
        Enemy_Shot_Up,
        Building_Shot_Up,
        Through
    }

    //private Dictionary<Mission, bool> _dicMission = new Dictionary<Mission, bool>();
    [SerializeField]
    private Mission _m;

    //自分のミッションがクリアしているかどうか
    private bool _clearFrag;
    
    //ミッションクリアしたときに表示させる画像
    public GameObject _clear;

    [SerializeField, TooltipAttribute("横からスライドしてくるならチェックを入れる")]
    private bool _confirmation;
    [SerializeField, TooltipAttribute("メインミッションだったらチェックを入れる")]
    private bool _mainMisson;

    //横にスライドするための変数
    private RectTransform _myRectTransform;
    private Vector2 _startPosition;

    //何個ミッションをクリアしたか数えるオブジェクト
    private GameObject _clearCheck;
    

    // Use this for initialization
    void Start ()
    {
        _clearFrag = false;
        _myRectTransform = GetComponent<RectTransform>();
        _startPosition = GetComponent<RectTransform>().localPosition;
        _clearCheck = GameObject.Find("MissionClearChecker");
        //_dicMission.Add(Mission.a, MissionManager.Misson.GET_Enemy_Down());
        //_dicMission.Add(Mission.b, MissionManager.Misson.GET_Enemy_Shot_Up());
        //_dicMission.Add(Mission.c, MissionManager.Misson.GET_Building_Shot_Up());
    }
	
	// Update is called once per frame
	void Update ()
    {
        //ミッションをクリアしていなかったらクリアしたかどうか調べる
        if (_clearFrag == false)
        {
            if (_m == Mission.Enemy_Down && MissionManager.Misson.GET_Enemy_Down() == true)
            {
                Clear();
            }
            else if (_m == Mission.Enemy_Shot_Up && MissionManager.Misson.GET_Enemy_Shot_Up() == true)
            {
                Clear();
            }
            else if (_m == Mission.Building_Shot_Up && MissionManager.Misson.GET_Building_Shot_Up() == true)
            {
                Clear();
            }
            else if (_m == Mission.Through && MissionManager.Misson.GET_Through() == true)
            {
                Clear();
            }
        }
	}

    void Clear()
    {
        //ストップ画面で表示する
        if (_confirmation == false)
        {
            this.GetComponent<Button>().interactable = false;
            _clearFrag = true;
            _clear.SetActive(true);
            if(_mainMisson == true)
            {
                ClearCheck();
                SceneChangeScript.sceneChange.FadeOut("StageClear");
            }
        }
        //クリアしたら横からスライドさせる
        else
        {
            ClearCheck();
            _clearFrag = true;
            this.GetComponent<MissonSlidePosition>().Move(1);
            LeanTween.move(
                _myRectTransform,
                new Vector2(this.GetComponent<RectTransform>().localPosition.x - 430, this.GetComponent<RectTransform>().localPosition.y),
                1.0f)
                .setEase(LeanTweenType.easeOutQuart)
                .setOnComplete(()=>
                {
                    LeanTween.move(
                        _myRectTransform,
                        new Vector2(this.GetComponent<RectTransform>().localPosition.x + 430, this.GetComponent<RectTransform>().localPosition.y),
                        0.3f)
                        .setDelay(1.0f)
                        .setOnComplete(() => {
                            this.GetComponent<MissonSlidePosition>().Move(-1);
                        });
                });
        }
    }

    void ClearCheck()
    {
        if (_clearCheck == null) return;

        _clearCheck.GetComponent<MissonClearCheck>().AddClearCount();
    }
}
