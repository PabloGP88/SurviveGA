using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Dna _dna;
    private int _moveIndex = 0;
    private bool _isActive = true;
    
    private float _stepTimer = 0f;
    [SerializeField] private float stepDuration = 0.1f; // seconds per step
    
    private void Start()    
    {
        _dna = GetComponent<Dna>();
    }

    private void Update()
    {
        if (!_isActive) return;

        _stepTimer += Time.deltaTime;
        
        if (_stepTimer >= stepDuration)
        {
            _stepTimer = 0f;

            if (_moveIndex < _dna.directions.Length)
            {
                transform.Translate(_dna.directions[_moveIndex] * _dna.stepSize);
                _moveIndex++;
            }
            else
            {
                _isActive = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("food"))
        {
            _dna.foodEaten++;

            if (!_dna.firstFood)
            {
                _dna.firstFood = true;
                _dna.stepsToFirstFood = _moveIndex;
            }
            
        } else if (other.CompareTag("negative"))
        {
            _dna.fitness -= 50f;
            _isActive = false;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (other.CompareTag("poisson"))
        {
            _dna.fitness -= 1f;
        }
    }

    public int GetTotalMoves()
    {
        return _moveIndex;
    }
    public void ResetAgent()
    {
        _moveIndex = 0;
        _isActive = true;
        _dna.fitness = 0;
        _dna.foodEaten = 0;
        _dna.stepsToFirstFood = 0;
        _dna.firstFood = false;
    }
}
