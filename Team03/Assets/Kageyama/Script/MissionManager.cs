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
    [SerializeField, TooltipAttribute("何体敵を倒したか数える(入力しないで)")]
    public int _enemy_Down_Count;
    //何体敵を倒したらミッションクリアにするか決める
    [SerializeField, TooltipAttribute("何体敵を倒すか決める")]
    private int _enemy_Down_Clear = 0;

    //何体敵が打ちあがっているか
    private int _enemy_Shot_Up_Count;
    //何体敵を倒したらミッションクリアにするか決める
    [SerializeField, TooltipAttribute("何体敵を打ち上げていればよいか決める")]
    private int _enemy_Shot_Up_Clear = 0;

    //何体敵が打ちあがっているか
    private int _building_Shot_Up_Count;
    //何体敵を倒したらミッションクリアにするか決める
    [SerializeField, TooltipAttribute("何体オブジェクトを打ち上げていればよいか決める")]
    private int _building_Shot_Up_Clear = 0;

    //現在何個ミッションをクリアしているか調べる
    [SerializeField, TooltipAttribute("いくつのミッションをクリアしているか(入力しないで)")]
    public int _clearCount = 0;
    //何個ミッションをクリアしたらゲームクリアにするか決める
    [SerializeField, TooltipAttribute("クリアするミッションの個数")]
    private int _gameClear = 0;

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
        if(_clearCount >= _gameClear)
        {
            SceneChangeScript.sceneChange.SceneOut(1);
        }
	}

    /// <summary>
    /// 敵を倒したらミッションを進める
    /// </summary>
    /// <param name="add">加算していく数</param>
    public void Enemy_Down(int add)
    {
        _enemy_Down_Count += add;

        //ミッションクリア条件を満たしていればミッションクリア
        if (_enemy_Down_Count >= _enemy_Down_Clear && _enemy_Down == false)
        {
            _clearCount += 1;
            _enemy_Down = true;
        }
    }

    /// <summary>
    /// 打ちあがっている敵を数える
    /// </summary>
    /// <param name="up_count">打ちあがったら+1、落ちたら-1</param>
    public void Enemy_Shot_Up(int up_count)
    {
        _enemy_Shot_Up_Count += up_count;

        //ミッションクリア条件を満たしていればミッションクリア
        if (_enemy_Shot_Up_Count >= _enemy_Shot_Up_Clear &&
            _enemy_Shot_Up == false)
        {
            _clearCount += 1;
            _enemy_Shot_Up = true;
        }
    }

    /// <summary>
    /// 打ちあがっているオブジェクトの数を数える
    /// </summary>
    /// <param name="up_count">打ちあがったら+1、落ちたら-1</param>
    public void Building_Shot_Up(int up_count)
    {
        _building_Shot_Up_Count += up_count;

        //ミッションクリア条件を満たしていればミッションクリア
        if (_building_Shot_Up_Count >= _building_Shot_Up_Clear &&
            _building_Shot_Up == false)
        {
            _clearCount += 1;
            _building_Shot_Up = true;
        }
    }

    /// <summary>
    /// 指定の場所を通ったらフラグを立てる
    /// </summary>
    public void Through()
    {
        //ミッションクリア条件を満たしていればミッションクリア
        if (_through == false)
        {
            _clearCount += 1;
            _through = true;
        }
    }

    public bool GET_Enemy_Down()
    {
        return _enemy_Down;
    }

    public bool GET_Enemy_Shot_Up()
    {
        return _enemy_Shot_Up;
    }

    public bool GET_Building_Shot_Up()
    {
        return _building_Shot_Up;
    }

    public bool GET_Through()
    {
        return _through;
    }
}
