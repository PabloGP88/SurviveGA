using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Food : MonoBehaviour
{
    public Vector2 xRange;
    public Vector2 yRange;

    public int maxColl = 100;
    private int _currentColl = 0;
    public void Initialize(Vector2 _xRange, Vector2 _yRange)
    {
        xRange = _xRange;
        yRange = _yRange;
        
        transform.position = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Agent"))
        {
            _currentColl++;
            if (_currentColl >= maxColl)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            transform.position = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
        }
    }

    public void Reset()
    {
        _currentColl = 0;
    }
}