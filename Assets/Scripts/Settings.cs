using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Slider speedSlider;

    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.value = Time.timeScale;
        }
    }

    private void Update()
    {
        UpdateDebugText();
        UpdateTimeScale();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateDebugText()
    {
        if (debugText && populationManager)
        {
            debugText.text = $"Generation: {populationManager.GetCurrentGeneration()}\n" +
                             $"Time: {populationManager.GetTimeRemaining():F1}s\n" +
                             $"Best Fitness (Current): {populationManager.GetCurrentBestFitness():F1}\n" +
                             $"Best Fitness (Last Gen): {populationManager.GetBestFitnessLastGen():F1}\n" +
                             $"Avg Fitness (Last Gen): {populationManager.GetAverageFitnessLastGen():F1}\n" +
                             $"Population Size: {populationManager.GetPopulationSize()}\n" +
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
}