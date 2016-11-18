using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Btest : MonoBehaviour {

    [SerializeField]
    private ParticleSystem _pt;
    private Button _button;
    private Image _image;

    public Color _color;
    private Color _currentcolor;
	// Use this for initialization
	void Start () {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();

        _currentcolor = _image.color;	
	}
	
	// Update is called once per frame
	void Update () {
        if (_pt.isPlaying)
        {
            _button.enabled = false;
            _image.color = _color;
        }
        else
        {
            _button.enabled = true;
            _image.color = _currentcolor;
        }
    }
}
