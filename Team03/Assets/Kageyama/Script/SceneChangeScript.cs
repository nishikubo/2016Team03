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
        Fade_Object.SetActive(true);
        FadeIn();
        if(intime <= 0)
        {
            intime = 0.1f;
        }

        if(outtime <= 0)
        {
            outtime = 0.1f;
        }
    }

    //フェード状態を返す(falseだったらシーン移動可能)
    public void FadeFalse()
    {
        FadeStart = false;
    }

    //フェードイン
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

    //フェードアウトによるシーン移動(番号参照)
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
    //フェードアウトによるシーン移動(名前参照)
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

    //シーン移動(番号参照)
    public void SceneOut(int number)
    {
        SceneManager.LoadScene(number);
    }

    //シーン移動(名前参照)
    public void SceneOut(string name)
    {
        SceneManager.LoadScene(name);
    }

    //ゲームを終了する
    public void Quit()
    {
        Application.Quit();
    }

    //シーン移動せずに画面全体を薄暗くする
    public void FadeBlack()
    {
        if (FadeStart == true)
        {
            return;
        }
        Fade.GetComponent<Image>().enabled = true;
        FadeStart = true;
        LeanTween.alpha(Fade, 0.5f, outtime);
    }
}
