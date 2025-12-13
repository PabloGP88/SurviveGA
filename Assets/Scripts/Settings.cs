using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{   
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private TextMeshProUGUI _instructionsText;
    
    [SerializeField] private PopulationManager populationManager;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Toggle _eliteToggle;
    [SerializeField] private Button _exportCSV;

    private bool _settingsPanelActive = false;
    
    private readonly string _instructionsOn = "Press 'Esc' to hide settings";
    private readonly string _instructionsOff = "Press 'Esc' to show settings";
    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.value = 5;
        }
        
        if (_eliteToggle != null)
        {
            _eliteToggle.onValueChanged.AddListener(populationManager.ToggleEliteOnly);
        }
        
        if (_exportCSV != null)
        {
            _exportCSV.onClick.AddListener(OnExportCSVClicked);
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            _settingsPanelActive = !_settingsPanelActive;
        }
    
        UpdateDebugText();
        UpdateTimeScale();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateDebugText()
    {
        
        _settingsPanel.SetActive(_settingsPanelActive);

        _instructionsText.text = _settingsPanelActive ? _instructionsOn : _instructionsOff;
        
        if (debugText && populationManager)
        {
            debugText.text = $"Generation: {populationManager.GetCurrentGeneration()}\n" +
                             $"Time: {populationManager.GetTimeRemaining():F1}s\n" +
                             $"Best Fitness (Last Gen): {populationManager.GetBestFitnessLastGen():F1}\n" +
                             $"Avg Fitness (Last Gen): {populationManager.GetAverageFitnessLastGen():F1}\n" +
                             $"Population Size: {populationManager.GetPopulationSize()}\n" +
                             $"Mutation Rate: {populationManager.GetMutationRate()}\n" +
                             $"Speed x{(speedSlider != null ? speedSlider.value : Time.timeScale):F1}";
        }
    }

    private void UpdateTimeScale()
    {
        if (speedSlider)
        {
            Time.timeScale = speedSlider.value;
        }
    }
    
    private void OnExportCSVClicked()
    {
        if (populationManager != null)
        {
            populationManager.ExportToCSV();
        }
    }
}