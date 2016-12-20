using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    private bool _stop;
    public RectTransform _resumption;
    
    [SerializeField]
    private RectTransform _fade;
    private bool _fade_frag;
    [SerializeField]
    private GameObject _missonUI;
    [SerializeField]
    private GameObject _waku;

    // Use this for initialization
    void Start ()
    {
        _fade_frag = false;
        _stop = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_stop == false)
            {
                if (_fade_frag == true) return;

                _stop = true;
                _fade.GetComponent<Image>().enabled = true;
                _fade_frag = true;
                LeanTween.alpha(_fade, 0.5f, 0.1f)
                    .setOnComplete(() =>
                    {
                        _fade_frag = false;
                        _missonUI.SetActive(true);
                        _resumption.GetComponent<Button>().Select();
                        _waku.GetComponent<RectTransform>().localPosition = new Vector2(-200, -100);
                        _waku.GetComponent<Frame>()._resumption = true;
                        Time.timeScale = 0;
                    });
            }
            else
            {
                Resumption();
            }
        }
    }

    public void Resumption()
    {
        if (_fade_frag == true) return;
        _missonUI.SetActive(false);
        Time.timeScale = 1;
        _fade_frag = true;
        LeanTween.alpha(_fade, 0, 0.1f)
            .setOnComplete(() =>
            {
                _fade_frag = false;
                _stop = false;
            });
    }

    public void TitleExit()
    {
        if (_fade_frag == true) return;
        _missonUI.SetActive(false);
        Time.timeScale = 1;
        _fade_frag = true;
        LeanTween.alpha(_fade, 1.0f, 0.5f)
            .setOnComplete(() =>
            {
                SceneChangeScript.sceneChange.SceneOut("Title");
            });
    }
}
