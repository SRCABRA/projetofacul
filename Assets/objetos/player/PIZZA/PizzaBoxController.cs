using UnityEngine;
using System.Collections;

public class PizzaBoxController : MonoBehaviour
{
    private bool isFloating = false;
    private Vector3 floatDirection = Vector3.up;
    private float floatSpeed = 0.5f;
    private float rotationSpeed = 50f;
    private bool canBeCollected = false;
    private float groundHeight;

    void Start()
    {
        
    }

    void Update()
    {
        if (isFloating && transform.parent == null)
        {
            FloatAndRotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("chao") && transform.parent == null)
        {
            isFloating = true;
            GetComponent<Rigidbody>().isKinematic = true; // Desativa a física para flutuar

            // Ajusta a posição para ficar logo acima do chão
            groundHeight = collision.contacts[0].point.y;
            Vector3 newPosition = transform.position;
            newPosition.y = groundHeight + 0.1f; // Ajusta a altura para ficar um pouco acima do chão
            transform.position = newPosition;

            StartCoroutine(EnableCollection());
        }
        else if (collision.gameObject.CompareTag("Player") && transform.parent == null && canBeCollected)
        {
            // Restaura a caixa de pizza ao jogador
            collision.gameObject.GetComponent<LifeController>().RestorePizzaBox(transform);
            isFloating = false;
            canBeCollected = false;
        }
    }

    IEnumerator EnableCollection()
    {
        yield return new WaitForSeconds(1f); // Tempo de espera antes de permitir a coleta
        canBeCollected = true;
    }

    void FloatAndRotate()
    {
        // Flutuar
        transform.Translate(floatDirection * floatSpeed * Time.deltaTime);

        // Alternar direção de flutuação
        if (transform.position.y > groundHeight + 1.5f)
        {
            floatDirection = Vector3.down;
        }
        else if (transform.position.y < groundHeight + 1f)
        {
            floatDirection = Vector3.up;
        }

        // Girar
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}