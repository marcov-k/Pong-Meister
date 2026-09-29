using System.Collections;
using UnityEngine;

public class PaddleControlAIBasic : MonoBehaviour
{
    Ball ball;
    GameObject ballObject;
    Rigidbody2D ballRigidbody;
    PaddleMovement myPaddle;
    SpriteRenderer myRenderer;
    float checkThreshold;
    float placeThreshold;
    float alignThreshold;
    float target = 0;
    bool setTarget = true;
    PauseMenu pauseMenu;

    void Awake()
    {
        myPaddle = GetComponent<PaddleMovement>();
        myRenderer = GetComponent<SpriteRenderer>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!pauseMenu.GetPause())
        {
            SetMovement();
        }
    }

    void Initialize()
    {
        float leftPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        float rightPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        float diff = rightPoint - leftPoint;
        checkThreshold = leftPoint + diff / 1.75f;
        placeThreshold = leftPoint + 0.65f * diff;
        alignThreshold = leftPoint + 0.8f * diff;
    }

    void SetMovement()
    {
        if (ballObject.transform.position.x > checkThreshold && ballRigidbody.linearVelocityX > 0)
        {
            setTarget = false;
            if (ballObject.transform.position.x < alignThreshold && (Mathf.Abs(transform.position.y) > 1 || ballObject.transform.position.x > placeThreshold))
            {
                if (ballRigidbody.linearVelocityY > 0.75f)
                {
                    myPaddle.SetVelocity(1);
                }
                else if (ballRigidbody.linearVelocityY < -0.75f)
                {
                    myPaddle.SetVelocity(-1);
                }
                else
                {
                    myPaddle.SetVelocity(0);
                }
            }
            else if (Mathf.Abs(transform.position.y) > 1 || ballObject.transform.position.x > alignThreshold)
            {
                if (ballObject.transform.position.y > transform.position.y + myRenderer.bounds.extents.y / 3)
                {
                    myPaddle.SetVelocity(1);
                }
                else if (ballObject.transform.position.y < transform.position.y - myRenderer.bounds.extents.y / 3)
                {
                    myPaddle.SetVelocity(-1);
                }
                else
                {
                    myPaddle.SetVelocity(0);
                }
            }
        }
        else
        {
            if (!setTarget)
            {
                target = Random.Range(-1.5f, 1.5f);
                setTarget = true;
            }
            if (transform.position.y > target + 0.1f)
            {
                myPaddle.SetVelocity(-1);
            }
            else if (transform.position.y < target - 0.1f)
            {
                myPaddle.SetVelocity(1);
            }
            else
            {
                myPaddle.SetVelocity(0);
            }
        }
    }

    public void SetBall(Ball newBall)
    {
        ball = newBall;
        ballObject = ball.gameObject;
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
    }
}
