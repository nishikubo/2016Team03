using UnityEngine;
using System.Collections;

public class MissionManager : MonoBehaviour
{
    protected static MissionManager missionManager;

    //どこでも参照可
    public static MissionManager Misson
    {
        get
        {
            if (missionManager == null)
            {
                missionManager = (MissionManager)FindObjectOfType(typeof(MissionManager));
                if (missionManager == null)
                {
                    Debug.LogError("SceneChange Instance Error");
                }
            }

            return missionManager;
        }
    }

    //クリアしているかどうかを検知するフラグ
    [SerializeField, TooltipAttribute("クリアしていたらtrue")]
    public bool _enemy_Down, _enemy_Shot_Up, _building_Shot_Up, _through = false;
    //敵を何体倒したかずを数える
    private int _enemy_Down_Count;
    //何体敵を倒したらミッションクリアにするか決める
    [SerializeField, TooltipAttribute("何体敵を倒したか数える(後で消します)")]
    private int _enemy_Down_Clear;

    //現在何個ミッションをクリアしているか調べる
    [SerializeField, TooltipAttribute("いくつのミッションをクリアしているか(後で消します)")]
    private int _clearCount = 0;
    //何個ミッションをクリアしたらゲームクリアにするか決める
    [SerializeField, TooltipAttribute("クリアするミッションの個数")]
    private int _gameClearCount;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            _clearCount += 1;
        }
        if(_clearCount >= _gameClearCount)
        {
            SceneChangeScript.sceneChange.SceneOut(1);
        }
	}

    void Enemy_Down(int add)
    {
        _enemy_Down_Count += add;
        if(_enemy_Down_Count >= _enemy_Down_Clear && _enemy_Down == false)
        {
            _clearCount += 1;
            _enemy_Down = true;
        }
    }

    void Enemy_Shot_Up(int up_count)
    {
        int count = 3;
        if(up_count >= count && _enemy_Shot_Up == false)
        {
            _clearCount += 1;
            _enemy_Shot_Up = true;
        }
    }

    void Building_Shot_Up(int up_count)
    {
        int count = 3;
        if (up_count >= count && _building_Shot_Up == false)
        {
            _clearCount += 1;
            _building_Shot_Up = true;
        }
    }

    void Through()
    {
        if (_through == false)
        {
            _clearCount += 1;
            _through = true;
        }
    }
}
