using UnityEngine;
using System.Collections;

public class Player3 : MonoBehaviour {

    //移動する方向ベクトル
    private Vector3 moveDirection = Vector3.zero;
    //移動速度
    public float speed = 1.0f;
    //プレイヤーのジャンプ力
    //public float jumpPower = 10.0f;
    //ジャンプのカウント
    private float jumpCount = 0.0f;
    //ジャンプの最大値
    private int jumpMax = 20;
    //重さ
    public float mass = 1.0f;
    //CharacterController取得
    private CharacterController controller;



    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();

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
        //moveDirection.y -= gravity * Time.deltaTime * mass;

        //GetComponent<Rigidbody>().velocity = moveDirection*speed;
        GetComponent<Rigidbody>().AddForce(moveDirection, ForceMode.Acceleration);

        //if (damageFlag == false)
        //controller.Move(moveDirection * Time.deltaTime);

        //移動範囲
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPosition, maxPosition), Mathf.Clamp(transform.position.y, minPosition, maxPosition), Mathf.Clamp(transform.position.z, minPosition, maxPosition));






    }

    public void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag=="Floor")
        {
            Debug.Log("floor");
            if (Input.GetButton("Jump"))
            {
                jumpCount++;
                if (jumpCount >= jumpMax)
                {
                    jumpCount = jumpMax;
                    Debug.Log("jump----" + jumpCount);

                }
            }
            if (jumpCount > 0.0f && Input.GetButtonUp("Jump"))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * jumpCount / 2, ForceMode.VelocityChange);
                jumpCount = 0.0f;
            }

        }
    }

}
