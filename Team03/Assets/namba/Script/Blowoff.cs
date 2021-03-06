﻿using UnityEngine;
using System.Collections;

public class Blowoff : MonoBehaviour {

    /// <summary>
    /// 吹き飛ばします。
    /// </summary>
    /// <param name="rd">飛ばしたいヤツのRigidbody</param>
    /// <param name="vec">飛ばすベクトル</param>
    /// <param name="power">飛ばす際のパワー</param>
    public void blowoff(Rigidbody rd, Vector3 vec, float power = 1.0f, float heightp = 10.0f)
    {
        vec.y *= heightp;
        vec.x *= power;
        vec.z *= power;
        rd.AddForce(vec, ForceMode.Impulse);
    }
}
