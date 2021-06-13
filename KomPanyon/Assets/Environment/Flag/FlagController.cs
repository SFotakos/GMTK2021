using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kom"))
        {
            if (collision.GetComponent<PlayerController>().m_CompanionController.isJoined)
            {
                Debug.Log("Touched Flag");
                int sceneCount = SceneManager.sceneCountInBuildSettings;
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                if (currentSceneIndex < sceneCount - 1)
                {
                    Debug.Log("Load Next");
                    SceneManager.LoadScene(currentSceneIndex + 1, LoadSceneMode.Single);
                }
                else
                {
                    Debug.Log("Load Ending");
                    SceneManager.LoadScene("EndingScene", LoadSceneMode.Single);
                }
            }
        }
    }
}
