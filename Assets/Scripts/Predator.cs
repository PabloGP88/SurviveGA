using System;
using UnityEngine;

public class Predator : MonoBehaviour
{
    [SerializeField] private Transform _predator;
    [SerializeField] private Transform[] target;
    [SerializeField] private float speed;
    
    private const float StepDuration = 0.1f;
    private float _stepTimer = 0f;

    private Vector3 _initialPos;
    private Transform _currentTarget;
    private int _currentTargetIndex;

    private void Start()
    {
        _initialPos = _predator.position;
        _currentTargetIndex = 0;
    }

    private void Update()
    {
        _stepTimer += Time.deltaTime;

        if (_stepTimer >= StepDuration)
        {
            Move();
        }
    }

    private void Move()
    {
        _stepTimer = 0f;
        
        _currentTarget = target[_currentTargetIndex];
        
        _predator.position =   Vector3.MoveTowards(
            _predator.position, 
            _currentTarget.position, 
            speed
        );
        
        float distance = Vector3.Distance(_predator.position, _currentTarget.position);
        
        if (distance < 0.1f)
        {
            if (_currentTargetIndex == target.Length - 1)
            {
                _currentTargetIndex = 0;
            }
            else
            {
                {
                    _currentTargetIndex++;
                }
            }
        }
    }
    public void Reset()
    {
        _predator.position = _initialPos;
        _currentTargetIndex = 0;
    }

    
}
