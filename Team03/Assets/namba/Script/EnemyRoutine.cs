﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Wait,
    Pursuit,
    Attack,
    Died,
    Hit,
    Revival
}

public class EnemyRoutine : EnemyBase<EnemyRoutine, EnemyState>
{
    #region Component
    private Transform _target;
    private Rigidbody _rd;
    private NavMeshAgent _Agent;
    #endregion

    #region Tag
    private string _TargetTag = "Player";               // ターゲットとする相手のタグ
    private string _FieldTag = "Floor";                 // 地面のタグ
    #endregion

    #region -各種ステータス-
    public float _SearchArea = 10;                      // 索敵範囲(距離)
    public float _VisionAngle = 45;                     // 索敵範囲(角度)
    public float _AttackArea = 5;                       // 攻撃範囲(距離)
    public float _RotateSmooth = 3.0f;                  // 振り向きにかかる速度
    public float _Speed = 5;                            // 追跡時の移動スピード
    public float _JumpPower = 5;                        // ジャンプ力
    public Color _WaveColor;                            // 衝撃波のカラー
    private float _DownTime = 3;                         // ひっくり返っている時間
    #endregion

    public string[] _outlayer = new string[] { "" };    // 索敵時に無視するレイヤー名

    #region -各種フラグ-
    private bool _IsGround = false;                     // 接地しているかどうか
    private bool _IsHitPlayer = false;                  // プレイヤーと接触しているかどうか
    private bool _Diedflag = false;                     // 死亡しているかどうか
    private bool _TipOver = false;                      // ひっくり返っているかどうか
    [SerializeField]
    private bool _IsTipOver = false;   // ひっくり返るかどうか
    [SerializeField]
    private bool _IsPursuit = true;    // 追跡するかどうか
    #endregion

    private Vector3[] _LoiteringPos;                    // 徘徊するポジション
    private Vector3 _vec;
    private float _DefaultSpeed;

    #region -各種スクリプト-
    private PosGenerator _PosGene;
    private Blowoff _Blowoff;
    private ChackPCollide _Pcol;
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
        _Agent = GetComponent<NavMeshAgent>();

        // 各種スクリプトの取得
        _PosGene = transform.FindChild("LoiteringPositions").GetComponent<PosGenerator>();
        _Blowoff = GetComponent<Blowoff>();
        _Pcol = transform.FindChild("collide").GetComponent<ChackPCollide>();

        // 各種値の初期化
        _LoiteringPos = new Vector3[_PosGene.GetPos.Length];
        _DefaultSpeed = _Speed;
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
        statelist.Add(new StatePursuit(this));
        statelist.Add(new StateAttack(this));
        statelist.Add(new StateDied(this));
        statelist.Add(new StateHit(this));
        statelist.Add(new StateRevival(this));

        stateManager = new StateManager<EnemyRoutine>();

        ChangeState(EnemyState.Wait);
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
                if (hit.collider.tag == targetTag && Vector3.Angle(temp, transform.forward) <= _VisionAngle)
                {
                    discovery = true;
                }
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

        print("hit");

        Vector3 vec;
        vec = _vec;
        vec.y = 1;
        _Blowoff.blowoff(_rd, vec);

