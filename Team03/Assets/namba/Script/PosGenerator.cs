using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PosGenerator : MonoBehaviour
{
    public int _PosSize = 0;
    public GameObject _obj;

    private string _PosName = "Position";
    public Transform[] _Positions;
    // private List<Transform> _Positions = new List<Transform>();

    void Update()
    {
        // 生成するオブジェクトがセットされていなければreturn
        if (_obj == null) { return; }

        if(_PosSize != _Positions.Length)
        {
            Array.Resize(ref _Positions, _PosSize);
        }

        // 数が変わらない場合はポジション間に線を描画
        if (_Positions.Length == transform.childCount)
        {

            if (transform.childCount >= 1)
            {
                Debug.DrawRay(this.transform.position, _Positions[0].position - this.transform.position);
                Debug.DrawRay(this.transform.position, _Positions[_Positions.Length - 1].position - this.transform.position);
                if (transform.childCount >= 2)
                {
                    for (int i = 0; i < _Positions.Length - 1; i++)
                    {
                        Vector3 dir = _Positions[i + 1].position - _Positions[i].position;
                        Debug.DrawRay(_Positions[i].position, dir);
                    }
                }
            }

            return;
        }

        Generator();
    }

    // positionの増減
    void Generator()
    {
        // 数を増やす時
        if (_Positions.Length > transform.childCount)
        {
            for (int i = transform.childCount; i < _Positions.Length; i++)
            {
                GameObject obj = GameObject.Instantiate(_obj, this.transform.position, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                obj.name = _PosName + i;
                _Positions[i] = obj.transform;
            }

            return;
        }

        // 数を減らす時
        if (_Positions.Length < transform.childCount)
        {
            for (int i = transform.childCount; i > _Positions.Length; i--)
            {
                DestroyImmediate(transform.FindChild(_PosName + (i - 1)).gameObject);
            }
        }
    }

    public Transform[] GetPos
    {
        get {
            Transform[] positions = new Transform[_Positions.Length + 1];
            positions[0] = this.transform;
            _Positions.CopyTo(positions, 1);
            return positions;
        }
    }
}
