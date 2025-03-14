using UnityEngine;
using System.Collections;
using Cinemachine;

public class PizzaBoxManager : MonoBehaviour
{
    public static PizzaBoxManager instance; // Singleton para fácil acesso

    public CinemachineVirtualCamera virtualCamera; // Referência para a Cinemachine Virtual Camera
    public float cameraMoveSpeed = 5f; // Velocidade da câmera ao subir
    public float cameraTargetHeight = 50f; // Altura final da câmera

    private int totalPizzaBoxes;
    private int destroyedPizzaBoxes = 0;
    
    void Awake()
    {
        // Garante que temos apenas um gerenciador ativo
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Conta todas as caixas de pizza ativas na cena
        totalPizzaBoxes = GameObject.FindGameObjectsWithTag("PizzaBox").Length;
    }

    public void PizzaBoxDestroyed()
    {
        destroyedPizzaBoxes++;

        // Se todas as caixas sumiram, começa a contagem para mover a câmera
        if (destroyedPizzaBoxes >= totalPizzaBoxes)
        {
            StartCoroutine(MoveCameraUp());
        }
    }

    IEnumerator MoveCameraUp()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos antes de mover a câmera

        Transform cameraTransform = virtualCamera.transform;
        Vector3 startPosition = cameraTransform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, cameraTargetHeight, startPosition.z);

        float elapsedTime = 0f;
        float duration = (cameraTargetHeight - startPosition.y) / cameraMoveSpeed;

        while (elapsedTime < duration)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que a câmera chega exatamente no ponto alvo
        cameraTransform.position = targetPosition;
    }
}