        StopAllCoroutines();
        ChangeState(EnemyState.Hit);
    }


    #region ---各種衝突検知処理---
    public void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = true;
        if (col.collider.tag == _TargetTag) IsHitPlayer = true;
    }
    public void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = false;
        if (col.collider.tag == _TargetTag) IsHitPlayer = false;
    }

    public void OnTriggerEnter(Collider col)
    {
        if ((col.tag == "PlayerWave" || col.tag == "ObjectWave") && !IsCurrentState(EnemyState.Revival))
        {
            if (col.tag == "PlayerWave" && _TipOver)
            {
                if (IsDied) return;

                ChangeState(EnemyState.Revival);
                return;
            }
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
    // Playerと当たったかどうか
    public bool IsHitPlayer
    {
        get { return _IsHitPlayer; }
        set { _IsHitPlayer = value; }
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
    private class StateWait : IState<EnemyRoutine>
    {
        private int cureentroot = 0;

        public StateWait(EnemyRoutine owner) : base(owner)
        { }

        private bool run = false;
        private Vector3 v1, v2, forward;
        private Quaternion targetRotate;

        public override void Initialize()
        {
            owner._rd.isKinematic = true;
            owner._Agent.enabled = true;
            targetRotate = Quaternion.LookRotation(owner.transform.forward);
        }

        public override void Execute()
        {
            if (owner.Search(owner._target, owner.transform, owner._SearchArea, owner._outlayer, owner._TargetTag)
                && owner._IsPursuit)
            {
                owner.ChangeState(EnemyState.Pursuit);
                return;
            }

            Vector3 pos = owner._LoiteringPos[cureentroot];

            // 徘徊処理
            if (Vector3.Distance(owner.transform.position, pos) < 1)
            {
                owner.StartCoroutine(ToLookAround());
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotate, Time.deltaTime * owner._RotateSmooth);
                return;
            }

            owner._Agent.SetDestination(pos);
        }

        public override void End()
        {
            owner._Agent.enabled = false;
            owner._rd.isKinematic = false;
            owner.StopCoroutine(ToLookAround());
        }

        IEnumerator ToLookAround()
        {
            if (run) yield break;
            run = true;

            forward = owner.transform.forward;
            v1 = owner.transform.right;
            v2 = owner.transform.right * -1;
            targetRotate = Quaternion.LookRotation(forward);

            yield return new WaitForSeconds(0.5f);

            targetRotate = Quaternion.LookRotation(v1);

            yield return new WaitWhile(() => Quaternion.Angle(owner.transform.rotation, targetRotate) > 10);

            targetRotate = Quaternion.LookRotation(v2);

            yield return new WaitWhile(() => Quaternion.Angle(owner.transform.rotation, targetRotate) > 10);

            targetRotate = Quaternion.LookRotation(forward);

            yield return new WaitWhile(() => Quaternion.Angle(owner.transform.rotation, targetRotate) > 10);

            cureentroot += 1;
            cureentroot = cureentroot % owner._LoiteringPos.Length;

            run = false;
        }
    }

    /// <summary>
    /// 追跡処理
    /// </summary>
    private class StatePursuit : IState<EnemyRoutine>
    {
        public StatePursuit(EnemyRoutine owner) : base(owner)
        { }

        public override void Initialize()
        {
        }

        public override void Execute()
        {
            // _targetが_SearchArea外に出たら移行
            if (!owner.Search(owner._target, owner.transform, owner._SearchArea, owner._outlayer, owner._TargetTag))
            {
                owner.ChangeState(EnemyState.Wait);
                return;
            }

            // _targetが_AttackArea内に入ったら移行
            if (owner.ToDistance(owner._target, owner.transform) < owner._AttackArea)
            {
                owner.ChangeState(EnemyState.Attack);
                return;
            }

            Vector3 vec = owner._target.position - owner.transform.position;
            vec.y = 0;
            Quaternion targetRotate = Quaternion.LookRotation(vec);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotate, Time.deltaTime * owner._RotateSmooth);

            owner.transform.Translate(Vector3.forward * owner._Speed * Time.deltaTime);
        }

        public override void End()
        {
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private class StateAttack : IState<EnemyRoutine>
    {
        public StateAttack(EnemyRoutine owner) : base(owner)
        { }

        Vector3 vec;
        bool flag = false, run = false;

        public override void Initialize()
        {
            vec.y = owner._JumpPower;
        }

        public override void Execute()
        {
            if (owner.IsHitPlayer)
            {
                vec.y = owner._JumpPower;
            }
            if (owner.IsHitEnemy)
            {
                vec.y = owner._JumpPower;
            }


            owner._rd.velocity = vec;
            vec.y += Physics.gravity.y * Time.deltaTime;


            if (owner.IsGround && flag)
            {
                Vector3 pos = owner.transform.position;
                pos.y -= owner.transform.lossyScale.y * 0.5f - 0.1f;
                owner.StartCoroutine(Wave(pos));
                return;
            }
            if (!owner.IsGround && !flag) flag = true;
        }

        public override void End()
        {
            flag = false;
            run = false;
            owner._rd.velocity = Vector3.zero;
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
            if (owner.IsCurrentState(EnemyState.Attack))
            {
                owner.ChangeState(EnemyState.Pursuit);
            }


        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private class StateDied : IState<EnemyRoutine>
    {
        public StateDied(EnemyRoutine owner) : base(owner)
        { }

        public override void Initialize()
        {
            MissionManager.Misson.Enemy_Down(1);
            owner.Died();
            Debug.Log(owner.gameObject.name + "Down!!");
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
    private class StateHit : IState<EnemyRoutine>
    {
        public StateHit(EnemyRoutine owner) : base(owner)
        { }

        bool flag = false, run = false;

        public override void Initialize()
        {
            iTween.RotateTo(owner.gameObject, iTween.Hash("x", 270));
            MissionManager.Misson.Enemy_Shot_Up(1);
        }

        public override void Execute()
        {
            if(run && owner._Pcol.Pcollide)
            {
                owner._IsTipOver = false;

                Vector3 vec;
                vec = (owner.transform.localPosition - owner._target.localPosition).normalized;
                vec.y = 1;
                owner.StopAllCoroutines();

                owner._Blowoff.blowoff(owner._rd, vec, 10, 10);
                owner.ChangeState(EnemyState.Hit);
                return;
            }
            if (owner.IsGround && flag)
            {

                Vector3 pos = owner.transform.position;
                pos.y -= owner.transform.lossyScale.y * 0.5f - 0.1f;

                if (!owner._IsTipOver)
                {
                    WaveSetting.Setting.WaveSizeSet(2);
                    WaveSetting.Setting.Instance(1, pos, "EnemyWave");
                    MissionManager.Misson.Enemy_Shot_Up(-1);
                    owner.ChangeState(EnemyState.Died);
                    return;
                }

                owner.StartCoroutine(TippingOver(pos));

                return;
            }
            if (!owner.IsGround && !flag) flag = true;
        }

        public override void End()
        {
            owner.FreezeRotation(true);
            flag = false;
            run = false;
        }

        IEnumerator TippingOver(Vector3 pos)
        {
            if (run) yield break;
            run = true;

            owner._TipOver = true;
            WaveSetting.Setting.WaveSizeSet(1.5f);
            WaveSetting.Setting.Instance(1, pos, "EnemyWave");
            MissionManager.Misson.Enemy_Shot_Up(-1);

            yield return new WaitForSeconds(owner._DownTime);

            owner._TipOver = false;

            if (!owner.IsCurrentState(EnemyState.Hit)) { yield break; }
            iTween.RotateTo(owner.gameObject, iTween.Hash("x", 0));

            yield return new WaitForSeconds(1);

            owner._Speed *= 1.3f;
            owner._Speed = Mathf.Clamp(owner._Speed, owner._DefaultSpeed, owner._DefaultSpeed * 2);

            if (owner.IsCurrentState(EnemyState.Hit))
            {
                owner.ChangeState(EnemyState.Wait);
            }
        }
    }

    private class StateRevival : IState<EnemyRoutine>
    {
        public StateRevival(EnemyRoutine owner) : base(owner)
        { }

        bool flag = false;
        Vector3 vec;

        public override void Initialize()
        {
            vec = owner._vec;
            vec.y = 1;
            owner._Blowoff.blowoff(owner._rd, vec, 1, 5);
            iTween.RotateTo(owner.gameObject, iTween.Hash("x", 0));
            MissionManager.Misson.Enemy_Shot_Up(1);
        }

        public override void Execute()
        {
            if (owner.IsGround && flag)
            {
                owner.ChangeState(EnemyState.Wait);
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
