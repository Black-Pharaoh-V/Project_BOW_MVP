using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI resultText;

    [Header("Delay Settings")]
    public float messageDisplayTime = 2f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerDied()
    {
        if (gameEnded) return;
        gameEnded = true;
        ShowResultMessage("You Lose!");
        Invoke(nameof(LoadEndScene), messageDisplayTime);
    }

    public void OnEnemyDied()
    {
        if (gameEnded) return;
        gameEnded = true;
        ShowResultMessage("You Win!");
        Invoke(nameof(LoadEndScene), messageDisplayTime);
    }

    private void ShowResultMessage(string message)
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = message;
        }
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene("EndScene");
    }
}
