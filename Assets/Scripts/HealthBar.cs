using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.6f, 0f);
    Camera mainCam;
    private int maxH = 100;

    void Awake() { mainCam = Camera.main; }

    void Start()
    {
        if (target == null && transform.parent != null) target = transform.parent;
    }

    public void SetMax(int m)
    {
        maxH = Mathf.Max(1, m);
        if (fillImage != null) fillImage.fillAmount = 1f;
    }

    public void SetHealth(int current)
    {
        if (fillImage == null) return;
        fillImage.fillAmount = Mathf.Clamp01((float)current / maxH);
    }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        Vector3 dir = transform.position - mainCam.transform.position;
        dir.y = 0; // keep upright
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
