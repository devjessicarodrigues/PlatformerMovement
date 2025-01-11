using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float moveSpeed = 1f;    
    public float rangeX = 2f;          
    public float rangeY = 2f;       

    private float xMin;               
    private float xMax;                
    private float yLowerLimit;        
    private float yUpperLimit;         

    private bool moveRight = true;     
    private bool moveUp = true;       

    private bool platformSide = false; 
    private bool platformUp = false;   

    void Start()
    {
        if (CompareTag("PlatformSide"))
        {
            platformSide = true;
        }
        if (CompareTag("PlatformUp"))
        {
            platformUp = true;
        }
        xMin = transform.position.x - rangeX;
        xMax = transform.position.x + rangeX;
        yLowerLimit = transform.position.y - rangeY;
        yUpperLimit = transform.position.y + rangeY;
    }

    void Update()
    {
        if (platformSide)
        {
            if (transform.position.x > xMax)
            {
                moveRight = false;
            }
            else if (transform.position.x < xMin)
            {
                moveRight = true;
            }

            if (moveRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
        }

        if (platformUp)
        {
            if (transform.position.y > yUpperLimit)
            {
                moveUp = false;
            }
            else if (transform.position.y < yLowerLimit)
            {
                moveUp = true;
            }

            if (moveUp)
            {
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            }
        }
    }
}
