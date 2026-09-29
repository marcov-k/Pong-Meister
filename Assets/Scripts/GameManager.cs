using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool infinite = false;
    ScoreManager scoreManager;
    HighScoreHandler highScoreHandler;
    int leftScore = 0;
    int rightScore = 0;
    [SerializeField] GameObject paddlePlayer;
    [SerializeField] GameObject paddleBasic;
    [SerializeField] GameObject paddleStandard;
    [SerializeField] GameObject ball;
    Cursor cursor;
    [SerializeField] UpgradeContainer upgradeContainer;
    BoundaryCreator boundaryCreator;

    void Awake()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        cursor = FindFirstObjectByType<Cursor>();
        boundaryCreator = FindFirstObjectByType<BoundaryCreator>();
        if (infinite)
        {
            highScoreHandler = FindFirstObjectByType<HighScoreHandler>();
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        UpdateScore();
    }

    void InitializeGame()
    {
        PaddleControlAIStandard paddleStandardInst = null;
        PaddleControlAIBasic paddleBasicInst = null;
        GameObject enemyPaddle;
        if (Data.upgrades.Count > 0)
        {
            paddleStandardInst = Instantiate(paddleStandard).GetComponent<PaddleControlAIStandard>();
            enemyPaddle = paddleStandardInst.gameObject;
        }
        else
        {
            paddleBasicInst = Instantiate(paddleBasic).GetComponent<PaddleControlAIBasic>();
            enemyPaddle = paddleBasicInst.gameObject;
        }
        Ball ballInst = Instantiate(ball).GetComponent<Ball>();
        ballInst.SetWallPositions(boundaryCreator.GetPositions());
        PaddleMovement playerPaddle = Instantiate(paddlePlayer).GetComponent<PaddleMovement>();
        PaddleControlPlayer playerControl = playerPaddle.GetComponent<PaddleControlPlayer>();
        playerControl.SetEnemyPaddle(enemyPaddle);
        if (paddleStandardInst != null)
        {
            paddleStandardInst.SetBall(ballInst);
            paddleStandardInst.SetPlayer(playerPaddle);
        }
        else if (paddleBasicInst != null)
        {
            paddleBasicInst.SetBall(ballInst);
        }
        cursor.Hide(true);
    }

    void UpdateScore()
    {
        leftScore = scoreManager.GetLeftScore();
        rightScore = scoreManager.GetRightScore();
        if (!infinite)
        {
            if (leftScore >= 5 || rightScore >= 5)
            {
                if (leftScore >= 5)
                {
                    CalculatePings();
                    UpdateUpgrades();
                }
                EndGame();
            }
        }
        else
        {
            if (rightScore >= 1)
            {
                EndGame();
            }
        }
    }

    void CalculatePings()
    {
        int bonus = 0;
        foreach (string upgrade in Data.upgrades)
        {
            bonus += upgradeContainer.GetUpgrade(upgrade).pingBonus;
        }
        Data.pings += 5 + bonus;
    }

    void UpdateUpgrades()
    {
        foreach (string upgrade in Data.upgrades)
        {
            if (!PersData.unlockedUpgrades.Contains(upgrade))
            {
                PersData.unlockedUpgrades.Add(upgrade);
            }
        }
        Saver.SavePersistentData();
    }

    void EndGame()
    {
        if (!infinite)
        {
            if (Data.numUpgrades >= 10)
            {
                SceneManager.LoadScene("WinScreen");
            }
            else
            {
                Saver.SaveData();
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            highScoreHandler.HandleScore(leftScore);
            Saver.DeleteInfiniteData();
            SceneManager.LoadScene("Infinite");
        }
    }
}
