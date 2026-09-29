using System.Collections;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    SpriteRenderer myRenderer;
    float velocity;
    float horizontal;
    float spin;
    [SerializeField] bool leftPaddle = false;
    [SerializeField] float defaultSpeed = 4;
    float currentSpeed;
    [SerializeField] float speedMult = 2;
    [SerializeField] float speedBoostTime = 1.5f;
    [SerializeField] float spinSpeed = 4;
    [SerializeField] float horizontalSpeed = 4;
    [SerializeField] float horizontalLimit = 0.2f; // percentage of screen from left
    [SerializeField] float rotationLimit = 60;
    [SerializeField] float xOffset = 0.1f;
    float yOffset = 0;
    PauseMenu pauseMenu;
    Vector2 startPos;
    float leftBound;
    float rightBound;
    Ball ball;
    bool frozen = false;
    [SerializeField] float frozenTime = 1;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        ball = FindFirstObjectByType<Ball>();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!pauseMenu.GetPause())
        {
            ManageMovement();
            ManageRotation();
        }
    }

    void ManageMovement()
    {
        float bottomBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y + yOffset;
        float topBound = -bottomBound - yOffset;
        float paddleTop = transform.position.y + myRenderer.bounds.extents.y;
        float paddleBottom = transform.position.y - myRenderer.bounds.extents.y;

        if (!frozen)
        {
            if (velocity > 0 && paddleTop >= topBound)
            {
                velocity = 0;
                transform.position = new Vector2(transform.position.x, topBound - myRenderer.bounds.extents.y);
            }
            else if (velocity < 0 && paddleBottom <= bottomBound)
            {
                velocity = 0;
                transform.position = new Vector2(transform.position.x, bottomBound + myRenderer.bounds.extents.y);
            }
            if (Data.upgrades.Contains("move"))
            {
                float paddleRight = transform.position.x + myRenderer.bounds.extents.x;
                float paddleLeft = transform.position.x - myRenderer.bounds.extents.x;
                if (leftPaddle)
                {
                    leftBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x + myRenderer.bounds.extents.x + xOffset;
                    rightBound = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * horizontalLimit, 0)).x;
                }
                else
                {
                    leftBound = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * (1 - horizontalLimit), 0)).x;
                    rightBound = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - myRenderer.bounds.extents.x - xOffset;
                }

                if (horizontal > 0 && paddleRight >= rightBound)
                {
                    horizontal = 0;
                    transform.position = new Vector2(rightBound, transform.position.y);
                }
                else if (horizontal < 0 && paddleLeft <= leftBound)
                {
                    horizontal = 0;
                    transform.position = new Vector2(leftBound, transform.position.y);
                }
                myRigidbody.linearVelocityX = horizontal;
            }
            myRigidbody.linearVelocityY = velocity;
        }
        else
        {
            myRigidbody.linearVelocityX = 0;
            myRigidbody.linearVelocityY = 0;
        }
    }

    void ManageRotation()
    {
        if (Data.upgrades.Contains("aim"))
        {
            float angle = transform.eulerAngles.z;
            if (angle > 180)
            {
                angle -= 360;
            }

            if (angle >= rotationLimit && spin > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, rotationLimit);
                myRigidbody.angularVelocity = 0;
            }
            else if (angle <= -rotationLimit && spin < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -rotationLimit);
                myRigidbody.angularVelocity = 0;
            }
            else
            {
                myRigidbody.angularVelocity = spin * spinSpeed;
            }
        }
    }

    IEnumerator SpeedBoostCoroutine()
    {
        currentSpeed = defaultSpeed * speedMult;
        velocity = currentSpeed;
        yield return new WaitForSeconds(speedBoostTime);
        currentSpeed = defaultSpeed;
        velocity = currentSpeed;
    }

    IEnumerator FrozenCoroutine()
    {
        frozen = true;
        yield return new WaitForSeconds(frozenTime);
        frozen = false;
    }

    void Teleport()
    {
        transform.position = new Vector2(transform.position.x, -transform.position.y);
    }

    void Initialize()
    {
        float startX;
        float startY = (Screen.height - 1) / 2;
        float startOffset = myRenderer.bounds.extents.x + xOffset;
        currentSpeed = defaultSpeed;

        if (leftPaddle)
        {
            startX = 0;
            startPos = new Vector2(startX, startY);
            startPos = Camera.main.ScreenToWorldPoint(startPos);
            startPos = new Vector2(startPos.x + startOffset, startPos.y);
        }
        else
        {
            startX = Screen.width - 1;
            startPos = new Vector2(startX, startY);
            startPos = Camera.main.ScreenToWorldPoint(startPos);
            startPos = new Vector2(startPos.x - startOffset, startPos.y);
        }

        transform.position = startPos;
        Invoke("SetRotation", 0.1f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        spin = 0;
        velocity = 0;
    }

    void SetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        spin = 0;
    }

    public void SetVelocity(float newVelocity)
    {
        velocity = newVelocity * currentSpeed;
    }

    public void SetSpin(float newSpin)
    {
        spin = newSpin;
    }

    public float GetSpin()
    {
        return spin;
    }

    public void SetHorizontalMove(float newHorizontal)
    {
        horizontal = newHorizontal * horizontalSpeed;
    }

    public float GetRotationLimit()
    {
        return rotationLimit;
    }

    public bool GetLeftPaddle()
    {
        return leftPaddle;
    }

    public Vector2 GetStartPos()
    {
        return startPos;
    }

    public float GetHorizontal()
    {
        return horizontal;
    }

    public float GetLeftBound()
    {
        return leftBound;
    }

    public float GetRightBound()
    {
        return rightBound;
    }

    public void ChangeBallSize()
    {
        ball.ChangeSize();
    }

    public void ChangeBallSpeed()
    {
        ball.ChangeSpeed();
    }

    public void ChangeWind(float wind, bool player)
    {
        ball.SetWind(wind, player);
    }

    public void ChangeCurve(float curve, bool player)
    {
        ball.SetCurve(curve, player);
    }

    public void SpeedPaddle()
    {
        StartCoroutine(SpeedBoostCoroutine());
    }

    public void ReverseBall()
    {
        ball.ReverseBall();
    }

    public void TeleportPaddle()
    {
        Teleport();
    }

    public void FreezePaddle()
    {
        StartCoroutine(FrozenCoroutine());
    }

    public float GetHeightDiff()
    {
        return myRenderer.bounds.extents.y;
    }
}
