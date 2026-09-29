using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovering = false;
    TextMeshProUGUI text;
    [SerializeField] Color normalColor;
    [SerializeField] Color hoverColor;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
    }

    void Awake()
    {
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        SetTextColor();
    }

    void OnDisable()
    {
        hovering = false;
    }

    void Update()
    {
        SetTextColor();
    }

    void SetTextColor()
    {
        if (hovering)
        {
            text.color = hoverColor;
        }
        else
        {
            text.color = normalColor;
        }
    }
}
