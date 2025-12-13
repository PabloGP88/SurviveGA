using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Food : MonoBehaviour
{
    public Vector2 xRange;
    public Vector2 yRange;
    
    public void Initialize(Vector2 _xRange, Vector2 _yRange)
    {
        xRange = _xRange;
        yRange = _yRange;
        
        transform.position = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if (other.CompareTag("food"))
           transform.position = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
    }
}