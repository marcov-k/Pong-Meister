using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    SpriteRenderer myRenderer;
    ScoreManager scoreManager;
    [SerializeField] AudioClip wallSound;
    [SerializeField] AudioClip paddleSound;
    [SerializeField] AudioClip scoreSound;
    float minSpeed;
    [SerializeField] float currentMinSpeed = 5f;
    [SerializeField] float startSpeed = 5;
    [SerializeField] float defaultSpeed = 15;
    [SerializeField] float currentMaxSpeed = 30f;
    float maxSpeed;
    [SerializeField] float maxBounceAngle = 60;
    [SerializeField] float startDelay = 0.25f;
    [SerializeField] float maxOffset = 5;
    Color normalColor;
    Color hideColor;
    float speed;
    float currentSpeed;
    bool roundHit = false;
    Vector2 velocity;
    IEnumerator stuckCoroutine;
    PauseMenu pauseMenu;
    MusicManager musicManager;
    [SerializeField] float speedChangeWall = 2f;
    [SerializeField] float sizeMultiplier = 3f;
    [SerializeField] float sizeTime = 0.75f;
    float defaultSize;
    [SerializeField] float speedMult = 0.1f; // speed multiplier/second
    [SerializeField] float speedChangeAbility = 5f;
    [SerializeField] float speedTime = 2f;
    bool initializing = true;
    bool speedWhileInitializing = false;
    float speedChanged = 0;
    IEnumerator speedCoroutine;
    [SerializeField] float gravity = 1f;
    IEnumerator gravityCoroutine;
    [SerializeField] float inertia = 1f;
    float wind = 0;
    [SerializeField] float windStrength = 1f;
    float curve = 0;
    [SerializeField] float curveStrength = 1f;
    float stuckThreshold = Mathf.Epsilon;
    bool launching = false;
    List<GameObject> walls = new List<GameObject>();
    IEnumerator boundsCoroutine;

    void Awake()
    {
        initializing = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        musicManager = FindFirstObjectByType<MusicManager>();
    }

    void Start()
    {
        FirstLoad();
    }

    void Update()
    {
        if (!pauseMenu.GetPause())
        {
            Movement();
            CheckScored();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!initializing)
        {
            Turn(collision);
        }
    }

    void Turn(Collision2D collision)
    {
        if (collision.gameObject.layer != 8)
        {
            velocity = new Vector2(velocity.x, -velocity.y);
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            if (Data.upgrades.Contains("stick"))
            {
                if (currentSpeed > currentMinSpeed)
                {
                    currentSpeed -= speedChangeWall;
                }
                else
                {
                    currentSpeed = currentMinSpeed;
                }
            }
            else if (Data.upgrades.Contains("slip"))
            {
                if (currentSpeed < currentMaxSpeed)
                {
                    currentSpeed += speedChangeWall;
                }
                else
                {
                    currentSpeed = currentMaxSpeed;
                }
            }
            velocity = currentSpeed * transform.right;
            if (musicManager != null)
            {
                musicManager.PlayEffect(wallSound, 0);
            }
        }
        else
        {
            if (musicManager != null)
            {
                musicManager.PlayEffect(paddleSound, 0);
            }
            if (!roundHit)
            {
                speed = defaultSpeed;
                currentSpeed = speed + speedChanged;
                if (Data.upgrades.Contains("gravity"))
                {
                    gravityCoroutine = GravityCoroutine();
                    StartCoroutine(gravityCoroutine);
                }
                roundHit = true;
                launching = false;
            }
            float angle;
            if (Data.upgrades.Contains("aim"))
            {
                angle = AimedTurn(collision);
            }
            else
            {
                angle = BasicTurn(collision);
            }

            transform.rotation = Quaternion.Euler(0, 0, angle);
            if (!Data.upgrades.Contains("inertia"))
            {
                currentSpeed = speed + speedChanged;
            }
            else
            {
                CalculateInertia(collision);
            }
            velocity = currentSpeed * transform.right;
        }
    }

    float BasicTurn(Collision2D collision)
    {
        GameObject paddle = collision.gameObject;
        float angle = transform.eulerAngles.z;
        if (HitPaddleFront(paddle))
        {
            angle = CalculateBasicAngle(paddle);
        }
        else
        {
            if (paddle.GetComponent<PaddleMovement>().GetLeftPaddle())
            {
                if (velocity.x < 0)
                {
                    angle = CalculateBasicAngle(paddle);
                }
            }
            else
            {
                if (velocity.x > 0)
                {
                    angle = CalculateBasicAngle(paddle);
                }
            }
        }
        return angle;
    }

    float CalculateBasicAngle(GameObject paddle)
    {
        SpriteRenderer paddleRenderer = paddle.GetComponent<SpriteRenderer>();
        float paddlePos = paddle.transform.position.y;
        float paddleDiff = paddleRenderer.bounds.extents.y;
        float ballDiff = Mathf.Abs(transform.position.y - paddlePos);
        float radius = myRenderer.bounds.extents.y;
        float screenBottom = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float screenTop = -screenBottom;
        float angle = maxBounceAngle * (ballDiff / paddleDiff);
        if (angle > maxBounceAngle)
        {
            angle = maxBounceAngle;
        }

        if (Mathf.Abs(screenTop - (transform.position.y + radius)) < Mathf.Epsilon || Mathf.Abs(screenBottom - (transform.position.y - radius)) < Mathf.Epsilon)
        {
            angle *= -1;
        }

        if (paddle.transform.position.x < 0)
        {
            if (transform.position.y < paddlePos)
            {
                angle *= -1;
            }
        }
        else
        {
            if (transform.position.y > paddlePos)
            {
                angle *= -1;
            }
            angle -= 180;
        }
        return angle;
    }

    float AimedTurn(Collision2D collision)
    {
        GameObject paddle = collision.gameObject;
        float angle = transform.eulerAngles.z;
        angle = paddle.transform.eulerAngles.z;
        if (!paddle.GetComponent<PaddleMovement>().GetLeftPaddle())
        {
            angle -= 180;
        }
        if (!HitPaddleFront(paddle))
        {
            angle -= 180;
        }
        return angle;
    }

    bool HitPaddleFront(GameObject paddle)
    {
        bool front = false;
        SpriteRenderer paddleRenderer = paddle.GetComponent<SpriteRenderer>();
        PaddleMovement paddleScript = paddle.GetComponent<PaddleMovement>();
        float paddleWidth = paddleRenderer.bounds.extents.x;
        float forwardAngle = paddle.transform.eulerAngles.z;
        if (!paddleScript.GetLeftPaddle())
        {
            forwardAngle -= 180;
        }
        float backwardAngle = forwardAngle - 180;
        forwardAngle *= Mathf.Deg2Rad;
        backwardAngle *= Mathf.Deg2Rad;
        float posX = paddle.transform.position.x + paddleWidth * Mathf.Cos(forwardAngle);
        float xDiff = posX - transform.position.x;
        float posY = paddle.transform.position.y + paddleWidth * Mathf.Sin(forwardAngle);
        float yDiff = posY - transform.position.y;
        float forwardDist = Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2));
        posX = paddle.transform.position.x + paddleWidth * Mathf.Cos(backwardAngle);
        xDiff = posX - transform.position.x;
        posY = paddle.transform.position.y + paddleWidth * Mathf.Sin(backwardAngle);
        yDiff = posY - transform.position.y;
        float backwardDist = Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2));
        if (forwardDist < backwardDist)
        {
            front = true;
        }
        return front;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool left = false; // which side lost the last round
        if (transform.position.x < 0)
        {
            left = true;
        }
        Scored(left);
    }

    void CheckScored()
    {
        float radius = myRenderer.bounds.extents.y;
        float left = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        float right = -left;
        if (transform.position.x + radius < left)
        {
            Scored(true);
        }
        else if (transform.position.x - radius > right)
        {
            Scored(false);
        }
    }

    void Scored(bool left)
    {
        if (scoreManager != null)
        {
            scoreManager.Scored(left);
        }
        if (musicManager != null)
        {
            musicManager.PlayEffect(scoreSound, 0.25f);
        }
        StartCoroutine(Initialize(left));
    }

    void FirstLoad()
    {
        normalColor = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 1);
        hideColor = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 0);
        defaultSize = transform.localScale.x;
        minSpeed = currentMinSpeed;
        maxSpeed = currentMaxSpeed;
        StartCoroutine(Initialize(false));
    }

    IEnumerator Initialize(bool left)
    {
        ResetPosition();
        float startRotation;
        float startOffset;
        myRenderer.color = hideColor;
        myRigidbody.gravityScale = 0;
        initializing = true;
        launching = true;

        if (stuckCoroutine != null)
        {
            StopCoroutine(stuckCoroutine);
            stuckCoroutine = null;
        }
        if (boundsCoroutine != null)
        {
            StopCoroutine(boundsCoroutine);
            boundsCoroutine = null;
        }
        if (gravityCoroutine != null)
        {
            StopCoroutine(gravityCoroutine);
            gravityCoroutine = null;
        }
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
            speedCoroutine = null;
            currentMaxSpeed = maxSpeed;
            currentMinSpeed = minSpeed;
            currentSpeed = 0;
            myRigidbody.linearVelocity = new Vector2(0, 0);
            speedWhileInitializing = false;
            speedChanged = 0;
        }

        startOffset = Random.Range(-maxOffset, maxOffset);

        if (left)
        {
            startRotation = 0;
        }
        else
        {
            startRotation = 180;
        }

        startRotation += startOffset;

        roundHit = false;
        ResetPosition();

        yield return new WaitForSecondsRealtime(startDelay);

        myRenderer.color = normalColor;
        ResetPosition();

        yield return new WaitForSecondsRealtime(2 * startDelay);

        roundHit = false;
        ResetPosition();
        transform.rotation = Quaternion.Euler(0, 0, startRotation);
        stuckCoroutine = CheckStuck();
        StartCoroutine(stuckCoroutine);
        boundsCoroutine = CheckBounds();
        StartCoroutine(boundsCoroutine);
        speed = startSpeed;
        currentSpeed = speed;
        if (speedWhileInitializing)
        {
            currentSpeed += speedChanged;
            speedWhileInitializing = false;
        }
        velocity = currentSpeed * transform.right;
        initializing = false;
    }

    void ResetPosition()
    {
        currentSpeed = 0;
        velocity = new Vector2(0, 0);
        transform.position = new Vector2(0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator CheckBounds()
    {
        List<Vector2> positions = new List<Vector2>(); // left, top, right, bottom
        List<Vector2> bounds = new List<Vector2>(); // left, top, right, bottom
        SpriteRenderer wallRenderer;
        float radius;
        foreach (GameObject wall in walls)
        {
            wallRenderer = wall.GetComponent<SpriteRenderer>();
            positions.Add(wall.transform.position);
            bounds.Add(wallRenderer.bounds.extents);
        }
        while (true)
        {
            radius = myRenderer.bounds.extents.y;
            if (positions.Count > 0)
            {
                if (transform.position.x < positions[0].x)
                {
                    Scored(true);
                }
                else if (transform.position.x > positions[2].x)
                {
                    Scored(false);
                }
                else if (transform.position.y > positions[1].y)
                {
                    transform.position = new Vector2(transform.position.x, positions[1].y - bounds[1].y - radius - 0.1f);
                }
                else if (transform.position.y < positions[3].y)
                {
                    transform.position = new Vector2(transform.position.x, positions[3].y + bounds[3].y + radius + 0.1f);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CheckStuck()
    {
        int framesStuck = 0;
        float bottomBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float topBound = -bottomBound;
        float ballTop;
        float ballBottom;
        float lastY = 0;
        while (true)
        {
            ballTop = transform.position.y + myRenderer.bounds.extents.y;
            ballBottom = transform.position.y - myRenderer.bounds.extents.y;
            if ((Mathf.Abs(topBound - ballTop) < stuckThreshold * 2 || Mathf.Abs(ballBottom - bottomBound) < stuckThreshold * 2) && (Mathf.Abs(transform.position.y - lastY) < velocity.y / 60))
            {
                framesStuck++;
            }
            else
            {
                framesStuck = 0;
            }

            if (framesStuck >= 20)
            {
                if (transform.position.y > 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 0.05f);
                }
                else
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + 0.05f);
                }
                framesStuck = 0;
            }
            lastY = transform.position.y;
            yield return new WaitForEndOfFrame();
        }
    }

    void HandleWind()
    {
        float moveDist = wind * windStrength * Time.deltaTime;
        float ballBottom = transform.position.y - myRenderer.bounds.extents.y;
        float ballTop = transform.position.y + myRenderer.bounds.extents.y;
        float bottomBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float topBound = -bottomBound;
        if (ballTop + moveDist < topBound && ballBottom + moveDist > bottomBound)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + moveDist);
        }
    }

    void HandleCurve()
    {
        float newY = velocity.y + curve * curveStrength * Time.deltaTime;
        float currentX = velocity.x;
        float newX = Mathf.Sqrt(currentSpeed * currentSpeed - newY * newY);
        if (currentX < 0)
        {
            newX *= -1;
        }
        if (Mathf.Abs(newX) > 7.5f && ((currentX > 0 && newX > 0) || (currentX < 0 && newX < 0)))
        {
            velocity = new Vector2(newX, newY);
        }
    }

    void CalculateInertia(Collision2D collision)
    {
        PaddleMovement paddle = collision.transform.GetComponent<PaddleMovement>();
        float paddleVelocity;
        if (Data.upgrades.Contains("aim"))
        {
            float paddleDist = paddle.GetHeightDiff();
            paddleVelocity = paddle.GetSpin();
            if (transform.position.y > paddle.transform.position.y)
            {
                paddleVelocity = -paddle.GetSpin();
            }
            else if (transform.position.y < paddle.transform.position.y)
            {
                paddleVelocity = paddle.GetSpin();
            }
            if (!paddle.GetLeftPaddle())
            {
                paddleVelocity *= -1;
            }
            Vector2 ballDiff = new Vector2(transform.position.x - paddle.transform.position.x, transform.position.y - paddle.transform.position.y);
            float ballDist = Mathf.Sqrt(Mathf.Pow(ballDiff.x, 2) + Mathf.Pow(ballDiff.y, 2));
            float diffDecimal = ballDist / paddleDist;
            currentSpeed += paddleVelocity * diffDecimal * inertia;
            if (currentSpeed > currentMaxSpeed)
            {
                currentSpeed = currentMaxSpeed;
            }
            else if (currentSpeed < currentMinSpeed)
            {
                currentSpeed = currentMinSpeed;
            }
        }
        else if (Data.upgrades.Contains("move"))
        {
            paddleVelocity = paddle.GetHorizontal();
            if (!paddle.GetLeftPaddle())
            {
                paddleVelocity *= -1;
            }
            if (currentSpeed < currentMaxSpeed)
            {
                currentSpeed += paddleVelocity * inertia;
            }
            if (currentSpeed > currentMaxSpeed)
            {
                currentSpeed = currentMaxSpeed;
            }
        }
    }

    IEnumerator GravityCoroutine()
    {
        while (true)
        {
            if (!launching)
            {
                AdjustSpeed();
                float currentX = velocity.x;
                float newY = velocity.y - gravity * Time.deltaTime;
                float newX = Mathf.Sqrt(Mathf.Pow(currentSpeed, 2) - Mathf.Pow(newY, 2));
                if (currentX < 0)
                {
                    newX *= -1;
                }
                if (Mathf.Abs(newX) > 7.5f && ((currentX > 0 && newX > 0) || (currentX < 0 && newX < 0)))
                {
                    velocity = new Vector2(newX, newY);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void Movement()
    {
        if (!pauseMenu.GetPause())
        {
            if (!Data.upgrades.Contains("gravity"))
            {
                currentSpeed = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
                float angle = Mathf.Atan2(velocity.y, velocity.x);
                if (!launching)
                {
                    AdjustSpeed();
                }
                Vector2 right = new Vector2(1 * Mathf.Cos(angle), 1 * Mathf.Sin(angle));
                velocity = currentSpeed * right;
            }
            myRigidbody.angularVelocity = 0;
            myRigidbody.linearVelocity = velocity;
            if (Data.upgrades.Contains("wind") && !initializing)
            {
                HandleWind();
            }
            if (Data.upgrades.Contains("curve") && !initializing)
            {
                HandleCurve();
            }
        }
    }

    void AdjustSpeed()
    {
        if (Data.upgrades.Contains("thrust") && currentSpeed < currentMaxSpeed)
        {
            currentSpeed += speed * speedMult * Time.deltaTime;
        }
        else if (Data.upgrades.Contains("drag") && currentSpeed > currentMinSpeed)
        {
            currentSpeed -= speed * speedMult * Time.deltaTime;
        }
        if (currentSpeed > currentMaxSpeed)
        {
            currentSpeed = currentMaxSpeed;
        }
        else if (currentSpeed < currentMinSpeed && !initializing)
        {
            currentSpeed = currentMinSpeed;
        }
    }

    public void ChangeSize()
    {
        StartCoroutine(SizeCoroutine());
    }

    public void ChangeSpeed()
    {
        speedCoroutine = SpeedCoroutine();
        StartCoroutine(speedCoroutine);
    }

    public void SetWind(float newWind, bool player)
    {
        if (player && velocity.x < 0)
        {
            wind = newWind;
        }
        else if (!player && velocity.x > 0)
        {
            wind = newWind;
        }
        else
        {
            wind = 0;
        }
    }

    public void SetCurve(float newCurve, bool player)
    {
        if (player && velocity.x > 0)
        {
            curve = newCurve;
        }
        else if (!player && velocity.x < 0)
        {
            curve = newCurve;
        }
        else
        {
            curve = 0;
        }
    }

    public void ReverseBall()
    {
        Reverse();
    }

    void Reverse()
    {
        velocity = new Vector2(velocity.x, -velocity.y);
    }

    IEnumerator SizeCoroutine()
    {
        float sizeChange = 0;
        if (Data.upgrades.Contains("big"))
        {
            sizeChange = defaultSize * sizeMultiplier - defaultSize;
        }
        else if (Data.upgrades.Contains("small"))
        {
            sizeChange = defaultSize / sizeMultiplier - defaultSize;
        }
        float newSize = transform.localScale.x + sizeChange;
        transform.localScale = new Vector3(newSize, newSize, newSize);
        yield return new WaitForSeconds(sizeTime);
        newSize = transform.localScale.x - sizeChange;
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    IEnumerator SpeedCoroutine()
    {
        if (Data.upgrades.Contains("speedb"))
        {
            speedChanged = speedChangeAbility;
            currentMaxSpeed += speedChanged;
        }
        else if (Data.upgrades.Contains("slow"))
        {
            speedChanged = -speedChangeAbility;
            currentMinSpeed += speedChanged;
        }
        if (!initializing)
        {
            currentSpeed += speedChanged;
            velocity = currentSpeed * transform.right;
            myRigidbody.angularVelocity = 0;
            myRigidbody.linearVelocity = velocity;
        }
        else
        {
            speedWhileInitializing = true;
        }

        yield return new WaitForSeconds(speedTime);

        if (Data.upgrades.Contains("speedb"))
        {
            currentMaxSpeed = maxSpeed;
        }
        else if (Data.upgrades.Contains("slow"))
        {
            currentMinSpeed = minSpeed;
        }
        if (!initializing && roundHit)
        {
            currentSpeed -= speedChanged;
            velocity = currentSpeed * transform.right;
            myRigidbody.angularVelocity = 0;
            myRigidbody.linearVelocity = velocity;
            speedChanged = 0;
        }
        speedWhileInitializing = false;
    }

    public float GetGravity()
    {
        return gravity;
    }

    public float GetCurve()
    {
        return curve * curveStrength;
    }

    public float GetWind()
    {
        return wind * windStrength;
    }

    public bool GetLaunching()
    {
        return launching;
    }

    public void SetWallPositions(List<GameObject> newWalls)
    {
        walls.Clear();
        walls.AddRange(newWalls);
    }
}
