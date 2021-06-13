using UnityEngine;

public class FlagController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kom"))
        {
            Debug.Log("Next Scene");
        }
    }
}
