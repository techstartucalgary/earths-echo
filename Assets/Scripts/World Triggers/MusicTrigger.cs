using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    // This will show a dropdown with all GameplayContext options in the Inspector.
    public GameplayContext stateForMusic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.SetGameplayMusic(stateForMusic);
            Debug.Log("successfully set gameplay music to: " + stateForMusic);
        }
        Destroy(gameObject);
    }
}
