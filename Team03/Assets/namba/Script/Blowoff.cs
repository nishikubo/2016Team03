using UnityEngine;
using System.Collections;

public class Blowoff : MonoBehaviour {

    /// <summary>
    /// 吹き飛ばします。
    /// </summary>
    /// <param name="rd">飛ばしたいヤツのRigidbody</param>
    /// <param name="vec">飛ばすベクトル</param>
    /// <param name="power">飛ばす際のパワー</param>
    public void blowoff(Rigidbody rd, Vector3 vec, float power)
    {
        rd.AddForce(vec * power, ForceMode.Impulse);
    }
}
