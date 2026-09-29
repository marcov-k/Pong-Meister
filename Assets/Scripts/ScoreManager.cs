using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    int leftScore = 0;
    int rightScore = 0;

    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        text.text = leftScore + " : " + rightScore;
    }

    public void Scored(bool right)
    {
        if (right)
        {
            rightScore += 1;
        }
        else
        {
            leftScore += 1;
        }
    }

    public int GetLeftScore()
    {
        return leftScore;
    }

    public int GetRightScore()
    {
        return rightScore;
    }
}
