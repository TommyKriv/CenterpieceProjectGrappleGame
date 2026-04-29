using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] int frequency;
    [SerializeField] float intensity;

    private void Update()
    {
        this.transform.position = (transform.position + Vector3.up * Mathf.Cos(Time.time * frequency) * intensity);
    }
}
