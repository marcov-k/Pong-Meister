using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class PaddleControlAIStandard : MonoBehaviour
{
    Ball ball;
    GameObject ballObject;
    Rigidbody2D ballRigidbody;
    SpriteRenderer ballRenderer;
    PaddleMovement myPaddle;
    SpriteRenderer myRenderer;
    PauseMenu pauseMenu;
    PaddleMovement playerPaddle;
    PaddleControlPlayer playerControl;
    float targetPos;
    float targetOffset;
    bool targetSet = false;
    bool moveSet = false;
    float waitMove;
    bool waitMoveSet = false;
    float lastIdleSet;
    float idleRange = 2.5f;
    float minIdleSpeed = 0.4f;
    float maxIdleSpeed = 0.6f;
    bool targetAngleSet = false;
    float idleAngleLimit = 10;
    bool idleAngleSet = false;
    float maxSpinSpeed = 3.25f;
    float minSpinSpeed = 0.8f;
    float finalVelocity;
    Vector2 finalPos;
    float minHorizSpeed = 3f;
    float maxHorizSpeed = 4.5f;
    IEnumerator verticalCoroutine;
    IEnumerator horizontalCoroutine;
    IEnumerator rotationCoroutine;
    bool horizIdleSet = false;
    IEnumerator predictionCoroutine;
    IEnumerator abilityCoroutine;
    IEnumerator moveResetCoroutine;
    float targetStopPos;
    bool usedDefence = false;
    bool usedOffence = false;
    float[] cooldownTimes = new float[5] { 3, 3, 3, 3, 3 };
    bool[] abilityCooldowns = new bool[5] { false, false, false, false, false };
    string[] abilitiesAvailable = new string[5];
    [SerializeField] float predictionAccuracy = 40;
    [SerializeField] float moveResetTime = 0.5f;
    [SerializeField] float abilityDiffHigh = 1.5f;
    [SerializeField] float abilityDiffLow = 0.5f;
    [SerializeField] float moreAbilityChance = 0.5f;
    [SerializeField] UpgradeContainer upgradeContainer;
    bool flashed = false;
    [SerializeField] float flashedTime = 0.5f;
    IEnumerator curveCoroutine;
    IEnumerator windCoroutine;
    [SerializeField] float moveReTime = 1.25f;
    [SerializeField] float abilityReTime = 0.9f;
    float ballHitTime;
    
    void Awake()
    {
        myPaddle = GetComponent<PaddleMovement>();
        myRenderer = GetComponent<SpriteRenderer>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    void Start()
    {
        InitializeCooldowns();
        InitializeAbilities();
        InitializeNumbers();
    }

    void Update()
    {
        SetMovement();
        SetRotation();
    }

    void InitializeNumbers()
    {
        float edge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        targetStopPos = transform.position.x - myRenderer.bounds.extents.x;
        if (Data.upgrades.Count() > 2)
        {
            predictionAccuracy += 20;
        }
        if (Data.upgrades.Contains("aim"))
        {
            targetOffset = 0.25f * myRenderer.bounds.extents.y;
        }
        else
        {
            targetOffset = 0.4f * myRenderer.bounds.extents.y;
        }
        predictionCoroutine = PredictionCoroutine();
        StartCoroutine(predictionCoroutine);
        abilityCoroutine = AbilityCoroutine();
        StartCoroutine(abilityCoroutine);
        moveResetCoroutine = MoveResetCoroutine();
        StartCoroutine(moveResetCoroutine);
        if (Data.upgrades.Contains("curve"))
        {
            curveCoroutine = CurveCoroutine();
            StartCoroutine(curveCoroutine);
        }
        else if (Data.upgrades.Contains("wind"))
        {
            windCoroutine = WindCoroutine();
            StartCoroutine(windCoroutine);
        }
    }

    void InitializeCooldowns()
    {
        if (Data.upgrades.Contains("small"))
        {
            cooldownTimes[0] = upgradeContainer.GetUpgrade("small").cooldown;
        }
        else if (Data.upgrades.Contains("big"))
        {
            cooldownTimes[0] = upgradeContainer.GetUpgrade("big").cooldown;
        }
        if (Data.upgrades.Contains("speedb"))
        {
            cooldownTimes[1] = upgradeContainer.GetUpgrade("speedb").cooldown;
        }
        else if (Data.upgrades.Contains("slow"))
        {
            cooldownTimes[1] = upgradeContainer.GetUpgrade("slow").cooldown;
        }
        if (Data.upgrades.Contains("flash"))
        {
            cooldownTimes[2] = upgradeContainer.GetUpgrade("flash").cooldown;
        }
        else if (Data.upgrades.Contains("speedp"))
        {
            cooldownTimes[2] = upgradeContainer.GetUpgrade("speedp").cooldown;
        }
        if (Data.upgrades.Contains("reverse"))
        {
            cooldownTimes[3] = upgradeContainer.GetUpgrade("reverse").cooldown;
        }
        else if (Data.upgrades.Contains("teleports"))
        {
            cooldownTimes[3] = upgradeContainer.GetUpgrade("teleports").cooldown;
        }
        if (Data.upgrades.Contains("freeze"))
        {
            cooldownTimes[4] = upgradeContainer.GetUpgrade("freeze").cooldown;
        }
        else if (Data.upgrades.Contains("teleporte"))
        {
            cooldownTimes[4] = upgradeContainer.GetUpgrade("teleporte").cooldown;
        }
    }

    void InitializeAbilities()
    {
        List<string> abilities = new List<string>();
        foreach (string ability in Data.upgrades)
        {
            switch (ability)
            {
                case "small":
                    abilities.Add(ability);
                    break;
                case "big":
                    abilities.Add(ability);
                    break;
                case "speedb":
                    abilities.Add(ability);
                    break;
                case "slow":
                    abilities.Add(ability);
                    break;
                case "flash":
                    abilities.Add(ability);
                    break;
                case "speedp":
                    abilities.Add(ability);
                    break;
                case "reverse":
                    abilities.Add(ability);
                    break;
                case "teleports":
                    abilities.Add(ability);
                    break;
                case "freeze":
                    abilities.Add(ability);
                    break;
                case "teleporte":
                    abilities.Add(ability);
                    break;
            }
        }
        abilitiesAvailable = abilities.ToArray();
    }

    IEnumerator CurveCoroutine()
    {
        while (true)
        {
            if (!pauseMenu.GetPause() && ballRigidbody.linearVelocityX < 0)
            {
                if (targetPos > playerPaddle.transform.position.y)
                {
                    ball.SetCurve(1, false);
                }
                else if (targetPos < playerPaddle.transform.position.y)
                {
                    ball.SetCurve(-1, false);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator WindCoroutine()
    {
        while (true)
        {
            if (!pauseMenu.GetPause() && ballRigidbody.linearVelocityX > 0)
            {
                if (targetPos > transform.position.y)
                {
                    ball.SetWind(-1, false);
                }
                else if (targetPos < transform.position.y)
                {
                    ball.SetWind(1, false);
                }
                else
                {
                    ball.SetWind(0, false);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PredictionCoroutine()
    {
        float radius;
        float paddleWidth = myRenderer.bounds.extents.y;
        float bottomBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float topBound = -bottomBound;
        Vector2 pos;
        Vector2 move;
        float gravity = ball.GetGravity() / predictionAccuracy;
        float stopPos = 0;
        float avgMove;
        float edge;
        float dist;
        while (true)
        {
            if (!pauseMenu.GetPause())
            {
                radius = ballRenderer.bounds.extents.y;
                pos = ballObject.transform.position;
                move = ballRigidbody.linearVelocity / predictionAccuracy;
                float timesMoved = 0;
                float xMoved = 0;
                if (move.x > 0)
                {
                    if (Data.upgrades.Contains("move"))
                    {
                        stopPos = targetStopPos;
                    }
                    else
                    {
                        stopPos = transform.position.x - paddleWidth;
                    }
                    while (pos.x + radius < stopPos)
                    {
                        timesMoved++;
                        xMoved += move.x;
                        pos += move;
                        pos = AddWind(pos);
                        move = CheckBounce(move, pos.y, radius, topBound, bottomBound);
                        move = AddGravity(move, gravity);
                    }
                }
                else if (move.x < 0)
                {
                    stopPos = playerPaddle.transform.position.x + paddleWidth;
                    while (pos.x - radius > stopPos)
                    {
                        timesMoved++;
                        xMoved += move.x;
                        pos += move;
                        move = CheckBounce(move, pos.y, radius, topBound, bottomBound);
                        move = AddGravity(move, gravity);
                        move = AddCurve(move);
                    }
                }
                finalPos = pos;
                finalVelocity = move.y * predictionAccuracy;
                targetPos = pos.y;
                avgMove = (Mathf.Abs(xMoved) / timesMoved) * predictionAccuracy;
                edge = stopPos;
                dist = Mathf.Abs(edge - ball.transform.position.x);
                ballHitTime = dist / avgMove;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    Vector2 CheckBounce(Vector2 move, float yPos, float radius, float topBound, float bottomBound)
    {
        Vector2 newMove = move;
        if (yPos + radius >= topBound || yPos - radius <= bottomBound)
        {
            newMove = new Vector2(move.x, -move.y);
        }
        return newMove;
    }

    Vector2 AddGravity(Vector2 move, float gravity)
    {
        Vector2 newMove = move;
        if (Data.upgrades.Contains("gravity"))
        {
            float currentX = move.x;
            float newY = move.y - gravity;
            float currentSpeed = Mathf.Sqrt(Mathf.Pow(move.x, 2) + Mathf.Pow(move.y, 2));
            float newX = Mathf.Sqrt(Mathf.Pow(currentSpeed, 2) - Mathf.Pow(newY, 2));
            if (currentX < 0)
            {
                newX *= -1;
            }
            if (Mathf.Abs(newX) > 7.5f / predictionAccuracy && ((currentX > 0 && newX > 0) || (currentX < 0 && newX < 0)))
            {
                newMove = new Vector2(newX, newY);
            }
        }
        return newMove;
    }

    Vector2 AddCurve(Vector2 move)
    {
        Vector2 newMove = move;
        if (Data.upgrades.Contains("curve"))
        {
            float curve = ball.GetCurve() / predictionAccuracy;
            float currentX = move.x;
            float newY = move.y + curve;
            float currentSpeed = Mathf.Sqrt(Mathf.Pow(move.x, 2) + Mathf.Pow(move.y, 2));
            float newX = Mathf.Sqrt(Mathf.Pow(currentSpeed, 2) - Mathf.Pow(newY, 2));
            if (currentX < 0)
            {
                newX *= -1;
            }
            if (Mathf.Abs(newX) > 7.5f/predictionAccuracy && ((currentX > 0 && newX > 0) || (currentX < 0 && newX < 0)))
            {
                newMove = new Vector2(newX, newY);
            }
        }
        return newMove;
    }

    Vector2 AddWind(Vector2 pos)
    {
        Vector2 newPos = pos;
        if (Data.upgrades.Contains("wind"))
        {
            float wind = ball.GetWind() / predictionAccuracy;
            float ballTop = ballObject.transform.position.y + ballRenderer.bounds.extents.y;
            float ballBottom = ballObject.transform.position.y - ballRenderer.bounds.extents.y;
            float bottomBound = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
            float topBound = -bottomBound;
            if (ballTop + wind < topBound && ballBottom + wind > bottomBound)
            {
                newPos = new Vector2(newPos.x, newPos.y + wind);
            }
        }
        return newPos;
    }

    IEnumerator AbilityCoroutine()
    {
        List<string> abilityOptions = new List<string>();
        List<string> choices = new List<string>();
        int index;
        while (true)
        {
            if (!pauseMenu.GetPause() && !flashed && !ball.GetLaunching())
            {
                if (ballRigidbody.linearVelocityX > 0 && ballHitTime < abilityReTime)
                {
                    usedOffence = false;
                    if (!usedDefence)
                    {
                        abilityOptions.Clear();
                        choices.Clear();
                        if (Mathf.Abs(targetPos - transform.position.y) > abilityDiffHigh)
                        {
                            abilityOptions.AddRange(new List<string>() { "teleports", "reverse", "speedp", "slow", "big"});
                            choices.AddRange(DetermineAbility(abilityOptions));
                            if (choices.Count() > 0)
                            {
                                usedDefence = true;
                                foreach (string ability in choices)
                                {
                                    index = Array.IndexOf(abilitiesAvailable, ability);
                                    UseAbility(ability);
                                    abilityCooldowns[index] = true;
                                    StartCoroutine(CooldownCoroutine(index));
                                }
                            }
                        }
                    }
                }
                else if (ballRigidbody.linearVelocityX < 0 && ballHitTime < abilityReTime)
                {
                    usedDefence = false;
                    if (!usedOffence)
                    {
                        abilityOptions.Clear();
                        choices.Clear();
                        if (Mathf.Abs(targetPos - playerPaddle.transform.position.y) > abilityDiffHigh)
                        {
                            abilityOptions.AddRange(new List<string>() { "freeze", "flash", "speedb", "small" });
                            choices.AddRange(DetermineAbility(abilityOptions));
                            if (choices.Count() > 0)
                            {
                                usedOffence = true;
                                foreach (string ability in choices)
                                {
                                    index = Array.IndexOf(abilitiesAvailable, ability);
                                    UseAbility(ability);
                                    abilityCooldowns[index] = true;
                                    StartCoroutine(CooldownCoroutine(index));
                                }
                            }
                        }
                        else if (Mathf.Abs(targetPos - playerPaddle.transform.position.y) < abilityDiffLow)
                        {
                            abilityOptions.AddRange(new List<string>() { "teleporte", "reverse" });
                            choices.AddRange(DetermineAbility(abilityOptions));
                            if (choices.Count() > 0)
                            {
                                usedOffence = true;
                                foreach (string ability in choices)
                                {
                                    index = Array.IndexOf(abilitiesAvailable, ability);
                                    UseAbility(ability);
                                    abilityCooldowns[index] = true;
                                    StartCoroutine(CooldownCoroutine(index));
                                }
                            }
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void UseAbility(string ability)
    {
        switch (ability)
        {
            case "small":
                ball.ChangeSize();
                break;
            case "big":
                ball.ChangeSize();
                break;
            case "speedb":
                ball.ChangeSpeed();
                break;
            case "slow":
                ball.ChangeSpeed();
                break;
            case "flash":
                playerControl.Flashbang(flashedTime);
                break;
            case "speedp":
                myPaddle.SpeedPaddle();
                break;
            case "reverse":
                ball.ReverseBall();
                break;
            case "teleports":
                myPaddle.TeleportPaddle();
                break;
            case "freeze":
                playerPaddle.FreezePaddle();
                break;
            case "teleporte":
                playerPaddle.TeleportPaddle();
                break;
        }
    }

    List<string> DetermineAbility(List<string> options)
    {
        List<string> choices = new List<string>();
        int index;
        bool repeat = true;
        float num;
        foreach (string ability in options)
        {
            index = upgradeContainer.GetUpgrade(ability).abilityIndex;
            if (abilitiesAvailable.Contains(ability) && !abilityCooldowns[index])
            {
                choices.Add(ability);
                if (ability == "speedp" || ability == "flash")
                {
                    num = UnityEngine.Random.Range(0, 1);
                    if (num < moreAbilityChance)
                    {
                        repeat = true;
                    }
                    else
                    {
                        repeat = false;
                    }
                }
                else
                {
                    repeat = false;
                }
                if (!repeat)
                {
                    break;
                }
            }
            index--;
        }
        return choices;
    }

    IEnumerator CooldownCoroutine(int index)
    {
        float time = cooldownTimes[index];
        yield return new WaitForSeconds(time);
        abilityCooldowns[index] = false;
    }

    void SetMovement()
    {
        if (!pauseMenu.GetPause())
        {
            float ballVelocity = ballRigidbody.linearVelocityX;
            float ballPos = ballObject.transform.position.x;
            float paddleWidth = myRenderer.bounds.extents.x;
            if (Data.upgrades.Contains("move") && !targetSet && ballVelocity > 0 && ballHitTime < moveReTime)
            {
                targetSet = true;
                float leftLimit = myPaddle.GetLeftBound();
                float rightLimit = myPaddle.GetRightBound() - 2 * paddleWidth;
                if (Data.upgrades.Contains("inertia"))
                {
                    float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballVelocity, 2) + Mathf.Pow(ballRigidbody.linearVelocityY, 2));
                    if (ballSpeed < 9)
                    {
                        rightLimit = transform.position.x;
                    }
                    else if (ballSpeed > 12)
                    {
                        leftLimit = transform.position.x;
                    }
                }
                targetStopPos = UnityEngine.Random.Range(leftLimit, rightLimit);
            }
            if (ballVelocity > 0 && !moveSet && ballHitTime < moveReTime && !flashed)
            {
                moveSet = true;
                if (verticalCoroutine != null)
                {
                    StopCoroutine(verticalCoroutine);
                    verticalCoroutine = null;
                }
                verticalCoroutine = VerticalCoroutine(targetPos);
                StartCoroutine(verticalCoroutine);
                if (Data.upgrades.Contains("move"))
                {
                    horizIdleSet = false;
                    HorizontalMovement(finalPos.x + paddleWidth, false);
                }
            }
            if (ballVelocity <= 0 || ballHitTime > moveReTime)
            {
                targetSet = false;
                moveSet = false;
                if (verticalCoroutine != null)
                {
                    StopCoroutine(verticalCoroutine);
                    verticalCoroutine = null;
                }
                IdleMovement();
                if (Data.upgrades.Contains("move") && !horizIdleSet)
                {
                    horizIdleSet = true;
                    float target = UnityEngine.Random.Range(myPaddle.GetLeftBound(), myPaddle.GetRightBound());
                    HorizontalMovement(target, true);
                }
            }
        }
    }

    IEnumerator MoveResetCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveResetTime);
            moveSet = false;
        }
    }

    IEnumerator VerticalCoroutine(float target)
    {
        if (Mathf.Abs(targetPos - transform.position.y) > targetOffset)
        {
            if (targetPos - targetOffset > transform.position.y)
            {
                while (targetPos - targetOffset > transform.position.y)
                {
                    myPaddle.SetVelocity(1);
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetVelocity(0);
                yield break;
            }
            else if (targetPos + targetOffset < transform.position.y)
            {
                while (targetPos + targetOffset < transform.position.y)
                {
                    myPaddle.SetVelocity(-1);
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetVelocity(0);
                yield break;
            }
        }
        else
        {
            myPaddle.SetVelocity(0);
        }
    }

    void HorizontalMovement(float target, bool idle)
    {
        if (idle)
        {
            float speed = UnityEngine.Random.Range(minHorizSpeed, maxHorizSpeed);
            if (horizontalCoroutine != null)
            {
                StopCoroutine(horizontalCoroutine);
                horizontalCoroutine = null;
            }
            horizontalCoroutine = HorizontalCoroutine(speed, target, false);
            StartCoroutine(horizontalCoroutine);
        }
        else
        {
            float speed = UnityEngine.Random.Range(minHorizSpeed, maxHorizSpeed);
            if (horizontalCoroutine != null)
            {
                StopCoroutine(horizontalCoroutine);
                horizontalCoroutine = null;
            }
            bool align = false;
            if (Data.upgrades.Contains("inertia"))
            {
                align = true;
            }
            horizontalCoroutine = HorizontalCoroutine(speed, target, align);
            StartCoroutine(horizontalCoroutine);
        }
    }

    IEnumerator HorizontalCoroutine(float speed, float target, bool align)
    {
        if (align)
        {
            float ballSpeed = ballRigidbody.linearVelocityX;
            float ballDist = target - (ballObject.transform.position.x + ballRenderer.bounds.extents.x);
            float ballTime = ballDist / ballSpeed;
            float paddleDist = Mathf.Abs(target - transform.position.x);
            float paddleTime = paddleDist / speed;
            float delay = ballTime - paddleTime;
            float currentPos = transform.position.x;
            if (target < currentPos)
            {
                delay += 0.1f;
            }
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            if (target < currentPos)
            {
                myPaddle.SetHorizontalMove(-speed);
                while (target < transform.position.x)
                {
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetHorizontalMove(0);
                transform.position = new Vector2(target, transform.position.y);
            }
            else if (target > currentPos)
            {
                myPaddle.SetHorizontalMove(speed);
                while (target > transform.position.x)
                {
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetHorizontalMove(0);
                transform.position = new Vector2(target, transform.position.y);
            }
            else
            {
                myPaddle.SetHorizontalMove(0);
            }
        }
        else
        {
            float currentPos = transform.position.x;
            if (target < currentPos)
            {
                myPaddle.SetHorizontalMove(-speed);
                while (target < transform.position.x)
                {
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetHorizontalMove(0);
                transform.position = new Vector2(target, transform.position.y);
            }
            else if (target > currentPos)
            {
                myPaddle.SetHorizontalMove(speed);
                while (target > transform.position.x)
                {
                    yield return new WaitForEndOfFrame();
                }
                myPaddle.SetHorizontalMove(0);
                transform.position = new Vector2(target, transform.position.y);
            }
            else
            {
                myPaddle.SetHorizontalMove(0);
            }
        }
    }

    void IdleMovement()
    {
        float topBound = 1.5f * myRenderer.bounds.extents.y;
        float bottomBound = -topBound;
        targetSet = false;

        if (!waitMoveSet)
        {
            int num = UnityEngine.Random.Range(0, 2);
            if (num == 0)
            {
                waitMove = GenerateIdleSpeed(true);
            }
            else
            {
                waitMove = GenerateIdleSpeed(false);
            }
            lastIdleSet = transform.position.y;
            waitMoveSet = true;
        }

        if (transform.position.y >= topBound)
        {
            waitMove = GenerateIdleSpeed(false);
            lastIdleSet = transform.position.y;
        }
        else if (transform.position.y <= bottomBound)
        {
            waitMove = GenerateIdleSpeed(true);
            lastIdleSet = transform.position.y;
        }

        if (Mathf.Abs(transform.position.y - lastIdleSet) > idleRange)
        {
            int num = UnityEngine.Random.Range(0, 11);
            if (num == 0)
            {
                if (waitMove > 0)
                {
                    waitMove = GenerateIdleSpeed(false);
                }
                else
                {
                    waitMove = GenerateIdleSpeed(true);
                }
                lastIdleSet = transform.position.y;
            }
        }

        myPaddle.SetVelocity(waitMove);
    }

    void SetRotation()
    {
        if (Data.upgrades.Contains("aim"))
        {
            float angleLimit = 0;
            if (ballHitTime < moveReTime && ballRigidbody.linearVelocityX > 0)
            {
                if (!targetAngleSet)
                {
                    if (Mathf.Abs(finalVelocity) > 2.5f)
                    {
                        angleLimit = myPaddle.GetRotationLimit();
                    }
                    else
                    {
                        angleLimit = idleAngleLimit;
                    }
                    targetAngleSet = true;
                    idleAngleSet = false;
                    float targetAngle = UnityEngine.Random.Range(0, angleLimit);
                    bool align = false;
                    if (Data.upgrades.Contains("inertia"))
                    {
                        float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballRigidbody.linearVelocityX, 2) + Mathf.Pow(ballRigidbody.linearVelocityY, 2));
                        if ((targetPos > transform.position.y && ballSpeed < 9) || (targetPos < transform.position.y && ballSpeed > 12))
                        {
                            targetAngle *= -1;
                        }
                        align = true;
                    }
                    else if (finalVelocity < 0)
                    {
                        targetAngle *= -1;
                    }
                    if (rotationCoroutine != null)
                    {
                        StopCoroutine(rotationCoroutine);
                        rotationCoroutine = null;
                    }
                    rotationCoroutine = RotationCoroutine(targetAngle, align);
                    StartCoroutine(rotationCoroutine);
                }
            }
            else
            {
                if (!idleAngleSet)
                {
                    idleAngleSet = true;
                    targetAngleSet = false;
                    angleLimit = idleAngleLimit;
                    float targetAngle = UnityEngine.Random.Range(-angleLimit, angleLimit);
                    if (rotationCoroutine != null)
                    {
                        StopCoroutine(rotationCoroutine);
                        rotationCoroutine = null;
                    }
                    rotationCoroutine = RotationCoroutine(targetAngle, false);
                    StartCoroutine(rotationCoroutine);
                }
            }
        }
    }

    IEnumerator RotationCoroutine(float targetAngle, bool align)
    {
        float spin = 0;
        float angle = transform.eulerAngles.z;
        float currentAngle = transform.eulerAngles.z;
        if (angle > 180)
        {
            angle -= 360;
        }
        if (currentAngle > 180)
        {
            currentAngle -= 360;
        }
        if (align)
        {
            float xTarget = transform.position.x - myRenderer.bounds.extents.x;
            float ballSpeed = ballRigidbody.linearVelocityX;
            float ballDist = xTarget - ballObject.transform.position.x;
            float delay = (ballDist / ballSpeed) - 0.15f;
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            if (targetAngle > angle)
            {
                spin = GenerateSpin(true);
                while (targetAngle > currentAngle)
                {
                    myPaddle.SetSpin(spin);
                    yield return new WaitForEndOfFrame();
                    currentAngle = transform.eulerAngles.z;
                    if (currentAngle > 180)
                    {
                        currentAngle -= 360;
                    }
                }
                myPaddle.SetSpin(0);
                yield break;
            }
            else if (targetAngle < angle)
            {
                spin = GenerateSpin(false);
                while (targetAngle < currentAngle)
                {
                    myPaddle.SetSpin(spin);
                    yield return new WaitForEndOfFrame();
                    currentAngle = transform.eulerAngles.z;
                    if (currentAngle > 180)
                    {
                        currentAngle -= 360;
                    }
                }
                myPaddle.SetSpin(0);
                yield break;
            }
            else
            {
                myPaddle.SetSpin(0);
            }
        }
        else
        {
            if (targetAngle > angle)
            {
                spin = GenerateSpin(true);
                while (targetAngle > currentAngle)
                {
                    myPaddle.SetSpin(spin);
                    yield return new WaitForEndOfFrame();
                    currentAngle = transform.eulerAngles.z;
                    if (currentAngle > 180)
                    {
                        currentAngle -= 360;
                    }
                }
                myPaddle.SetSpin(0);
                yield break;
            }
            else if (targetAngle < angle)
            {
                spin = GenerateSpin(false);
                while (targetAngle < currentAngle)
                {
                    myPaddle.SetSpin(spin);
                    yield return new WaitForEndOfFrame();
                    currentAngle = transform.eulerAngles.z;
                    if (currentAngle > 180)
                    {
                        currentAngle -= 360;
                    }
                }
                myPaddle.SetSpin(0);
                yield break;
            }
            else
            {
                myPaddle.SetSpin(0);
            }
        }
    }

    float GenerateSpin(bool positive)
    {
        float spin = 0;

        if (targetAngleSet)
        {
            spin = maxSpinSpeed;
        }
        else if (idleAngleSet)
        {
            spin = UnityEngine.Random.Range(minSpinSpeed, maxSpinSpeed);
        }

        if (!positive)
        {
            spin *= -1;
        }
        return spin;
    }

    float GenerateIdleSpeed(bool positive)
    {
        float speed = UnityEngine.Random.Range(minIdleSpeed, maxIdleSpeed);
        if (!positive)
        {
            speed *= -1;
        }
        return speed;
    }

    public void SetBall(Ball newBall)
    {
        ball = newBall;
        ballObject = ball.gameObject;
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        ballRenderer = ball.GetComponent<SpriteRenderer>();
    }

    public void SetPlayer(PaddleMovement newPlayer)
    {
        playerPaddle = newPlayer;
        playerControl = playerPaddle.GetComponent<PaddleControlPlayer>();
    }

    IEnumerator FlashbangCoroutine()
    {
        flashed = true;
        if (!ball.GetLaunching())
        {
            List<string> abilityOptions = new List<string>() { "small", "big", "speedb", "slow", "flash", "speedp", "reverse", "teleports", "freeze", "teleporte" };
            List<string> choices = DetermineAbility(abilityOptions);
            int index;
            if (choices.Count() > 0)
            {
                foreach (string ability in choices)
                {
                    index = Array.IndexOf(abilitiesAvailable, ability);
                    UseAbility(ability);
                    abilityCooldowns[index] = true;
                    StartCoroutine(CooldownCoroutine(index));
                }
            }
        }
        yield return new WaitForSeconds(flashedTime);
        flashed = false;
    }

    public void Flashbang()
    {
        StartCoroutine(FlashbangCoroutine());
    }
}
