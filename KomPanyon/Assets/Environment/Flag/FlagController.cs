using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kom"))
        {
            Debug.Log("Touched Flag");
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex < sceneCount)
            {
                SceneManager.LoadScene(currentSceneIndex+1, LoadSceneMode.Single);
            } else
            {
                SceneManager.LoadScene("Ending Scene", LoadSceneMode.Single);
            }
        }
    }
}
