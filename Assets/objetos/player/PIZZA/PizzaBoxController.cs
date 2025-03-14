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

    private Coroutine destructionTimerCoroutine; // Guarda a referência do timer de destruição
    public float destructionTime = 5f; // Tempo para ser destruído após tocar o chão

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

            // Inicia o timer para destruição
            StartDestructionTimer();
        }
        else if (collision.gameObject.CompareTag("Player") && transform.parent == null && canBeCollected)
        {
            // Cancela o timer de destruição se o jogador pegar a caixa
            StopDestructionTimer();

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

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Impede que o jogador colete a caixa antes da destruição
        canBeCollected = false;

        // Notifica o gerenciador antes de destruir
        PizzaBoxManager.instance.PizzaBoxDestroyed();

        Destroy(gameObject); // Destroi o objeto
    }

    void StartDestructionTimer()
    {
        // Garante que não tenha dois timers rodando ao mesmo tempo
        if (destructionTimerCoroutine != null)
        {
            StopCoroutine(destructionTimerCoroutine);
        }
        destructionTimerCoroutine = StartCoroutine(DestroyAfterTime(destructionTime));
    }

    void StopDestructionTimer()
    {
        if (destructionTimerCoroutine != null)
        {
            StopCoroutine(destructionTimerCoroutine);
            destructionTimerCoroutine = null; // Reseta o timer
        }
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
