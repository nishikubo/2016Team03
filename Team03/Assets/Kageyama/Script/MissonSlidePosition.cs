using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MissonSlidePosition : MonoBehaviour
{
    private static int number;
    private static int check;
    // Use this for initialization
    void Start()
    {
        number = 1;
        check = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Move(int num)
    {
        if(num  <= 0)
        {
            number = 1;
            this.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 + -30 * number, 0);
            return;
        }
        this.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 + -30 * number, 0);
        number += num;
        check = number;
    }

    int NumberCheck()
    {
        return check;
    }
}
