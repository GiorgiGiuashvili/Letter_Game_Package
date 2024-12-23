using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DebugModeToggler : MonoBehaviour
{
    private int tapCount = 0;
    private float tapTimeLimit = 0.5f;
    private float lastTapTime = 0f;

    private float holdStartTime = 0f;
    private bool isHolding = false;

    public bool isDebugModeActive = false;

    [Header("Debug Menu Object's")]
    public GameObject DebugPanel;
    public TextMeshProUGUI FpsText;

    [Header("Fps Limiter")]
    public FPSLimit fpsLimit = FPSLimit.NoLimit;
    private float deltaTime = 0.0f;
    public enum FPSLimit
    {
        NoLimit = 0,
        Limit30 = 30,
        Limit60 = 60,
        Limit120 = 120,
        Limit240 = 240
    }

    void Update()
    {
        // Calculate FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float currentFPS = 1.0f / deltaTime;
        FpsText.text = "Fps: " + Mathf.Ceil(currentFPS).ToString();

        // Set TextMeshPro color based on FPS range
        if (currentFPS < 30)
        {
            FpsText.color = new Color(1.0f, 0.4f, 0.4f);
        }
        else if (currentFPS >= 30 && currentFPS <= 60)
        {
            FpsText.color = new Color(0.4f, 1.0f, 0.4f);
        }
        else if (currentFPS > 60 && currentFPS <= 90)
        {
            FpsText.color = new Color(0.4f, 1.0f, 1.0f);
        }
        else if (currentFPS > 90)
        {
            FpsText.color = new Color(0.6f, 0.8f, 1.0f);
        }

        // Handle mouse/touch input for taps and holding
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastTap = Time.time - lastTapTime;
            HandleTap(timeSinceLastTap);
            lastTapTime = Time.time;

            holdStartTime = Time.time; // Start timing for hold
            isHolding = true; // Assume holding starts
        }

        if (Input.GetMouseButton(0))
        {
            if (isHolding && Time.time - holdStartTime >= 2f)
            {
                DeactivateDebugMode();
                isHolding = false; // Reset holding flag
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false; // Reset holding on mouse release
        }
    }

    private void HandleTap(float timeSinceLastTap)
    {
        if (timeSinceLastTap <= tapTimeLimit)
        {
            tapCount++;
        }
        else
        {
            tapCount = 1;
        }

        if (tapCount >= 3)
        {
            ToggleDebugMode();
            tapCount = 0;
        }
    }

    void Start()
    {
        SetFPSLimit(fpsLimit);
    }

    public void SetFPSLimit(FPSLimit limit)
    {
        Application.targetFrameRate = (int)limit;
    }

    private void ToggleDebugMode()
    {
        isDebugModeActive = !isDebugModeActive;
        DebugPanel.SetActive(isDebugModeActive);
    }

    private void DeactivateDebugMode()
    {
        isDebugModeActive = false;
        DebugPanel.SetActive(false);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
