using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player2 : MonoBehaviour {

    //移動する方向ベクトル
    //private Vector3 moveDirection = Vector3.zero;
    //移動速度
    public float speed = 1.0f;
    //プレイヤーのジャンプ力
    public float jumpPower = 10.0f;
    //重さ
    public float mass = 1.0f;
    //CharacterController取得
    //private CharacterController controller;
    //急降下中か
    private bool swooped = false;
    private bool jumped = false;
    //地面へ（真下）
    RaycastHit floorhit;
    //急降下した地点
    private float distance = 0.0f;
    //質量保存
    private float ma = 0.0f;
    //高さ
    public List<float> hight = new List<float>();
    //波の大きさ
    public List<float> waveSize = new List<float>();
    //波の量
    public List<int> waveCount = new List<int>();
    //プレイヤーの移動範囲
    public float maxPosition = 20.0f;
    public float minPosition = -20.0f;

    //private Blowoff _Blowoff;
    private Rigidbody _rd;

    //private bool _waveHit = false;

    // Use this for initialization
    void Start()
    {
        //controller = GetComponent<CharacterController>();
        ma = mass;
        //_Blowoff = GetComponent<Blowoff>();
        _rd = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -100.0f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //_rd.AddForce(new Vector3(0,9.8f,0), ForceMode.Acceleration);
    }

    //移動処理
    void Move()
    {
        /* float y = moveDirection.y;
         moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
         moveDirection *= speed;*/
        //moveDirection.y += y;
        //moveDirection.y -= gravity * Time.deltaTime * mass;

        //controller.Move(moveDirection * Time.deltaTime);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, z).normalized;
        _rd.velocity = direction * speed;
        //_rd.MovePosition(moveDirection * Time.deltaTime);
        //_rd.velocity = moveDirection * speed;
        //GetComponent<Rigidbody>().velocity += (Vector3.forward * 0.1f) * Time.deltaTime / _rd.mass;
        //_rd.AddForce(moveDirection * Time.deltaTime);

        //transform.position += transform.forward *Time.deltaTime;

        //移動範囲
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPosition, maxPosition), Mathf.Clamp(transform.position.y, minPosition, maxPosition), Mathf.Clamp(transform.position.z, minPosition, maxPosition));



        //地面と接触してるとき
        /*if (controller.isGrounded)
        {
            swooped = false;
            //Debug.Log("mass " + mass + " ma " + ma);
            mass = ma;

            //mass = 1.0f;
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }      
        }*/
        //地面と接触していないとき
        //if(!controller.isGrounded)
        {
            swooped = true;
            if (Input.GetButtonDown("Jump") && swooped == true)
            {
                Swoop();
                //地面
                if (Physics.Raycast(transform.position, Vector3.down, out floorhit))
                {
                    if (floorhit.collider.tag == "Floor")
                    {
                        //Debug.Log("hit");
                        Wave();
                    }
                }

            }

        }
    }

    //ジャンプ処理　押された長さでジャンプ力変える
    void Jump()
    {
        _rd.AddForce(Vector3.up * speed, ForceMode.Impulse);

        //moveDirection.y = jumpPower;
        //Destroy(GetComponent<Rigidbody>());
    }

    //急降下 ジャンプ中に押されたら
    void Swoop()
    {
        mass = mass * 10;
        swooped = false;
    }

    //高いところから落ちたときの距離
    void Wave()
    {
        float dis = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, 0, 0));
        distance = dis;
        //Debug.Log(dis +" "+ distance);
        jumped = true;
        //Distance();
        //StartCoroutine(ShockWave(1.0f));
    }

    //高さから衝撃波生成
    void Distance()
    {
        if (distance >= hight[0])
        {
            //Debug.Log("衝撃波　大");
            WaveSetting.Setting.WaveSizeSet(waveSize[0]);
            WaveSetting.Setting.Instance(waveCount[0], new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), "PlayerWave");

        }
        else if (distance >= hight[1])
        {
            //Debug.Log("衝撃波　中");
            WaveSetting.Setting.WaveSizeSet(waveSize[1]);
            WaveSetting.Setting.Instance(waveCount[1], new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), "PlayerWave");
        }
        else if (distance >= hight[2])
        {
            //Debug.Log("衝撃波　小");
            WaveSetting.Setting.WaveSizeSet(waveSize[2]);
            WaveSetting.Setting.Instance(waveCount[2], new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), "PlayerWave");
        }
    }

    //衝撃波を出した後
    IEnumerator InvincibleTime(float interval)
    {
        //Debug.Log("Invincible time");
        float sp = speed;
        speed = 0;
        yield return new WaitForSeconds(interval);
        speed = sp;
        //Debug.Log("finish");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Floor" && jumped == true)
        {
            //Debug.Log("hit");

            Distance();
            jumped = false;
            StartCoroutine(InvincibleTime(0.5f));

        }
    }

    public void OnTriggerEnter(Collider col)
    {
        //敵の衝撃波と当たったら吹っ飛ばす
        if (col.tag == "EnemyWave")
        {
            //Debug.Log("atari");
            //敵の進行方向と逆に吹っ飛ばす
            //プレイヤー入力負荷

            //EnemyWaveHit();

            //moveDirection = new Vector3(0, 10, 0);

            //GameObject enemyWave = col.gameObject;
            //Vector3 enemyVec = enemyWave.transform.position;
            //float dis=Vector3.Distance(transform.position, enemyVec);
            //Vector3 dis = enemyVec - transform.position;

            //            Vector3 dis=transform.position;

            //Debug.Log(enemyWave.tag+"  vec: "+enemyVec+"  dis "+dis);
            //float x = moveDirection.x;
            //moveDirection.x += x;

            //moveDirection = new Vector3(0, 10, 0);
            //_rd = gameObject.AddComponent<Rigidbody>();


        }
    }

    //敵の衝撃波に当たった
    void EnemyWaveHit()
    {
        //_waveHit = true;
        //_rd = gameObject.AddComponent<Rigidbody>();
        //_rd=GetComponent<Rigidbody>();
        //_rd.AddForce(transform.position * 10, ForceMode.Impulse);
        //Debug.Log(_rd);


        //Vector3 vec = (transform.position /*- col.transform.position*/).normalized;
        /*
        vec /= 2;
        vec.y = 1;
        _rd.AddForce(vec * 10, ForceMode.Impulse);
        */
        //_Blowoff.blowoff(_rd, vec, 10);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Floor")
        {
            swooped = false;
            //Debug.Log("mass " + mass + " ma " + ma);
            mass = ma;

            //mass = 1.0f;
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
    }
}
