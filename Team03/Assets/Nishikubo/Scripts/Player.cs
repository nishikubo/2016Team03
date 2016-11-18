using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    //移動する方向ベクトル
    private Vector3 moveDirection = Vector3.zero;
    //移動速度
    public float speed = 1.0f;
    //プレイヤーのジャンプ力
    public float jumpPower = 10.0f;
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

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        ma = mass;

    }

    // Update is called once per frame
    void Update () {
        Move(9.8f);
    }

    //移動処理
    void Move(float gravity)
    {
        float y = moveDirection.y;
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        moveDirection.y += y;
        moveDirection.y -= gravity * Time.deltaTime * mass;

        controller.Move(moveDirection * Time.deltaTime);

        //移動範囲
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPosition, maxPosition), Mathf.Clamp(transform.position.y, minPosition, maxPosition), Mathf.Clamp(transform.position.z, minPosition, maxPosition));


        if (controller.isGrounded)
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
        if(!controller.isGrounded)
        {
            swooped = true;
            if(Input.GetButtonDown("Jump") && swooped == true)
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

    //ジャンプ処理
    void Jump()
    {
        moveDirection.y = jumpPower;
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
            WaveSetting.Setting.Instance(waveCount[0], new Vector3(transform.position.x, transform.position.y - transform.localScale.y , transform.position.z),"PlayerWave");

        }
        else if (distance >= hight[1])
        {
            //Debug.Log("衝撃波　中");
            WaveSetting.Setting.WaveSizeSet(waveSize[1]);
            WaveSetting.Setting.Instance(waveCount[1], new Vector3(transform.position.x, transform.position.y - transform.localScale.y , transform.position.z),"PlayerWave");
        }
        else if (distance >= hight[2])
        {
            //Debug.Log("衝撃波　小");
            WaveSetting.Setting.WaveSizeSet(waveSize[2]);
            WaveSetting.Setting.Instance(waveCount[2], new Vector3(transform.position.x, transform.position.y - transform.localScale.y , transform.position.z), "PlayerWave");
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
        if(hit.gameObject.tag=="Floor"&&jumped==true)
        {
            //Debug.Log("hit");

            Distance();
            jumped = false;
            StartCoroutine(InvincibleTime(0.5f));
            
        }
    }

}