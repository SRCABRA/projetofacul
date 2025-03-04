using UnityEngine;
using System.Collections;

public class MovePlatformController : MonoBehaviour
{
    public float targetY = -2f;          // altura exata de destino
    public float animationDuration = 1f;   // duração da animação
    private Vector3 originalPosition;
    private bool hasMoved = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    // Certifique-se de que o collider está marcado como Trigger.
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !hasMoved || other.gameObject.CompareTag("Enemy") && !hasMoved)
        {
            StartCoroutine(MovePlatformDownCoroutine());
        }

    }

    //fazer ele descer e subir

    IEnumerator MovePlatformDownCoroutine()
    {
            Vector3 targetPos = new Vector3(originalPosition.x, targetY, originalPosition.z);
            // Calcula a velocidade necessária para alcançar a altura em animationDuration
            float speed = Mathf.Abs(originalPosition.y - targetY) / animationDuration;

            while (Vector3.Distance(transform.position, targetPos) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos; // Garante que a posição final seja exatamente targetPos
            hasMoved = true;    
    }
}