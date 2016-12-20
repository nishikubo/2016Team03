using UnityEngine;
using System.Collections;

public class ChackPCollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Pcollide = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            Pcollide = false;
        }
    }

    public bool Pcollide
    {
        get;
        set;
    }
}
