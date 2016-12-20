using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameClearImage : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        RectTransform _myImage = this.GetComponent<RectTransform>();
        GameObject _camera = GameObject.Find("Main Camera");

        LeanTween.move(_myImage, new Vector2(0, 100), 1.5f)
            .setEase(LeanTweenType.easeInOutBounce)
            .setOnComplete(()=>
            {
                LeanTween.alpha(_myImage, 0, 1.5f)
                .setDelay(1.0f);
                //iTween.ShakePosition(_camera, iTween.Hash("x", 0.3f, "y", 0.3f, "time", 0.5f));
            });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
