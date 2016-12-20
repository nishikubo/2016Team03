using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClearScroll : MonoBehaviour
{
    private RectTransform _my_rect;
    [SerializeField]
    private float _scroll_time;

	// Use this for initialization
	void Start ()
    {
        _my_rect = this.GetComponent<RectTransform>();

        LeanTween.move(_my_rect, new Vector3(0, 1400), _scroll_time)
        .setDelay(3)
        .setOnComplete(() =>
        {
            StartCoroutine(SceneChange());
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2.0f);
        SceneChangeScript.sceneChange.FadeOut("Title");
    }
}
