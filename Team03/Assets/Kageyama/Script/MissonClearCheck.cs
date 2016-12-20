using UnityEngine;
using System.Collections;

public class MissonClearCheck : MonoBehaviour
{
    private int _clearCheck;

	// Use this for initialization
	void Start ()
    {
        _clearCheck = 0;
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void AddClearCount()
    {
        _clearCheck += 1;
        print(_clearCheck);
    }

    public int GetClear()
    {
        return _clearCheck;
    }
}
