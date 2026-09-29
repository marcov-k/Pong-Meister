using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashbangScreen : MonoBehaviour
{
    Color defaultColor = new Color(1, 1, 1, 0);
    Color flashColor = new Color(1, 1, 1, 1);
    Image myImage;

    void Awake()
    {
        myImage = GetComponent<Image>();
    }

    void Start()
    {
        myImage.color = defaultColor;
    }

    IEnumerator FlashCoroutine(float time)
    {
        myImage.color = flashColor;
        yield return new WaitForSeconds(time);
        myImage.color = defaultColor;
    }

    public void Flash(float time)
    {
        StartCoroutine(FlashCoroutine(time));
    }
}
