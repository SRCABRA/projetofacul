using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformController : MonoBehaviour
{
    // Referência para o transform do cubo cujo eixo Y será copiado.
    public Transform targetCube;
    
    // Duração da transição suave.
    public float transitionDuration = 1.0f;
    
    // Flag para garantir que o código seja executado apenas uma vez.
    private bool hasExecuted = false;

    // Método chamado quando outro collider entra em contato com este objeto. 
    // Este objeto deve ter o Box Collider com "Is Trigger" marcado para que este evento seja disparado.
    private void OnTriggerEnter(Collider other)
    {
        // Executa se o código ainda não foi executado e se o objeto colidido possui a tag "Player".
        if (!hasExecuted && other.CompareTag("Player"))
        {
            hasExecuted = true;
            StartCoroutine(SmoothTransition());
        }
    }

    // Coroutine para fazer uma transição suave (descida) do objeto.
    private IEnumerator SmoothTransition()
    {
        Vector3 startPos = transform.position;
        float targetY = targetCube.position.y;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);
        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o objeto atinja exatamente a posição final.
        transform.position = targetPos;
    }
}