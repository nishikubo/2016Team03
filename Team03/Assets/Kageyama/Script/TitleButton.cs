using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    private RectTransform _selectButton;
    private RectTransform _myRectTransform;

    private float _scale;
    [SerializeField]
    private float _sizeChenge;
    [SerializeField]
    private float _maxSize;

    private bool _sizeUp;
	// Use this for initialization
	void Start ()
    {
        _myRectTransform = this.GetComponent<RectTransform>();
        _scale = 1;
        _sizeUp = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        _selectButton = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
        if(_myRectTransform == _selectButton)
        {
            if (_sizeUp == false)
            {
                _scale -= _sizeChenge;
            }
            else if(_sizeUp == true)
            {
                _scale += _sizeChenge;
            }

            _myRectTransform.localScale = new Vector3(_scale, _scale, 1);
            if (_scale >= _maxSize) _sizeUp = false;
            else if (_scale <= 1) _sizeUp = true;
        }
        else
        {
            _myRectTransform.localScale = new Vector3(1, 1, 1);
            _scale = 1;
        }
	}
}
