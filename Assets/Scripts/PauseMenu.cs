using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    GameObject organizer;
    [SerializeField] Cursor cursor;
    SettingsManager settingsManager;

    void Awake()
    {
        organizer = transform.GetChild(0).gameObject;
        settingsManager = FindFirstObjectByType<SettingsManager>();
    }

    void Start()
    {
        organizer.SetActive(false);
    }

    public void OnPause(InputValue input)
    {
        if (paused)
        {
            if (!settingsManager.GetSettingsOpen())
            {
                paused = false;
                organizer.SetActive(false);
                Time.timeScale = 1;
                if (SceneManager.GetActiveScene().name == "MainGame")
                {
                    cursor.Hide(true);
                }
            }
        }
        else
        {
            paused = true;
            Time.timeScale = 0;
            organizer.SetActive(true);
            if (SceneManager.GetActiveScene().name == "MainGame")
            {
                cursor.Hide(false);
            }
        }
    }

    public void LoadMenu()
    {
        paused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        settingsManager.OpenSettings();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool GetPause()
    {
        return paused;
    }
}
