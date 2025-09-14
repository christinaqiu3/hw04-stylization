using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public AudioClip goalSound;
    public AudioSource audioSource;
    private bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        // audioSource = GetComponent<AudioSource>();
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
            if (goalSound)
                audioSource.PlayOneShot(goalSound);

            UIManager.Instance.AddScore(1);

        Destroy(gameObject);

        }

    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
