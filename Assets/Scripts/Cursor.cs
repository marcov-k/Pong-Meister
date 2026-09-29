using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    RectTransform myTransform;
    Image myRenderer;
    Color defaultColor;
    Color hiddenColor;

    void Awake()
    {
        myTransform = GetComponent<RectTransform>();
        myRenderer = GetComponent<Image>();
    }

    void Start()
    {
        UnityEngine.Cursor.visible = false;
        defaultColor = new Color(1, 1, 1, 1);
        hiddenColor = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        myTransform.position = Mouse.current.position.ReadValue();
    }

    public void Hide(bool hide)
    {
        if (hide)
        {
            myRenderer.color = hiddenColor;
        }
        else
        {
            myRenderer.color = defaultColor;
        }
    }
}
