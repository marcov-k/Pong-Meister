using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    Image myImage;
    PaddleControlPlayer player;
    int index;
    string control;
    TextMeshProUGUI myText;

    void Awake()
    {
        myImage = GetComponent<Image>();
        myText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void StartTimer(float time)
    {
        StartCoroutine(TimerCoroutine(time));
    }

    IEnumerator TimerCoroutine(float time)
    {
        float currentTime = time;
        myImage.fillAmount = 0;
        while (currentTime > 0)
        {
            yield return new WaitForEndOfFrame();
            currentTime -= Time.deltaTime;
            float fill = 1 - currentTime / time;
            myImage.fillAmount = fill;
            myText.text = currentTime.ToString("0.0");
        }
        myImage.fillAmount = 1;
        myText.text = control;
        player.TimerFinished(index);
    }

    public void InitializeTimer(PaddleControlPlayer newPlayer, int newIndex)
    {
        player = newPlayer;
        index = newIndex;
        myText.text = "";
        transform.SetSiblingIndex(newIndex);
        SetControl();
    }

    void SetControl()
    {
        switch (index)
        {
            case 0:
                control = "A";
                break;
            case 1:
                control = "LMB";
                break;
            case 2:
                control = "E";
                break;
            case 3:
                control = "D";
                break;
            case 4:
                control = "Q";
                break;
        }
    }

    public void SetFill(float fill)
    {
        myImage.fillAmount = fill;
    }

    public void SetImage(Sprite image)
    {
        myImage.sprite = image;
    }

    public void ShowText()
    {
        myText.text = control;
    }
}
