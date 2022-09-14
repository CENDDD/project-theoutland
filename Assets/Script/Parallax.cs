using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField]
    private Transform mainCameraPosition;

    [SerializeField]
    private float backgroundMoveSpeed;
    private float directionX;

    [SerializeField]
    private float offsetByX = 13f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        directionX = Input.GetAxis("Horizontal") * backgroundMoveSpeed * Time.fixedDeltaTime;

        transform.position = new Vector2(transform.position.x + directionX, transform.position.y);
        if(transform.position.x - mainCameraPosition.position.x < -offsetByX)
        {
            transform.position = new Vector2(mainCameraPosition.position.x + offsetByX, transform.position.y);
        }   
        else if(transform.position.x - mainCameraPosition.position.x > offsetByX)
        {
            transform.position = new Vector2(mainCameraPosition.position.x - offsetByX, transform.position.y);
        } 
    }
}
