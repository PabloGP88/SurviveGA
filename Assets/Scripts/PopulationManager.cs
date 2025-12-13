using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int populationSize;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float generationTime = 60f;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Slider _speedSlider;
    
    [Header("Genetic Algorithm Settings")]
    [SerializeField] private float mutationRate = 0.05f;
    [SerializeField] private int eliteCount = 2;
    
    private List<GameObject> _population = new List<GameObject>();
    private int _currentGeneration = 1;
    private float _timer = 0f;
    private float _bestFitnessLastGen = 0f;
    private float _averageFitnessLastGen = 0f;

    private void Start()
    {
        _speedSlider.value = Time.timeScale;
        CreatePopulation();
        UpdateDebugText();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= generationTime)
        {
            NextGeneration();
        }
        
        UpdateDebugText();
    }

    private void CreatePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject agent = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            agent.GetComponent<Dna>().InitData();
            _population.Add(agent);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void NextGeneration()
    {
        // Calculate fitness for all agents based on their performance
        foreach (GameObject agent in _population)
        {
            agent.GetComponent<Dna>().CalculateFitness();
        }
        
        // Sort by fitness
        _population = _population.OrderByDescending(a => a.GetComponent<Dna>().fitness).ToList();
        _bestFitnessLastGen = _population[0].GetComponent<Dna>().fitness;
        _averageFitnessLastGen = _population.Average(a => a.GetComponent<Dna>().fitness);
        
        List<GameObject> newPopulation = new List<GameObject>();
        
        // ELITISM: Keep the best N agents
        for (int i = 0; i < eliteCount && i < _population.Count; i++)
        {
            GameObject elite = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            Dna eliteDna = elite.GetComponent<Dna>();
            eliteDna.InitData();
            
            Dna bestDna = _population[i].GetComponent<Dna>();
            eliteDna.stepSize = bestDna.stepSize;
            System.Array.Copy(bestDna.directions, eliteDna.directions, eliteDna.directions.Length);
            eliteDna.fitness = 0;
            
            elite.GetComponent<SpriteRenderer>().color = Color.cyan;
            
            newPopulation.Add(elite);
        }
        
        // Create children for the rest of the population
        while (newPopulation.Count < populationSize)
        {
            // Tournament selection for better parent diversity
            GameObject parent1 = TournamentSelection();
            GameObject parent2 = TournamentSelection();
            
            GameObject child = Crossover(parent1, parent2);
            child.GetComponent<Dna>().Mutate(mutationRate);
            
            newPopulation.Add(child);
        }
        
        // Clean up old population
        foreach (GameObject agent in _population)
        {
            Destroy(agent);
        }
        
        _population = newPopulation;
        _currentGeneration++;
        _timer = 0f;
    }
    
    // Tournament selection: pick best from random sample
    // ReSharper disable Unity.PerformanceAnalysis
    private GameObject TournamentSelection(int tournamentSize = 4)
    {
        GameObject best = _population[Random.Range(0, _population.Count)];
        float bestFitness = best.GetComponent<Dna>().fitness;
        
        for (int i = 1; i < tournamentSize; i++)
        {
            GameObject candidate = _population[Random.Range(0, _population.Count)];
            float candidateFitness = candidate.GetComponent<Dna>().fitness;
            
            if (candidateFitness > bestFitness)
            {
                best = candidate;
                bestFitness = candidateFitness;
            }
        }
        
        return best;
    }
    
    // UNIFORM CROSSOVER 
    // ReSharper disable Unity.PerformanceAnalysis
    private GameObject Crossover(GameObject parent1, GameObject parent2)
    {
        GameObject child = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
        Dna childDna = child.GetComponent<Dna>();
        childDna.InitData();
        
        Dna parent1Dna = parent1.GetComponent<Dna>();
        Dna parent2Dna = parent2.GetComponent<Dna>();
        
        int split = Random.Range(0, childDna.directions.Length);
        
        for (int j = 0; j < childDna.directions.Length; j++)
        {
            childDna.directions[j] = (Random.value < 0.5) 
                ? parent1Dna.directions[j] 
                : parent2Dna.directions[j];
        }
        
        return child;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateDebugText()
    {
        float currentBestFitness = _population.Count > 0 
            ? _population.Max(a => a.GetComponent<Dna>().fitness) 
            : 0f;
        
        debugText.text = $"Generation: {_currentGeneration}\n" +
                         $"Time: {generationTime - _timer:F1}s\n" +
                         $"Best Fitness (Current): {currentBestFitness:F1}\n" +
                         $"Best Fitness (Last Gen): {_bestFitnessLastGen:F1}\n" +
                         $"Avg Fitness (Last Gen): {_averageFitnessLastGen:F1}\n" +
                         $"Population Size: {populationSize}\n" +
                         $"Speed x{_speedSlider.value:F1}";

        Time.timeScale = _speedSlider.value;
    }
}