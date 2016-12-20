using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    //移動する方向ベクトル
    private Vector3 moveDirection = Vector3.zero;
    //移動速度
    public float speed = 1.0f;
    //ジャンプのカウント
    public int jumpCount = 0;
    //ジャンプの最大値
    private int jumpMax = 30;
    //重さ
    public float mass = 1.0f;
    //CharacterController取得
    private CharacterController controller;
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
    //ダメージをうけたか
    private bool damageFlag = false;
    //hpUI
    public GameObject _HP;
    //衝撃波にあたったとき吹っ飛ぶか
    private bool blowed = false;
    private bool blow_t = false;
    //ジャンプできるかどうか
    private bool jump = false;
    //カメラ揺れ
    private GameObject camera_;
    //レバガチャのカウント
    private int revaCount = 0;


    private float reTime = 0.0f;

    //プレイヤーのアニメーション
    private Animator anim;

    //ジャンプ溜め中
    public GameObject _jumpCharge;
    private GameObject ch_par;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        camera_ = GameObject.Find("Main Camera");
        anim = GetComponent<Animator>();

        ma = mass;

        _jumpCharge.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        Move(9.8f);
    }

    //移動処理
    void Move(float gravity)
    {
        float x = moveDirection.x;
        float y = moveDirection.y;
        float z = moveDirection.z;
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;

        moveDirection.y += y;
        moveDirection.y -= gravity * Time.deltaTime * mass;


        //プレイヤーの向き
        Quaternion rota = Quaternion.LookRotation(transform.position + (Vector3.right * moveDirection.x) + (Vector3.forward * moveDirection.z) - transform.position);
        


        controller.Move(moveDirection * Time.deltaTime);


        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            transform.rotation = rota;
        }
        if (!Input.GetButton("Horizontal") || !Input.GetButton("Vertical"))
        {
            anim.SetBool("walk", false);
        }

        //移動範囲
        MoveRenge();

        if (controller.isGrounded)
        {
            
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                anim.SetBool("walk", true);
            }

            swooped = false;
            //Debug.Log("mass " + mass + " ma " + ma);
            mass = ma;

            //mass = 1.0f;
            if(Input.GetButton("Jump")&&jump==false)
            {
                anim.SetBool("save", true);

                JumpCharge();

                jumpCount++;

                if (jumpCount>=jumpMax)
                {
                    jumpCount = jumpMax;

                }
                //ジャンプ溜め中に移動したらスピード半減
                if(Input.GetButton("Horizontal")|| Input.GetButton("Vertical"))
                {
                    //Debug.Log("osareta-");
                    speed = 2;
                }

            }
            if (jumpCount>0 && Input.GetButtonUp("Jump")&&jump==false)
            {
                anim.SetBool("save", false);
                anim.SetBool("air", true);
                //Destroy(ch_par);
                _jumpCharge.GetComponent<Renderer>().enabled = false;



                Jump();
                jumpCount = 0;
                speed = 5;
            }

            //地面についたら加速しない
            if (blow_t == true)
            {
                //Debug.Log("tuiteruyo-------");

                //敵に吹っ飛ばされた衝撃で麻痺状態
                reTime += Time.deltaTime;
                //Debug.Log((int)reTime);
                //レバガチャ入力で復帰
                if(reTime<7.0f)
                {
                    //Debug.Log("reva");
                    Revergacha(20);

                }

                //時間経過で復帰                
                if (reTime>=7.0f)
                {
                    //Debug.Log("time");
                    speed = 5;
                    blowed = false;
                    blow_t = false;
                    reTime = 0.0f;
                    jump = false;
                    //GetComponent<Renderer>().material.color = Color.cyan;
                }

                //通常に戻す
                /*speed = 5;
                blowed = false;
                blow_t = false;*/
                //moveDirection.x = 0;


            }
        }
        if (!controller.isGrounded)
        {
            //swooped = true;
            if (Input.GetButtonDown("Jump") && swooped == true)
            {
                Swoop();
                //地面
                if (Physics.Raycast(transform.position, Vector3.down, out floorhit))
                {
                    if (floorhit.collider.tag == "Floor")
                    {
                        Wave();
                    }
                }
            }
            //浮いてる方向に加速
            if (blowed == true)
            {
                moveDirection.x += x;
                moveDirection.z += z;
                controller.Move(moveDirection * Time.deltaTime);
            }
        }
        //anim.SetFloat("Speed", Mathf.Abs(Vector3.Magnitude(moveDirection - new Vector3(0, moveDirection.y, 0))));

    }

    //ジャンプ処理　押された長さでジャンプ力変える
    void Jump()
    {
        moveDirection.y = jumpCount/5;
        swooped = true;
    }
    //ジャンプチャージ中
    void JumpCharge()
    {
        //ch_par = (GameObject)Instantiate(_jumpCharge, transform.position, new Quaternion(0, 0, 0, 0));
        //ch_par.transform.parent = transform;

        _jumpCharge.GetComponent<Renderer>().enabled = true;


    }

    //急降下
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
        jumped = true;
    }

    //高さから衝撃波生成
    void Distance()
    {
        //生成と同時にカメラを揺らす
        iTween.ShakePosition(camera_, iTween.Hash("x", 0.3f, "y", 0.3f, "time", 0.5f));

        if (distance >= hight[0])
        {
            //Debug.Log("衝撃波　大");
            WaveSetting.Setting.WaveSizeSet(waveSize[0]);
            WaveSetting.Setting.Instance(waveCount[0], new Vector3(transform.position.x, transform.position.y /*- transform.localScale.y*/, transform.position.z), "PlayerWave");

        }
        else if (distance >= hight[1])
        {
            //Debug.Log("衝撃波　中");
            WaveSetting.Setting.WaveSizeSet(waveSize[1]);
            WaveSetting.Setting.Instance(waveCount[1], new Vector3(transform.position.x, transform.position.y  /*- transform.localScale.y*/, transform.position.z), "PlayerWave");

        }
        else if (distance >= hight[2])
        {
            //Debug.Log("衝撃波　小");
            WaveSetting.Setting.WaveSizeSet(waveSize[2]);
            WaveSetting.Setting.Instance(waveCount[2], new Vector3(transform.position.x, transform.position.y  /*- transform.localScale.y*/, transform.position.z), "PlayerWave");
        }
    }

    //移動制限
    void MoveRenge()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPosition, maxPosition), Mathf.Clamp(transform.position.y, minPosition, maxPosition), Mathf.Clamp(transform.position.z, minPosition, maxPosition));
    }

    //衝撃波を出した後
    IEnumerator InvincibleTime(float interval)
    {
        float sp = speed;
        speed = 0;
        yield return new WaitForSeconds(interval);
        anim.SetBool("down", false);
        speed = sp;
    }

    //敵に当たったあと
    IEnumerator Knockback(float interval)
    {
        float sp = speed;
        //敵とは反対側に
        //ノックバック
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", transform.position - (transform.forward * 2.0f),
            "time", 0.4f,
            "easetype", iTween.EaseType.linear,
            "oncomplete", "onInvincibleState",
            "oncompletetarget", gameObject
        ));
        //移動できなくする
        speed = 0;
        yield return new WaitForSeconds(interval);
        speed = sp;
        damageFlag = false;
        jump = false;
        gameObject.transform.FindChild("mqo_mesh").GetComponent<Renderer>().enabled = true;
    }

    //点滅させるだけ
    IEnumerator Flashing()
    {
        while (damageFlag==true)
        {
            //GameObject mesh = gameObject.transform.FindChild("mqo_mesh").gameObject;
            //mesh.GetComponent<Renderer>().enabled = !mesh.GetComponent<Renderer>().enabled;
            gameObject.transform.FindChild("mqo_mesh").GetComponent<Renderer>().enabled = !gameObject.transform.FindChild("mqo_mesh").GetComponent<Renderer>().enabled;

            yield return new WaitForSeconds(0.1f);
        }
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //地面に当たってる 衝撃波出した後
        if (hit.gameObject.tag == "Floor" && jumped == true)
        {
            //Debug.Log(hit.gameObject.transform.position);
            anim.SetBool("air",false);
            anim.SetBool("down",true);

            Distance();
            jumped = false;


            StartCoroutine(InvincibleTime(0.5f));
        }
        //地面に当たってる　衝撃波出さない
        if (hit.gameObject.tag == "Floor" && jumped == false)
        {
            anim.SetBool("air", false);
        }
        //敵に当たる
        if ((hit.gameObject.tag == "Enemy"||(hit.gameObject.tag=="Building"&&hit.gameObject.GetComponent<BuilBlow>()._IsGround==false))&&damageFlag==false)
        {
            damageFlag = true;
            jump = true;

            //HPが1つ減る
            if (damageFlag == true)
            {
                Debug.Log(hit.gameObject.name);
                _HP.GetComponent<HP>().Damage();
                StartCoroutine(Flashing());               
                StartCoroutine(Knockback(3.0f));
            }
        }

    }

    public void OnTriggerEnter(Collider col)
    {
        //敵の衝撃波と当たったら吹っ飛ばす
        if ((col.tag == "EnemyWave" || col.tag == "ObjectWave") && damageFlag==false )
        {
            blowed = true;
            swooped = false;
            jump = true;
            revaCount = 0;

            GameObject enm = col.gameObject;
            Vector3 enmVec = enm.transform.position;
            Vector3 dis = enmVec-transform.position ;
            dis = dis.normalized;

            moveDirection = transform.position - (new Vector3(dis.x, dis.y*5, dis.z) * 2);
            
            StartCoroutine(EnemyWaveHit());

        }
    }

    //敵の衝撃波に当たった
    IEnumerator EnemyWaveHit()
    {
        //敵の進行方向と逆に吹っ飛ばす
        //プレイヤー入力負荷
        //float sp = speed;
        speed = 0;


        yield return new WaitForSeconds(0.5f);
        blow_t = true;

        //Debug.Log("tuita--");

        //GetComponent<Renderer>().material.color = Color.red;

    }

    //レバガチャで復帰
    void Revergacha(int max)
    {
        if(revaCount<max)
        {
            //damageFlag = true;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                revaCount++;
                //Debug.Log("count Q:" + revaCount);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                revaCount++;
                //Debug.Log("count W:" + revaCount);

            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                revaCount++;
                //Debug.Log("count E:" + revaCount);

            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                revaCount++;
                //Debug.Log("count R:" + revaCount);

            }

        }
        //指定回数以上押されたら
        if (revaCount>=max)
        {
            revaCount = max;


            //GetComponent<Renderer>().material.color = Color.cyan;

            //通常に戻す
            //damageFlag = false;
            jump = false;
            speed = 5;
            blowed = false;
            blow_t = false;
            //Debug.Log("return" + revaCount);

        }


    }

}