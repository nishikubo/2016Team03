using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyBirdState
{
    Wait,
    Attack,
    Rise,
    Died,
    Hit
}

public class EnemyBirdRoutine : EnemyBase<EnemyBirdRoutine, EnemyBirdState>
{
    #region Component
    private Transform _target;
    private Rigidbody _rd;
    #endregion

    #region Tag
    private string _TargetTag = "Player";               // ターゲットとする相手のタグ
    private string _FieldTag = "Floor";                 // 地面のタグ
    #endregion

    #region -各種ステータス-
    public float _SearchArea = 10;                      // 索敵範囲(距離)
    public float _RotateSmooth = 3.0f;                  // 振り向きにかかる速度
    public float _Speed = 5;                            // 追跡時の移動スピード
    public Color _WaveColor;                            // 衝撃波のカラー
    #endregion

    public string[] _outlayer = new string[] { "" };    // 索敵時に無視するレイヤー名

    #region -各種フラグ-
    private bool _IsGround = false;                     // 接地しているかどうか
    private bool _Diedflag = false;                     // 死亡しているかどうか
    #endregion

    private Vector3[] _LoiteringPos;                    // 徘徊するポジション
    private Vector3 _vec;
    private float _height;                              // 高さ
    private string _HitTag;                             // 衝突相手のTag

    #region -各種スクリプト-
    private PosGenerator _PosGene;
    private Blowoff _Blowoff;
    #endregion

    [SerializeField]
    private bool _laydrawflag = true;                   // 徘徊ルートの表示(デバッグ用)

    // Use this for initialization
    public void Start()
    {
        // Targetの座標を取得
        _target = GameObject.FindGameObjectWithTag(_TargetTag).transform;

        // 各種コンポーネントの取得
        _rd = GetComponent<Rigidbody>();

        // 各種スクリプトの取得
        _PosGene = transform.FindChild("LoiteringPositions").GetComponent<PosGenerator>();
        _Blowoff = GetComponent<Blowoff>();

        // 各種値の初期化
        _LoiteringPos = new Vector3[_PosGene.GetPos.Length];
        for (int i = 0; i < _LoiteringPos.Length; i++)
        {
            _LoiteringPos[i] = _PosGene.GetPos[i].position;
        }


        if (_laydrawflag)
        {
            Transform poss = transform.FindChild("LoiteringPositions");
            poss.transform.parent = null;
        }
        else
        {
            GameObject obj = transform.FindChild("LoiteringPositions").gameObject;
            obj.SetActive(false);
        }

        // Stateの初期設定
        statelist.Add(new StateWait(this));
        statelist.Add(new StateAttack(this));
        statelist.Add(new StateRise(this));
        statelist.Add(new StateDied(this));
        statelist.Add(new StateHit(this));

        stateManager = new StateManager<EnemyBirdRoutine>();

        ChangeState(EnemyBirdState.Wait);
    }


    /// <summary>
    /// 範囲内のサーチ
    /// </summary>
    /// <param name="targetpos">ターゲットのTransform</param>
    /// <param name="position">自分のTransform</param>
    /// <param name="area">サーチ範囲</param>
    /// <param name="outlayer">lay飛ばす際に無視するlayer</param>
    /// <param name="targetTag">ターゲットのタグ</param>
    /// <returns></returns>
    public bool Search(Transform targetpos, Transform position, float area, string[] outlayer, string targetTag)
    {
        bool discovery = false;

        Vector3 temp = targetpos.position - position.position;
        float toTargetDistance = Vector3.SqrMagnitude(temp);

        if (toTargetDistance < area * area)
        {
            RaycastHit hit;
            int layerMask = ~LayerMask.GetMask(outlayer);
            if (Physics.Raycast(position.position, temp.normalized, out hit, area, layerMask))
            {
                discovery = hit.collider.tag == targetTag;
            }
        }

        return discovery;
    }

    /// <summary>
    /// ターゲットとの距離をFloatで返します(2乗された値なので注意)
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    /// <param name="thispos">自分のTransform</param>
    /// <returns>距離</returns>
    public float ToDistance(Transform target, Transform thispos)
    {
        Vector3 temp = target.position - thispos.position;

        return Vector3.SqrMagnitude(temp);
    }

    // ヒット処理
    public void Hit(int damage)
    {
        if (IsDied) return;

        Debug.Log("ﾋｯﾄｫｫ!!");

        ChangeState(EnemyBirdState.Hit);
    }


