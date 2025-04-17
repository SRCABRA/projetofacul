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
        GetComponent<Rigidbody>().isKinematic = true;

        // Ajusta a altura
        groundHeight = collision.contacts[0].point.y;
        Vector3 newPosition = transform.position;
        newPosition.y = groundHeight + 0.1f;
        transform.position = newPosition;

        canBeCollected = true; // <- já permite pegar sem delay

        StartDestructionTimer();
    }
    else if (collision.gameObject.CompareTag("Player") && transform.parent == null && canBeCollected)
    {
        // Cancela destruição
        StopDestructionTimer();

        // Restaura a pizza ao jogador
        LifeController lifeController = collision.gameObject.GetComponent<LifeController>();
        lifeController.RestorePizzaBox(transform);

        // Ativa invulnerabilidade de 2 segundos
        lifeController.ActivateInvulnerability(2f);

        isFloating = false;
        canBeCollected = false;
    }
}


    IEnumerator EnableCollection()
    {
        yield return new WaitForSeconds(1f); // Tempo de espera antes de permitir a coleta
        canBeCollected = true;
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

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject); // Destroi o objeto após o tempo definido
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
