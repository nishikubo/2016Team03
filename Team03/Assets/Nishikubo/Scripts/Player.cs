using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //重力加速度
    private float gravity = 9.8f;
    //移動する方向ベクトル
    private Vector3 moveDirection = Vector3.zero;
    //移動速度
    public float speed = 1.0f;
    //プレイヤーのジャンプ力
    public float jumpPower = 10.0f;
    //CharacterController取得
    private CharacterController controller;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update () {
        Move();
    }

    //移動処理
    void Move()
    {

        float y = moveDirection.y;
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        moveDirection.y += y;
        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpPower;
            }
        }
    }
}
