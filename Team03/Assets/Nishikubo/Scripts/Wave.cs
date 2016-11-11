using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {

    public WaveSetting wave;
    private GameObject player;

    private float distance = 0.0f;
    //private bool ear = false;
    

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        
	}
	
	// Update is called once per frame
	void Update () {

        Distance(0.0f);
        
        //Debug.Log(set);

        //switch ()
        //{
        //    case int n:
        //        Console.WriteLine("整数 " + n);
        //        break;
        //    case string s:
        //        Console.WriteLine("文字列 " + s);
        //        break;
        //    default:
        //        Console.WriteLine("その他");
        //        break;
        //}
    }

    void Distance(float dis)
    {
        //地面とプレイヤーの距離
        dis = Vector3.Distance(new Vector3(0, player.transform.position.y, 0), new Vector3(0, transform.position.y, 0));
        //地面より上
        if (dis>5)
        {
            //ear = true;

            //wave.Instance(10, new Vector3(player.transform.position.x, transform.position.y + 0.1f, player.transform.position.z));
        }
        

        //地面に触れているとき
        else
        {
            //set = true;

        }
        //set = false;
        //地面に触れてないときの最大値
        /*
        if(set==false)
        {

        }

        //Debug.Log("dis " + dis);
        if (dis > 2)
        {
            //Debug.Log(true);
            //set = false;
        }*/
        
        //switch(dis)
        //{
        //    case float 
        //}
    }

    //波のループ
    /*IEnumerator ShockWave(int num, float interval)
    {
        float playerX = player.transform.position.x;
        float playerZ = player.transform.position.z;
        //プレイヤーと床の距離みて波の大きさ変化
        
        
        for (int i = 0; i < num; i++)
        {
            Instantiate(wave, new Vector3(playerX, transform.position.y + 0.01f, playerZ), player.transform.rotation);
            yield return new WaitForSeconds(interval);
        }
        
    }*/

    //触れたときに1回
    void OnTriggerEnter(Collider col)
    {
        //set = true;
        //Debug.Log(col);

        if (col.tag == "Player" )
        {
            distance = Vector3.Distance(new Vector3(0, player.transform.position.y, 0), new Vector3(0, transform.position.y, 0));
            if (distance>5)
            {

            }
            
            //Debug.Log("player");
            //player = col.gameObject;
            //wave.Instance(10, new Vector3(player.transform.position.x, transform.position.y+0.1f, player.transform.position.z) /*+ new Vector3(0, 1.5f, 0)*/);
            
        
        
        //Instantiate(waveEffect, new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z), player.transform.rotation);
            //StartCoroutine(ShockWave(5, 0.1f));
        }
        

    }

    void OnTriggerExit(Collider col)
    {

        //Debug.Log("exit");
    }

}
