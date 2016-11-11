using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Wait,
    Pursuit,
    Attack,
    Died
}

public class EnemyRoutine : EnemyBase<EnemyRoutine, EnemyState>
{
    private Transform _target;
    private Rigidbody _rd;
    private NavMeshAgent _Agent;

    public string _TargetTag = "Player";
    public string _FieldTag = "Floor";
    public float _SearchArea = 10;
    public float _AttackArea = 5;
    public float _rotateSmooth = 3.0f;
    public float _Speed = 5;
    public float _JumpPower = 5;
    public int _life = 5;
    public string[] _outlayer = new string[] { "" };
    public bool _IsGround = false;
    public bool _IsHitPlayer = false;

    public Vector3[] _LoiteringPos;

    private PosGenerator _PosGene;

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

        // 各種値の初期化
        _LoiteringPos = new Vector3[_PosGene.GetPos.Length];
        for(int i = 0; i < _LoiteringPos.Length; i++)
        {
            _LoiteringPos[i] = _PosGene.GetPos[i].position;
        }


        transform.FindChild("LoiteringPositions").parent = null;

        // Stateの初期設定
        statelist.Add(new StateWait(this));
        statelist.Add(new StatePursuit(this));
        statelist.Add(new StateAttack(this));
        statelist.Add(new StateDied(this));

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

    // ダメージ処理
    public void Hit(int damage)
    {
        Debug.Log("ﾋｯﾄｫｫ!!");
        _life -= damage;
        if (_life <= 0)
        {
            ChangeState(EnemyState.Died);
        }

    }


    // 検知以下二つ
    public void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = true;
        if (col.collider.tag == _TargetTag) _IsHitPlayer = true;
    }
    public void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == _FieldTag) IsGround = false;
        if (col.collider.tag == _TargetTag) _IsHitPlayer = false;
    }

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

        public override void Initialize()
        {
            Debug.Log("徘徊状態ﾃﾞｽ");
            owner._rd.isKinematic = true;
            owner._Agent.enabled = true;
        }

        public override void Execute()
        {
            if (owner.Search(owner._target, owner.transform, owner._SearchArea, owner._outlayer, owner._TargetTag))
            {
                owner.ChangeState(EnemyState.Pursuit);
                return;
            }

            Vector3 pos = owner._LoiteringPos[cureentroot];

            // 徘徊処理
            if (Vector3.Distance(owner.transform.position, pos) < 1)
            {
                cureentroot += 1;
                cureentroot = cureentroot % owner._LoiteringPos.Length;
            }

            owner._Agent.SetDestination(pos);
        }

        public override void End()
        {
            owner._Agent.enabled = false;
            owner._rd.isKinematic = false;
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
            Debug.Log("追跡状態にﾊｲﾘﾏｼﾀ!!");
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
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotate, Time.deltaTime * owner._rotateSmooth);

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
        bool flag = false;

        public override void Initialize()
        {
            Debug.Log("ｻｰﾁｱﾝﾄﾞﾃﾞｽﾄﾛｰｲ");
            // vec = Vector3.forward * owner._Speed;
            vec.y = owner._JumpPower;
        }

        public override void Execute()
        {
            if (owner.IsHitPlayer)
            {
                Debug.Log("Playerと衝突");
                vec.y = owner._JumpPower;
            }

            owner._rd.velocity = vec;
            vec.y += Physics.gravity.y * Time.deltaTime;

            if (owner.IsGround && flag)
            {
                Debug.Log("Take That, You Field!!");
                owner.ChangeState(EnemyState.Pursuit);
                return;
            }
            if (!owner.IsGround && !flag) flag = true;
        }

        public override void End()
        {
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
            Debug.Log("ｵﾗﾊｼﾝｼﾞﾏｯﾀﾀﾞｰ");
        }

        public override void Execute()
        {
        }

        public override void End()
        {
        }
    }
}
