#if UNITY_STANDALONE
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public AudioSource audioSource;
    public GameObject soundButton;
    public Sprite soundSprite0;
    public Sprite soundSprite1;
    public Sprite soundSprite2;
    public Sprite soundSprite3;

    private int soundLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);

        // Остановить или возобновить игру
        if (isPaused)
        {
            Time.timeScale = 0f; // Остановить игровое время
        }
        else
        {
            Time.timeScale = 1f; // Возобновить игровое время
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);
    }
    public void soundButtonPressed()
    {
        soundLevel++;
        if (soundLevel > 3) soundLevel = 0;
        switch (soundLevel)
        {
            case 0:
                soundButton.GetComponent<Image>().sprite = soundSprite0;
                audioSource.mute = true;
                break;
            case 1:
                audioSource.mute = false;
                audioSource.volume = 0.3F;
                soundButton.GetComponent<Image>().sprite = soundSprite1;
                break;
            case 2:
                audioSource.mute = false;
                audioSource.volume = 0.6F;
                soundButton.GetComponent<Image>().sprite = soundSprite2;
                break;
            case 3:
                audioSource.mute = false;
                audioSource.volume = 1.0F;
                soundButton.GetComponent<Image>().sprite = soundSprite3;
                break;
        }
    }
    public void Close()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
#endif