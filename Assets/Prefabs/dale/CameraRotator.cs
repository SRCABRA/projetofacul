using UnityEngine;

public class AutoOrbitCamera : MonoBehaviour
{
    public Transform target;      // Objeto alvo
    public float distance = 5.0f; // Distância da câmera para o alvo
    public float orbitSpeed = 20.0f; // Velocidade de rotação

    void LateUpdate()
    {
        if (target)
        {
            // Rotaciona a câmera em torno do alvo no eixo Y
            transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

            // Mantém a câmera sempre olhando para o alvo
            transform.LookAt(target);
        }
    }
}
