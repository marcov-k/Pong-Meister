using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PaddleControlPlayer : MonoBehaviour
{
    PaddleMovement myPaddle;
    PauseMenu pauseMenu;
    [SerializeField] float[] cooldownTimes = new float[5] {3, 3, 3, 3, 3};
    bool[] abilityCooldowns = new bool[5] {false, false, false, false, false};
    Timer[] timers = new Timer[5];
    [SerializeField] UpgradeContainer upgradeContainer;
    PaddleMovement enemyPaddle;
    PaddleControlAIStandard enemyPaddleAI;
    FlashbangScreen flashScreen;
    
    void Awake()
    {
        myPaddle = GetComponent<PaddleMovement>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        flashScreen = FindFirstObjectByType<FlashbangScreen>();
        timers = FindObjectsByType<Timer>(FindObjectsSortMode.InstanceID);
        Array.Reverse(timers);
        InitializeCooldowns();
        InitializeTimers();
    }

    void InitializeCooldowns()
    {
        if (Data.upgrades.Contains("small"))
        {
            cooldownTimes[0] = upgradeContainer.GetUpgrade("small").cooldown;
            timers[0].SetImage(upgradeContainer.GetUpgrade("small").icon);
        }
        else if (Data.upgrades.Contains("big"))
        {
            cooldownTimes[0] = upgradeContainer.GetUpgrade("big").cooldown;
            timers[0].SetImage(upgradeContainer.GetUpgrade("big").icon);
        }
        if (Data.upgrades.Contains("speedb"))
        {
            cooldownTimes[1] = upgradeContainer.GetUpgrade("speedb").cooldown;
            timers[1].SetImage(upgradeContainer.GetUpgrade("speedb").icon);
        }
        else if (Data.upgrades.Contains("slow"))
        {
            cooldownTimes[1] = upgradeContainer.GetUpgrade("slow").cooldown;
            timers[1].SetImage(upgradeContainer.GetUpgrade("slow").icon);
        }
        if (Data.upgrades.Contains("flash"))
        {
            cooldownTimes[2] = upgradeContainer.GetUpgrade("flash").cooldown;
            timers[2].SetImage(upgradeContainer.GetUpgrade("flash").icon);
        }
        else if (Data.upgrades.Contains("speedp"))
        {
            cooldownTimes[2] = upgradeContainer.GetUpgrade("speedp").cooldown;
            timers[2].SetImage(upgradeContainer.GetUpgrade("speedp").icon);
        }
        if (Data.upgrades.Contains("reverse"))
        {
            cooldownTimes[3] = upgradeContainer.GetUpgrade("reverse").cooldown;
            timers[3].SetImage(upgradeContainer.GetUpgrade("reverse").icon);
        }
        else if (Data.upgrades.Contains("teleports"))
        {
            cooldownTimes[3] = upgradeContainer.GetUpgrade("teleports").cooldown;
            timers[3].SetImage(upgradeContainer.GetUpgrade("teleports").icon);
        }
        if (Data.upgrades.Contains("freeze"))
        {
            cooldownTimes[4] = upgradeContainer.GetUpgrade("freeze").cooldown;
            timers[4].SetImage(upgradeContainer.GetUpgrade("freeze").icon);
        }
        else if (Data.upgrades.Contains("teleporte"))
        {
            cooldownTimes[4] = upgradeContainer.GetUpgrade("teleporte").cooldown;
            timers[4].SetImage(upgradeContainer.GetUpgrade("teleporte").icon);
        }
    }

    void InitializeTimers()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i].InitializeTimer(this, i);
        }
        int numActive = 0;
        if (Data.upgrades.Count > 9)
        {
            numActive = 5;
        }
        else if (Data.upgrades.Count > 8)
        {
            numActive = 4;
        }
        else if (Data.upgrades.Count > 7)
        {
            numActive = 3;
        }
        else if (Data.upgrades.Count > 4)
        {
            numActive = 2;
        }
        else if (Data.upgrades.Count > 2)
        {
            numActive = 1;
        }
        foreach (Timer timer in timers)
        {
            timer.SetFill(0);
        }
        for (int i = 0; i < numActive; i++)
        {
            timers[i].SetFill(1);
            timers[i].ShowText();
        }
    }

    public void OnMove(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            Vector2 input = value.Get<Vector2>();
            myPaddle.SetVelocity(input.y);
        }
    }

    public void OnLook(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            Vector2 input = value.Get<Vector2>();
            if (Data.upgrades.Contains("aim"))
            {
                myPaddle.SetSpin(-input.x);
            }
            if (Data.upgrades.Contains("move"))
            {
                myPaddle.SetHorizontalMove(input.x);
            }
            if (Data.upgrades.Contains("wind"))
            {
                myPaddle.ChangeWind(input.y, true);
            }
            if (Data.upgrades.Contains("curve"))
            {
                myPaddle.ChangeCurve(input.y, true);
            }
        }
    }

    public void OnAbility1(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            float input = value.Get<float>();
            if (input > 0 && !abilityCooldowns[0])
            {
                if (Data.upgrades.Contains("big") || Data.upgrades.Contains("small"))
                {
                    abilityCooldowns[0] = true;
                    myPaddle.ChangeBallSize();
                    timers[0].StartTimer(cooldownTimes[0]);
                }
            }
        }
    }

    public void OnAbility2(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            float input = value.Get<float>();
            if (input > 0 && !abilityCooldowns[1])
            {
                if (Data.upgrades.Contains("speedb") || Data.upgrades.Contains("slow"))
                {
                    abilityCooldowns[1] = true;
                    myPaddle.ChangeBallSpeed();
                    timers[1].StartTimer(cooldownTimes[1]);
                }
            }
        }
    }

    public void OnAbility4(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            float input = value.Get<float>();
            if (input > 0 && !abilityCooldowns[2])
            {
                if (Data.upgrades.Contains("speedp"))
                {
                    myPaddle.SpeedPaddle();
                    abilityCooldowns[2] = true;
                    timers[2].StartTimer(cooldownTimes[2]);
                }
                else if (Data.upgrades.Contains("flash") && enemyPaddleAI != null)
                {
                    enemyPaddleAI.Flashbang();
                    abilityCooldowns[2] = true;
                    timers[2].StartTimer(cooldownTimes[2]);
                }
            }
        }
    }

    public void OnAbility5(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            float input = value.Get<float>();
            if (input > 0 && !abilityCooldowns[3])
            {
                if (Data.upgrades.Contains("reverse"))
                {
                    myPaddle.ReverseBall();
                    abilityCooldowns[3] = true;
                    timers[3].StartTimer(cooldownTimes[3]);
                }
                else if (Data.upgrades.Contains("teleports"))
                {
                    myPaddle.TeleportPaddle();
                    abilityCooldowns[3] = true;
                    timers[3].StartTimer(cooldownTimes[3]);
                }
            }
        }
    }

    public void OnAbility6(InputValue value)
    {
        if (!pauseMenu.GetPause())
        {
            float input = value.Get<float>();
            if (input > 0 && !abilityCooldowns[4])
            {
                if (enemyPaddle != null)
                {
                    if (Data.upgrades.Contains("freeze"))
                    {
                        enemyPaddle.FreezePaddle();
                        abilityCooldowns[4] = true;
                        timers[4].StartTimer(cooldownTimes[4]);
                    }
                    else if (Data.upgrades.Contains("teleporte"))
                    {
                        enemyPaddle.TeleportPaddle();
                        abilityCooldowns[4] = true;
                        timers[4].StartTimer(cooldownTimes[4]);
                    }
                }
            }
        }
    }

    public void TimerFinished(int index)
    {
        abilityCooldowns[index] = false;
    }

    public void SetEnemyPaddle(GameObject newPaddle)
    {
        enemyPaddle = newPaddle.GetComponent<PaddleMovement>();
        if (newPaddle.GetComponent<PaddleControlAIStandard>() != null)
        {
            enemyPaddleAI = newPaddle.GetComponent<PaddleControlAIStandard>();
        }
    }

    public void Flashbang(float time)
    {
        flashScreen.Flash(time);
    }
}
