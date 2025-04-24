using UnityEngine;
using Cinemachine;

public class CollisionCameraModify : MonoBehaviour
{
    [Header("Referências")]
    public CinemachineVirtualCamera vcam;
    public Transform player;
    public Transform lookTarget;
    public Transform cameraPivot;
    private CameraTiltController tiltController;

    [Header("Close Camera Offset")]
    public Vector3 closeOffset = new Vector3(0, 2.5f, -5f);

    [Header("Close Camera Rotation (em graus)")] // NOVO:
    public Vector3 closeRotationEuler = new Vector3(30f, 180f, 0f); // Personalizável via Inspector

    [Header("Rotação horizontal extra (graus)")]
    public float horizontalAngleOffset = 180f;

    [Header("Damping")]
    private float normalHorizontalDamping = 0.5f;
    private float normalVerticalDamping = 0.5f;
    private float closeHorizontalDamping = 8f;
    private float closeVerticalDamping = 8f;

    [Header("Velocidades")]
    private float transitionSpeed = 0.5f;
    private float lookRotationSpeed = 1.5f;

    private Vector3 originalOffset;
    private bool isClose = false;
    private bool isInCloseCutscene = false;

    private CinemachineTransposer transposer;
    private CinemachineComposer composer;

    private bool isReturningToNormal = false;
    private float returnTimer = 0f;
    private float returnDuration = 0.5f;

    private float originalScreenX;
    private float originalScreenY;

    private bool allowManualRotation = false;

    void Start()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        composer = vcam.GetCinemachineComponent<CinemachineComposer>();

        originalOffset = transposer.m_FollowOffset;
        originalScreenX = composer.m_ScreenX;
        originalScreenY = composer.m_ScreenY;

        ApplyNormalDamping();

        vcam.LookAt = cameraPivot;
        tiltController = cameraPivot.GetComponent<CameraTiltController>();
    }

    void Update()
    {
        Vector3 targetOffset = isClose ? closeOffset : originalOffset;
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, Time.deltaTime * transitionSpeed);

        if (isClose)
        {
            // NOVO: Aplica rotação customizada no CameraPivot (durante o Close Mode)
            Quaternion targetRotation = Quaternion.Euler(closeRotationEuler);
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, Time.deltaTime * lookRotationSpeed);
        }

        if (isReturningToNormal)
        {
            returnTimer += Time.deltaTime;

            if (returnTimer >= returnDuration)
            {
                isReturningToNormal = false;
                allowManualRotation = false;

                AlignCameraToPlayer();

                if (tiltController != null)
                    tiltController.enabled = true;

                vcam.LookAt = cameraPivot;
                ApplyNormalDamping();
                composer.m_ScreenX = originalScreenX;
                composer.m_ScreenY = originalScreenY;
            }
        }
    }

    void LateUpdate()
    {
        if (allowManualRotation && !isClose) // Apenas se não estiver no modo close
        {
            Vector3 direction = (lookTarget.position - cameraPivot.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion offsetRotation = Quaternion.Euler(0, horizontalAngleOffset, 0);
            Quaternion finalRotation = baseRotation * offsetRotation;

            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, finalRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraClose"))
        {
            EnterCloseMode();
        }
        else if (other.CompareTag("CameraNormal"))
        {
            ExitCloseMode();
        }
    }

    void EnterCloseMode()
    {
        isClose = true;
        isInCloseCutscene = true;
        isReturningToNormal = false;

        allowManualRotation = false; // Desativa rot manual enquanto usamos rotação fixa
        vcam.LookAt = null;

        if (tiltController != null)
            tiltController.enabled = false;

        ApplyCloseDamping();
    }

    void ExitCloseMode()
    {
        isClose = false;
        isInCloseCutscene = false;

        isReturningToNormal = true;
        returnTimer = 0f;
        allowManualRotation = true;

        vcam.LookAt = null;
        composer.m_HorizontalDamping = 0f;
        composer.m_VerticalDamping = 0f;
        composer.m_ScreenX = 0.5f;
        composer.m_ScreenY = 0.5f;
    }

    void ApplyCloseDamping()
    {
        composer.m_HorizontalDamping = closeHorizontalDamping;
        composer.m_VerticalDamping = closeVerticalDamping;
        composer.m_DeadZoneWidth = 0.9f;
        composer.m_DeadZoneHeight = 0.9f;
    }

    void ApplyNormalDamping()
    {
        composer.m_HorizontalDamping = normalHorizontalDamping;
        composer.m_VerticalDamping = normalVerticalDamping;
        composer.m_DeadZoneWidth = 0.05f;
        composer.m_DeadZoneHeight = 0.05f;
    }

    void AlignCameraToPlayer()
    {
        Vector3 direction = (lookTarget.position - cameraPivot.position).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion offsetRotation = Quaternion.Euler(0, horizontalAngleOffset, 0);
        Quaternion finalRotation = baseRotation * offsetRotation;

        cameraPivot.rotation = finalRotation;
    }
}
