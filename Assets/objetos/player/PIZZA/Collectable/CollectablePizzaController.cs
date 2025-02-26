using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePizzaController : MonoBehaviour
{
    public float rotationSpeed = 50f; // rotação do objeto
    public float floatAmplitude = 0.5f; // amplitude do movimento de flutuação
    public float floatFrequency = 1f; // frequência do movimento de flutuação
    public float floatOffset = 1f; // offset para aumentar a altura da flutuação

    public float timeToDestroy = 5.0f; // tempo para destruir o objeto

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the object
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Float the object up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude + floatOffset;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
