using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PArallaxBG : MonoBehaviour
{
    public Camera cam;
    public Transform subject;

    Vector2 startPosition;

    float startZ;

    Vector2 travel => (Vector2)cam.transform.position - startPosition;

    float distanceFormSubject => transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z + (distanceFormSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parallaxFactor => Mathf.Abs(distanceFormSubject) / clippingPlane;

    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void Update()
    {
        Vector2 newPos = startPosition + travel * 0.9f;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }


}
