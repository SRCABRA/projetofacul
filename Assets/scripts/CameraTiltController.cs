using UnityEngine;
using Cinemachine;

public class CameraTiltController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    [Header("Horizontal Settings")]
    public float maxHorizontalTilt = 20f;
    public float horizontalSpeed = 100f;
    public float horizontalReturnSpeed = 5f;

    [Header("Vertical Settings")]
    public float maxVerticalTilt = 15f;
    public float verticalSpeed = 80f;
    public float verticalReturnSpeed = 4f;

    [Header("Mouse Sensitivity")]
    public float mouseSensitivityMultiplier = 0.5f;
    public float mouseThreshold = 0.1f;

    [Header("Composer Offset Settings")]
    public float maxOffsetX = 2f;
    public float maxOffsetY = 1f;
    public float offsetSmoothTime = 0.2f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float yawVelocity = 0f;
    private float pitchVelocity = 0f;

    private Vector2 offsetVelocity;
    private Vector2 currentOffset;

    private CinemachineComposer composer;

    void Start()
    {
        if (virtualCam != null)
            virtualCam.TryGetComponent(out composer);
    }

    void Update()
    {
        if (composer == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float rightStickX = Input.GetAxis("RightStickHorizontal");
        float rightStickY = Input.GetAxis("RightStickVertical");

        float processedMouseX = ProcessMouseInput(mouseX);
        float processedMouseY = ProcessMouseInput(mouseY);

        float inputYaw = processedMouseX + rightStickX;
        float inputPitch = -(processedMouseY + rightStickY);

        // Rotação de inclinação (Pitch/Yaw visual)
        if (Mathf.Abs(inputYaw) > 0.01f)
        {
            currentYaw += inputYaw * horizontalSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, -maxHorizontalTilt, maxHorizontalTilt);
        }
        else
        {
            currentYaw = Mathf.SmoothDamp(currentYaw, 0f, ref yawVelocity, 1f / horizontalReturnSpeed);
        }

        if (Mathf.Abs(inputPitch) > 0.01f)
        {
            currentPitch += inputPitch * verticalSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, -maxVerticalTilt, maxVerticalTilt);
        }
        else
        {
            currentPitch = Mathf.SmoothDamp(currentPitch, 0f, ref pitchVelocity, 1f / verticalReturnSpeed);
        }

        // Atualiza rotação local (opcional se quiser que o pivot incline visualmente)
        transform.localRotation = Quaternion.Euler(30f + currentPitch, currentYaw, 0f);

        // Atualiza offset do composer para “mover a câmera”
        Vector2 targetOffset = new Vector2(
            inputYaw * maxOffsetX,
            inputPitch * maxOffsetY
        );

        currentOffset = Vector2.SmoothDamp(currentOffset, targetOffset, ref offsetVelocity, offsetSmoothTime);
        composer.m_TrackedObjectOffset = currentOffset;
    }

    float ProcessMouseInput(float input)
    {
        if (Mathf.Abs(input) > mouseThreshold)
        {
            float sign = Mathf.Sign(input);
            float adjusted = Mathf.InverseLerp(mouseThreshold, 1f, Mathf.Abs(input));
            return sign * Mathf.SmoothStep(0f, 1f, adjusted) * mouseSensitivityMultiplier;
        }
        return 0f;
    }
}
