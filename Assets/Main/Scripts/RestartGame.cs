using UnityEngine;
using UnityEngine.SceneManagement; // Add this to use SceneManager
using UnityEngine.UI;

public class RestartGame : MonoBehaviour
{
    public Button restartButton;

    private void Start()
    {
        // Make sure the button click event uses unscaled time
        restartButton.onClick.AddListener(RestartScene);
    }

    public void RestartScene()
    {
        Debug.Log("Button Clicked");
        // Temporarily set time scale to 1 to handle the button click
        Time.timeScale = 1f;
        // Use unscaled time to handle the button click
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        // Ensure the UI continues to update using unscaled delta time
        if (restartButton != null)
        {
            restartButton.interactable = true; // Or any other update logic
        }
    }
}
