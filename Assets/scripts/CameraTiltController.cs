using UnityEngine;

public class CameraTiltController : MonoBehaviour
{
    public Transform cameraTarget;

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

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float yawVelocity = 0f;
    private float pitchVelocity = 0f;

    void Update()
    {
        // Leitura do mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Leitura do controle
        float rightStickX = Input.GetAxis("RightStickHorizontal");
        float rightStickY = Input.GetAxis("RightStickVertical"); // crie esse no Input Manager (5th axis, geralmente)

        // Processar mouse input com curva suave
        float processedMouseX = ProcessMouseInput(mouseX);
        float processedMouseY = ProcessMouseInput(mouseY);

        // Combina inputs
        float inputYaw = processedMouseX + rightStickX;
        float inputPitch = -(processedMouseY + rightStickY); // invertido pra cima ser positivo

        // ROTACAO HORIZONTAL (YAW)
        if (Mathf.Abs(inputYaw) > 0.01f)
        {
            currentYaw += inputYaw * horizontalSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, -maxHorizontalTilt, maxHorizontalTilt);
        }
        else
        {
            currentYaw = Mathf.SmoothDamp(currentYaw, 0f, ref yawVelocity, 1f / horizontalReturnSpeed);
        }

        // ROTACAO VERTICAL (PITCH)
        if (Mathf.Abs(inputPitch) > 0.01f)
        {
            currentPitch += inputPitch * verticalSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, -maxVerticalTilt, maxVerticalTilt);
        }
        else
        {
            currentPitch = Mathf.SmoothDamp(currentPitch, 0f, ref pitchVelocity, 1f / verticalReturnSpeed);
        }

        // Aplica rotação baseada nos dois eixos
        transform.position = cameraTarget.position;
        transform.rotation = Quaternion.Euler(30f + currentPitch, currentYaw, 0f); // base 30º + pitch
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
