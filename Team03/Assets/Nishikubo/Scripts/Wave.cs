using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {

    //prefub
    public GameObject waveEffect;
    public GameObject wave;
    private GameObject player;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(wave, transform.position, transform.rotation);
        }
    }


    IEnumerator ShockWave(int num, float interval)
    {
        //Instantiate(waveEffect, transform.position, transform.rotation);

        for (int i = 0; i < num; i++)
        {
            //Debug.Log(i);
            //Instantiate(wave, player.transform.position, player.transform.rotation);
            Instantiate(wave, new Vector3(player.transform.position.x, transform.position.y+0.01f, player.transform.position.z), player.transform.rotation);
            yield return new WaitForSeconds(interval);
        }
    }

    //触れたときに1かいだけ
    void OnTriggerEnter(Collider col)
    {
        if(col.tag=="Player")
        {
            //Debug.Log(col.gameObject.tag);
            player = col.gameObject;
            
            //Instantiate(waveEffect, player.transform.position, player.transform.rotation);
            Instantiate(waveEffect, new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z), player.transform.rotation);
            StartCoroutine(ShockWave(5, 0.1f));
        }
    }

}
