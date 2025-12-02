using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PanelType
{
    MonoSi = 0,
    PolySi = 1,
    ThinFilm = 2
}

public class SolarPanelController : MonoBehaviour
{
    [Header("Panel Configuration")]
    [SerializeField] private PanelType panelType = PanelType.MonoSi;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer panelRenderer;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private string emissionColorProperty = "_EmissionColor";

    private const float N_COATING = 2.0f;
    private const float LAMBDA = 550f;

    private float absorption;
    private float targetPower;
    private float idealThickness;
    private float currentThickness;
    private float currentPower;
    private bool isActive = false;

    private void Awake()
    {
        InitializePanelParameters();
        idealThickness = LAMBDA / (4f * N_COATING);
        currentThickness = 70f;
    }

    private void InitializePanelParameters()
    {
        switch (panelType)
        {
            case PanelType.MonoSi:
                absorption = 0.90f;
                targetPower = 0.90f;
                break;
            case PanelType.PolySi:
                absorption = 0.92f;
                targetPower = 0.92f;
                break;
            case PanelType.ThinFilm:
                absorption = 0.95f;
                targetPower = 0.94f;
                break;
        }
    }

    public void UpdateThickness(float thickness)
    {
        currentThickness = thickness;
        CalculatePower();
    }

    private void CalculatePower()
    {
        float difference = Mathf.Abs(currentThickness - idealThickness);
        float reflection = Mathf.Clamp01(difference / 150f);
        currentPower = absorption * (1f - reflection);

        if (isActive)
        {
            UpdateVisualFeedback();
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (active)
        {
            CalculatePower();
            UpdateVisualFeedback();
        }
        else
        {
            ClearVisualFeedback();
        }
    }

    private void UpdateVisualFeedback()
    {
        if (panelRenderer != null)
        {
            bool goalReached = currentPower >= targetPower;
            Color emissionColor = goalReached ? successColor : Color.black;

            Material mat = panelRenderer.material;
            if (mat.HasProperty(emissionColorProperty))
            {
                mat.SetColor(emissionColorProperty, emissionColor * 0.3f);
            }
        }
    }

    private void ClearVisualFeedback()
    {
        if (panelRenderer != null)
        {
            Material mat = panelRenderer.material;
            if (mat.HasProperty(emissionColorProperty))
            {
                mat.SetColor(emissionColorProperty, Color.black);
            }
        }
    }

    public float GetCurrentPower()
    {
        return currentPower;
    }

    public float GetTargetPower()
    {
        return targetPower;
    }

    public float GetCurrentThickness()
    {
        return currentThickness;
    }

    public bool IsGoalReached()
    {
        return currentPower >= targetPower;
    }

    public PanelType GetPanelType()
    {
        return panelType;
    }

    public string GetPanelTypeName()
    {
        return panelType switch
        {
            PanelType.MonoSi => "Mono-Si Solar Panel",
            PanelType.PolySi => "Poly-Si Solar Panel",
            PanelType.ThinFilm => "Thin-Film Solar Panel",
            _ => "Unknown"
        };
    }
}
