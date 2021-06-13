using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private bool _isPaused;
    private bool _isDead;
    private bool _hasWon;

    public TextMeshProUGUI levelText;

    [SerializeField] PlayerController playerController;

    private void Awake()
    {
        LockCursor();
        string text;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0)
        {
            levelText.text = "Tutorial";
        } else if (sceneIndex == SceneManager.sceneCountInBuildSettings-1)
        {
            levelText.text = "Home";
        } else
        {
            levelText.text = "Level: " + sceneIndex;
        }
        
    }

    private void Update()
    {
        //if (!_isDead && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (_isPaused)
        //        ResumeGame();
        //    else
        //        PauseGame();
        //}

        if (!_isDead && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        //musicAudioSource.Pause();
        if (!_isDead && !_hasWon)
            //pauseScreen.gameObject.SetActive(true);
        _isPaused = true;
        UnlockCursor();
    }
    public void ResumeGame()
    {
        LockCursor();
        Time.timeScale = 1f;
        //musicAudioSource.Play();
        //pauseScreen.gameObject.SetActive(false);
        _isPaused = false;
    }
    public void Died()
    {
        
        _isDead = true;
        PauseGame();
        //deathScreen.gameObject.SetActive(true);
        
    }

    public void Restart()
    {
        _isDead = false;
        _hasWon = false;
        //deathScreen.gameObject.SetActive(false);
        //victoryScreen.gameObject.SetActive(false);
        //musicAudioSource.Stop();
        //RestartGame();
        ResumeGame();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        playerController.Reset();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void UnlockCursor()
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
