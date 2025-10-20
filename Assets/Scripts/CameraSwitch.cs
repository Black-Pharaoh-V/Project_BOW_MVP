using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Assign your cameras here")]
    public Camera mainCamera;
    public Camera isoCamera;

    private bool isISOActive = false;

    // This function will be linked to the UI Button
    public void SwitchCamera()
    {
        isISOActive = !isISOActive;

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(!isISOActive);
        if (isoCamera != null)
            isoCamera.gameObject.SetActive(isISOActive);
    }
}
