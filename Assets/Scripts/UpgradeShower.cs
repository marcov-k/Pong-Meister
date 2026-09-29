using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UpgradeShower : MonoBehaviour
{
    float showOpacity = 1;
    float hideOpacity = 0;
    Image myImage;
    TextMeshProUGUI myText;
    RectTransform myTransform;
    bool show = false;

    void Awake()
    {
        myImage = GetComponent<Image>();
        myText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        myTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        SetOpacity(hideOpacity);
    }

    void SetOpacity(float opacity)
    {
        myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, opacity);
        myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, opacity);
    }

    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3[] corners = new Vector3[4];
        myTransform.GetWorldCorners(corners);
        float xDiff = corners[3].x - transform.position.x;
        float yDiff = corners[2].y - transform.position.y;
        Vector3 newPos = new Vector3(mousePos.x + xDiff + 1, mousePos.y + yDiff + 3, 0);
        if (show)
        {
            float xMin = newPos.x - xDiff;
            float xMax = newPos.x + xDiff;
            float yMin = newPos.y - yDiff;
            float yMax = newPos.y + yDiff;
            if (xMin < 0)
            {
                newPos = new Vector3(xDiff, newPos.y, 0);
            }
            else if (xMax > Screen.width)
            {
                newPos = new Vector3(Screen.width - xDiff, newPos.y, 0);
            }
            if (yMin < 0)
            {
                newPos = new Vector3(newPos.x, yDiff, 0);
            }
            else if (yMax > Screen.height)
            {
                newPos = new Vector3(newPos.x, Screen.height - yDiff, 0);
            }
        }
        transform.position = newPos;
    }

    public void Show(Upgrade upgrade)
    {
        SetOpacity(showOpacity);
        myText.text = upgrade.description;
        show = true;
    }

    public void Hide()
    {
        myText.text = null;
        SetOpacity(hideOpacity);
        show = false;
    }
}
