using UnityEngine;
using System.Collections;

public class BuilBlow : MonoBehaviour
{
    private Blowoff _Blowoff;
    private Rigidbody _rd;

    private bool _IsGround = false;
    private bool _flag = false;
    private bool _Ishit = false;

    #region ---衝撃波のColor---
    [SerializeField] private Color _BuilWaveColor;
    #endregion

    public void Start()
    {
        _Blowoff = GetComponent<Blowoff>();
        _rd = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(_IsGround && _Ishit)
        {
            _Ishit = false;
            StartCoroutine(Wait());
        }
        if (!_IsGround && !_Ishit) _Ishit = true;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerWave" || col.tag == "EnemyWave")
        {
            if (_flag) return;

            Vector3 vec = (transform.position - col.transform.position).normalized;
            vec *= 0;
            vec.y = 1;
            _Blowoff.blowoff(_rd, vec, 10);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Floor") _IsGround = true;
    }
    public void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Floor") _IsGround = false;
    }
    IEnumerator Wait()
    {
        _flag = true;

        Vector3 pos = transform.position;
        pos.y -= transform.lossyScale.y * 0.5f - 0.1f;

        WaveSetting.Setting.WaveSizeSet(2);
        WaveSetting.Setting.SettingColor(_BuilWaveColor);
        WaveSetting.Setting.Instance(1, pos, "ObjectWave");

        yield return new WaitForSeconds(1);

        _flag = false;
    }
}
