using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePizzaController : MonoBehaviour
{
    public float rotationSpeed = 50f; // rotação do objeto
    public float floatAmplitude = 0.5f; // amplitude do movimento de flutuação
    public float floatFrequency = 1f; // frequência do movimento de flutuação
    public float baseHeight = 1f; // altura base da flutuação

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the object
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Float the object up and down
        float newY = baseHeight + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
