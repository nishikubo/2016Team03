using UnityEngine;
using System.Collections;

public class ptest : MonoBehaviour
{

    private ParticleSystem _pt;
    public float _time = 5;

    // Use this for initialization
    void Start()
    {
        _pt = GetComponent<ParticleSystem>();
        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(_time);

        _pt.Stop();

        yield return new WaitWhile(() => _pt.IsAlive(true));

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(Lifetime());
    }
}
