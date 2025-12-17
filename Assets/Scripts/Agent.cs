using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Dna _dna;
    private int _moveIndex = 0;
    private bool _isActive = true;
    private Vector2 _startPosition;
    
    private float _stepTimer = 0f;
    [SerializeField] private float stepDuration = 0.1f; // seconds per step
    
    private void Start()    
    {
        _startPosition = transform.position;
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
                
                // I am converting them to cell size so they get rewarded for exploring the world
                
                Vector2 pos = new Vector2(
                    Mathf.Floor(transform.position.x/2) * 2,
                    Mathf.Floor(transform.position.y/2) * 2
                );
                
                _dna.explorationDone.Add(pos);
                
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
            _dna.hasDied = true;
            _isActive = false;
            GetComponent<SpriteRenderer>().color = Color.red;
        }

    }

    public int GetTotalMoves()
    {
        return _moveIndex;
    }

    public void ResetAgent()
    {
        _dna.foodEaten = 0;
        _dna.firstFood = false;
        _dna.stepsToFirstFood = 0;
        _dna.explorationDone.Clear();
        _dna.hasDied = false;
        _dna.firstFood = false;
        _dna.fitness = 0;
        _isActive = true;
        _moveIndex = 0;
        transform.position = _startPosition;
    }
    
}
