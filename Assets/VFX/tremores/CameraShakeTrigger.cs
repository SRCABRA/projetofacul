using UnityEngine;
using Cinemachine;

public class CameraShakeTrigger : MonoBehaviour
{
    public static CameraShakeTrigger Instance;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(float force = 1f)
    {
        Debug.Log("Gerando tremor de câmera!");
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(force);
        }
    }
}
