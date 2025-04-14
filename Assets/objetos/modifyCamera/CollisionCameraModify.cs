using UnityEngine;
using Cinemachine;

public class CollisionCameraModify : MonoBehaviour
{
    [Header("Referências")]
    public CinemachineVirtualCamera vcam;
    public Transform player;
    public Transform lookTarget;

    [Header("Close Camera Offset")]
    public Vector3 closeOffset = new Vector3(0, 2.5f, -5f);

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

    // Transição
    private bool isReturningToNormal = false;
    private float returnTimer = 0f;
    private float returnDuration = 0.5f;

    // Screen position original
    private float originalScreenX;
    private float originalScreenY;

    // Controle de rotação manual
    private bool allowManualRotation = false;

    void Start()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        composer = vcam.GetCinemachineComponent<CinemachineComposer>();

        originalOffset = transposer.m_FollowOffset;
        originalScreenX = composer.m_ScreenX;
        originalScreenY = composer.m_ScreenY;

        ApplyNormalDamping();
        vcam.LookAt = player;
    }

    void Update()
    {
        Vector3 targetOffset = isClose ? closeOffset : originalOffset;
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, Time.deltaTime * transitionSpeed);

        if (isReturningToNormal)
        {
            returnTimer += Time.deltaTime;

            if (returnTimer >= returnDuration)
            {
                isReturningToNormal = false;
                allowManualRotation = false;

                // AQUI: Força a rotação para alinhar corretamente antes de entregar ao Cinemachine
                AlignCameraToPlayer();

                // Reativa LookAt e damping suaves
                vcam.LookAt = player;
                ApplyNormalDamping();
                composer.m_ScreenX = originalScreenX;
                composer.m_ScreenY = originalScreenY;
            }
        }
    }

    void LateUpdate()
    {
        if (allowManualRotation)
        {
            Vector3 direction = (lookTarget.position - vcam.transform.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion offsetRotation = Quaternion.Euler(0, horizontalAngleOffset, 0);
            Quaternion finalRotation = baseRotation * offsetRotation;

            vcam.transform.rotation = Quaternion.Slerp(vcam.transform.rotation, finalRotation, Time.deltaTime * lookRotationSpeed);
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

        allowManualRotation = true;
        vcam.LookAt = null;

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
        Vector3 direction = (lookTarget.position - vcam.transform.position).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion offsetRotation = Quaternion.Euler(0, horizontalAngleOffset, 0);
        Quaternion finalRotation = baseRotation * offsetRotation;

        // Força a rotação exata sem suavização
        vcam.transform.rotation = finalRotation;
    }
}
