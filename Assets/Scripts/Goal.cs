using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public AudioClip winSound;
    private AudioSource audioSource;
    private bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Player"))
        {
            gameEnded = true;
            Debug.Log("You Win!");

            if (winSound != null && audioSource != null)
                audioSource.PlayOneShot(winSound);

            // Show win UI (if you have one)
            UIManager.Instance.ShowWinScreen();

            // Reload scene after 3 seconds (optional)
            Invoke("ReloadScene", 3f);
        }

    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
