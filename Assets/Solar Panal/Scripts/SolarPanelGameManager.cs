using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarPanelGameManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private SolarPanelController monoSiPanel;
    [SerializeField] private SolarPanelController polySiPanel;
    [SerializeField] private SolarPanelController thinFilmPanel;

    [Header("UI References")]
    [SerializeField] private Dropdown panelDropdown;
    [SerializeField] private Slider thicknessSlider;
    [SerializeField] private TextMeshProUGUI panelTypeText;
    [SerializeField] private TextMeshProUGUI thicknessValueText;
    [SerializeField] private TextMeshProUGUI currentPowerText;
    [SerializeField] private TextMeshProUGUI targetPowerText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusIndicator;
    [SerializeField] private TextMeshProUGUI overallStatusText;

    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;

    [Header("Victory")]
    [SerializeField] private GameObject victoryPanel;

    private SolarPanelController currentPanel;
    private bool victoryTriggered = false;

    private void Start()
    {
        SetupSlider();
        SetupDropdown();
        SwitchToPanel(0);
    }

    private void SetupSlider()
    {
        if (thicknessSlider != null)
        {
            thicknessSlider.minValue = 0f;
            thicknessSlider.maxValue = 300f;
            thicknessSlider.value = 10f;
            thicknessSlider.onValueChanged.AddListener(OnThicknessChanged);
        }
    }

    private void SetupDropdown()
    {
        if (panelDropdown != null)
        {
            panelDropdown.onValueChanged.AddListener(OnPanelSelected);
        }
    }

    private void OnPanelSelected(int index)
    {
        SwitchToPanel(index);
    }

    private void SwitchToPanel(int index)
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
        }

        currentPanel = index switch
        {
            0 => monoSiPanel,
            1 => polySiPanel,
            2 => thinFilmPanel,
            _ => monoSiPanel
        };

        if (currentPanel != null)
        {
            currentPanel.SetActive(true);

            if (thicknessSlider != null)
            {
                currentPanel.UpdateThickness(thicknessSlider.value);
            }

            UpdateUI();
        }
    }

    private void OnThicknessChanged(float thickness)
    {
        if (currentPanel != null)
        {
            currentPanel.UpdateThickness(thickness);
            UpdateUI();
        }
    }

    private void Update()
    {
        CheckAllPanelsComplete();
    }

    private void UpdateUI()
    {
        if (currentPanel == null) return;

        if (panelTypeText != null)
        {
            panelTypeText.text = currentPanel.GetPanelTypeName();
        }

        if (thicknessValueText != null)
        {
            thicknessValueText.text = $"{currentPanel.GetCurrentThickness():F1} nm";
        }

        if (currentPowerText != null)
        {
            currentPowerText.text = $"Power: {currentPanel.GetCurrentPower() * 100f:F1}%";
        }

        if (targetPowerText != null)
        {
            targetPowerText.text = $"Target: {currentPanel.GetTargetPower() * 100f:F0}%";
        }

        bool goalReached = currentPanel.IsGoalReached();

        if (statusText != null)
        {
            statusText.text = goalReached ? "SUCCESS!" : "Adjust thickness...";
            statusText.color = goalReached ? successColor : failColor;
        }

        if (statusIndicator != null)
        {
            statusIndicator.color = goalReached ? successColor : failColor;
        }
    }

    private void CheckAllPanelsComplete()
    {
        if (victoryTriggered) return;

        int completedCount = 0;
        int totalPanels = 0;

        if (monoSiPanel != null)
        {
            totalPanels++;
            if (monoSiPanel.IsGoalReached()) completedCount++;
        }

        if (polySiPanel != null)
        {
            totalPanels++;
            if (polySiPanel.IsGoalReached()) completedCount++;
        }

        if (thinFilmPanel != null)
        {
            totalPanels++;
            if (thinFilmPanel.IsGoalReached()) completedCount++;
        }

        if (overallStatusText != null)
        {
            overallStatusText.text = $"Panels Complete: {completedCount}/{totalPanels}";
        }

        if (completedCount == totalPanels && totalPanels > 0)
        {
            OnAllPanelsComplete();
        }
    }

    private void OnAllPanelsComplete()
    {
        if (!victoryTriggered)
        {
            victoryTriggered = true;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }

            Debug.Log("All solar panels optimized successfully!");
        }
    }
}
