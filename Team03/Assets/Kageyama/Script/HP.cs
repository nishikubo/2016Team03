using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HP : MonoBehaviour
{
    public GameObject _playerHp;
    private int number = 5;

    void Awake()
    {
    }

	// Use this for initialition
	void Start ()
    {
        for (int i = 0; i < number; i++)
        {
            GameObject a = Instantiate(_playerHp);
            a.transform.SetParent(this.gameObject.transform, false);
            a.GetComponent<RectTransform>().localPosition = new Vector3(0 + i * 30, 0, 0);
            a.name = "HPImage" + (i+1);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Damage();
        }
	}


    public void Damage()
    {
        GameObject child = transform.FindChild("HPImage" + number).gameObject;
        if(child != null)
        {
            Destroy(child);
            number--;
        }
        if (number <= 0)
        {
            SceneChangeScript.sceneChange.SceneOut("GameOver");
            return;
        }

    }
}
