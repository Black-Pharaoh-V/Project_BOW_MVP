using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    // --- 1. MENU Button Function ---
    // Loads the game's initial start screen/main menu scene
    public void GoToMenu()
    {
        // Use the exact name of your start scene
        SceneManager.LoadScene("StartScene"); 
        Debug.Log("Loading StartScene (Main Menu).");
    }
    // --- 2. RETRY Button Function ---
    // Loads the scene where the main game/arena takes place
    public void RetryGame()
    {
        // Use the exact name of your main arena scene
        SceneManager.LoadScene("MainScene"); 
        Debug.Log("Loading MainScene for retry.");
    }
    // --- 3. EXIT Button Function ---
    // Stops the application (works in both Editor and built game)
    public void ExitGame()
    {
        Debug.Log("Game is exiting...");

        // This conditional compilation block ensures the code behaves correctly:
        // In the Editor, it stops the game. In a built game, it quits the application.
        #if UNITY_EDITOR
            // Stops play mode in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Closes the built application
            Application.Quit();
        #endif
    }
}
