using UnityEngine;
using UnityEngine.UI;

public class InventoryCells : MonoBehaviour {

	[SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    public string item { get; set; }
	public bool isLocked { get; set; }
    public bool isInventory { get; set; }

    public RectTransform rectTransform
	{
		get{ return _rectTransform; }
	}

    public Image image
    {
        get { return _image; }
    }

    public void SetCell(RectTransform tr, Image img)
	{
		_rectTransform = tr;
        _image = img;
    }
}