    #region ---各種衝突検知処理---
    public void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = true;
        if (col.collider.tag == "Building")
        {
            _HitTag = "PlayerWave";
            FreezeRotation(false);
            Hit(1);
            _vec = (transform.position - col.transform.position).normalized;
        }
    }
    public void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = false;
    }

    public void OnTriggerEnter(Collider col)
    {
        if ((col.tag == "PlayerWave" || col.tag == "ObjectWave") && !IsCurrentState(EnemyBirdState.Hit)) {
            _HitTag = col.tag;
            FreezeRotation(false);
            Hit(1);
            _vec = (transform.localPosition - col.transform.localPosition).normalized;
        }
    }
    #endregion

    #region ---各種プロパティ---
    // 地面に触れているかどうか
    public bool IsGround
    {
        get { return _IsGround; }
        set { _IsGround = value; }
    }
    // Enemyと当たったかどうか
    public bool IsHitEnemy
    {
        get
        {
            bool flag = false;
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, -Vector3.up, out hit, 0.5f))
            {
                flag = hit.collider.tag == "Enemy";
            }
            return flag;
        }
    }
    public bool IsDied
    {
        get { return _Diedflag; }
    }
    #endregion

    // 死亡
    public void Died()
    {
        _Diedflag = true;
    }

    // 回転軸を固定するかしないか
    public void FreezeRotation(bool value)
    {
        switch (value)
        {
            case true:
                _rd.constraints =
                    RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationZ;
                break;
            case false:
                _rd.freezeRotation = false;
                break;
        }
    }

    /*----------------------------------------------------/
                        ここからState処理
    /----------------------------------------------------*/

    /// <summary>
    /// 待機状態
    /// </summary>
    private class StateWait : IState<EnemyBirdRoutine>
    {
        private int cureentroot = 0;

        public StateWait(EnemyBirdRoutine owner) : base(owner)
        { }

        public override void Initialize()
        {
            owner._rd.useGravity = false;
        }

        public override void Execute()
        {
            if (owner.Search(owner._target, owner.transform, owner._SearchArea, owner._outlayer, owner._TargetTag))
            {
                owner.ChangeState(EnemyBirdState.Attack);
                return;
            }

            // 目的地設定
            Vector3 pos = owner._LoiteringPos[cureentroot];
            // 向きの設定
            Vector3 vec = (pos - owner.transform.position).normalized;
            // 徘徊処理
            vec.y = 0;
            Quaternion targetRotate = Quaternion.LookRotation(vec);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotate, Time.deltaTime * owner._RotateSmooth);

            owner.transform.Translate(Vector3.forward * owner._Speed * Time.deltaTime);


            if (Vector3.Distance(owner.transform.position, pos) < 1)
            {
                cureentroot += 1;
                cureentroot = cureentroot % owner._LoiteringPos.Length;
            }

        }

        public override void End()
        {
            owner._rd.useGravity = true;
        }

    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private class StateAttack : IState<EnemyBirdRoutine>
    {
        public StateAttack(EnemyBirdRoutine owner) : base(owner)
        { }

        Vector3 vec;
        bool run = false;

        public override void Initialize()
        {
            vec = new Vector3(0, 0, 0);
            owner._height = owner.transform.position.y;
        }

        public override void Execute()
        {
            if (owner.IsHitEnemy)
            {
                vec.y = 3.0f;
            }


            if (owner.IsGround)
            {
                Vector3 pos = owner.transform.position;
                pos.y -= owner.transform.lossyScale.y * 0.5f - 0.1f;
                owner.StartCoroutine(Wave(pos));
                return;
            }

            owner._rd.velocity = vec;
            vec.y += Physics.gravity.y * Time.deltaTime;
        }

        public override void End()
        {
            run = false;
            owner._rd.velocity = new Vector3(0, 0, 0);
        }

        IEnumerator Wave(Vector3 pos)
        {
            if (run) { yield break; }
            run = true;
            Debug.Log("Take That, You Field!!");

            WaveSetting.Setting.WaveSizeSet(1.5f);
            WaveSetting.Setting.SettingColor(owner._WaveColor);
            WaveSetting.Setting.Instance(1, pos, "EnemyWave");

            yield return new WaitForSeconds(1);
            if (owner.IsCurrentState(EnemyBirdState.Attack))
            {
                owner.ChangeState(EnemyBirdState.Rise);
            }
        }
    }

    private class StateRise : IState<EnemyBirdRoutine>
    {
        public StateRise(EnemyBirdRoutine owner) : base(owner)
        { }

        float vecy;

        public override void Initialize()
        {
            vecy = owner._height;
            owner._rd.useGravity = false;
        }

        public override void Execute()
        {
            if(vecy <= owner.transform.position.y)
            {
                owner.ChangeState(EnemyBirdState.Wait);
                return;
            }

            owner._rd.velocity = Vector3.up * owner._Speed * 0.5f;
        }

        public override void End()
        {
            owner._rd.velocity = new Vector3(0, 0, 0);
        }
    }



    /// <summary>
    /// 死亡処理
    /// </summary>
    private class StateDied : IState<EnemyBirdRoutine>
    {
        public StateDied(EnemyBirdRoutine owner) : base(owner)
        { }

        public override void Initialize()
        {
            Debug.Log("ｵﾗﾊｼﾝｼﾞﾏｯﾀﾀﾞｰ");
            owner.Died();
        }

        public override void Execute()
        {
        }

        public override void End()
        {
        }
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    private class StateHit : IState<EnemyBirdRoutine>
    {
        public StateHit(EnemyBirdRoutine owner) : base(owner)
        { }

        bool flag = false;

        public override void Initialize()
        {
            owner._rd.useGravity = true;

            Vector3 vec = owner._vec;
            vec.y = 1;
            owner._Blowoff.blowoff(owner._rd, vec);
            iTween.RotateTo(owner.gameObject, iTween.Hash("x", 270));
        }

        public override void Execute()
        {
            if (owner.IsGround && flag)
            {

                Vector3 pos = owner.transform.position;
                pos.y -= owner.transform.lossyScale.y * 0.5f - 0.1f;

                WaveSetting.Setting.WaveSizeSet(2);
                WaveSetting.Setting.Instance(1, pos, owner._HitTag);
                owner.ChangeState(EnemyBirdState.Died);
                return;
            }
            if (!owner.IsGround && !flag) flag = true;
        }

        public override void End()
        {
            owner.FreezeRotation(true);
            flag = false;
        }
    }

}
