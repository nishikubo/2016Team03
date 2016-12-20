using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    protected static SceneChangeScript Change;
    [SerializeField]
    private GameObject Fade_Object;
    [SerializeField]
    private RectTransform Fade;
    //フェードインの時間
    public float intime;
    //フェードアウトの時間
    public float outtime;
    //シーン移動開始のフラグ
    private bool FadeStart = true;

    //どこでも参照可
    public static SceneChangeScript sceneChange
    {
        get
        {
            if (Change == null)
            {
                Change = (SceneChangeScript)FindObjectOfType(typeof(SceneChangeScript));
                if (Change == null)
                {
                    Debug.LogError("SceneChange Instance Error");
                }
            }

            return Change;
        }
    }

    void Awake()
    {
        Fade = Fade_Object.GetComponent<RectTransform>();
        Cursor.visible = false; //カーソル非表示
        Fade_Object.SetActive(true);
        FadeIn();
        if (intime <= 0)
        {
            intime = 0.1f;
        }

        if (outtime <= 0)
        {
            outtime = 0.1f;
        }
    }

    /// <summary>
    ///フェード状態を返す(falseだったらシーン移動可能) 
    /// </summary>
    public void FadeFalse()
    {
        FadeStart = false;
    }

    /// <summary>
    ///フェードイン 
    /// </summary>
    public void FadeIn()
    {
        Fade.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        LeanTween.alpha(Fade, 0.0f, intime)
            .setOnComplete(() =>
            {
                Fade.GetComponent<Image>().enabled = false;
                FadeFalse();
            });
    }

    /// <summary>
    ///フェードアウトによるシーン移動(番号参照) 
    /// </summary>
    /// <param name="number"></param>
    public void FadeOut(int number)
    {
        if (FadeStart == true)
        {
            return;
        }
        Fade.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Fade.GetComponent<Image>().enabled = true;
        FadeStart = true;
        LeanTween.alpha(Fade, 1, outtime)
            .setOnComplete(() =>
            {
                SceneOut(number);
            });
    }

    /// <summary>
    ///フェードアウトによるシーン移動(名前参照) 
    /// </summary>
    /// <param name="name"></param>
    public void FadeOut(string name)
    {
        if (FadeStart == true)
        {
            return;
        }
        Fade.GetComponent<Image>().enabled = true;
        FadeStart = true;
        LeanTween.alpha(Fade, 1, outtime)
            .setOnComplete(() =>
            {
                SceneOut(name);
            });
    }

    /// <summary>
    /// シーン移動(番号参照)
    /// </summary>
    /// <param name="number"></param>
    public void SceneOut(int number)
    {
        SceneManager.LoadScene(number);
    }

    /// <summary>
    /// シーン移動(名前参照)
    /// </summary>
    /// <param name="name"></param>
    public void SceneOut(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// シーン移動せずに画面全体を薄暗くする
    /// </summary>
    public void FadeBlack()
    {
        if (FadeStart == true)
        {
            return;
        }
        Fade.GetComponent<Image>().enabled = true;
        FadeStart = true;
        LeanTween.alpha(Fade, 0.5f, 0.1f)
            .setOnComplete(() =>
            {
                FadeFalse();
            });
    }

    /// <summary>
    /// 暗くなっている画面を明るくする
    /// </summary>
    public void FadeWhite()
    {
        if (FadeStart == true)
        {
            return;
        }

        Fade.GetComponent<Image>().enabled = true;
        FadeStart = true;
        LeanTween.alpha(Fade, 0, 0.1f)
            .setOnComplete(() =>
            {
                FadeFalse();
            });
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }
}
