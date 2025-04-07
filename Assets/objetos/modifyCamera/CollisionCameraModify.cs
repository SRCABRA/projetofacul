using UnityEngine;
using Cinemachine;

public class CollisionCameraModify : MonoBehaviour
{
    [Header("Referências")]
    public CinemachineVirtualCamera vcam;
    public Transform player;
    public Transform lookTarget; // Empty no centro ou frente do jogador

    [Header("Close Camera Offset")]
    public Vector3 closeOffset = new Vector3(0, 2.5f, -5f); // Atrás do jogador

    [Header("Rotação horizontal extra (graus)")]
    public float horizontalAngleOffset = 180f; // 180 = olhar pelas costas

    [Header("Damping")]
    private float normalHorizontalDamping = 0.5f;
    private float normalVerticalDamping = 0.5f;
    private float closeHorizontalDamping = 8f;
    private float closeVerticalDamping = 8f;

    [Header("Velocidades")]
    public float transitionSpeed = 0.5f;
    public float lookRotationSpeed = 1.5f;
    public float returnLookRotationSpeed = 5f; // mais rápido pra voltar ao normal

    private Vector3 originalOffset;
    private bool isClose = false;
    private bool isInCloseCutscene = false;

    private CinemachineTransposer transposer;
    private CinemachineComposer composer;

    // Controle da transição suave de retorno
    private bool transitioningBack = false;
    private float transitionBackTimer = 0f;
    private float transitionBackDuration = 0.5f;

    // Para restaurar screen position original
    private float originalScreenX;
    private float originalScreenY;

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
        // Transição suave entre os offsets
        Vector3 targetOffset = isClose ? closeOffset : originalOffset;
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, Time.deltaTime * transitionSpeed);

        if (transitioningBack)
        {
            transitionBackTimer += Time.deltaTime;

            if (transitionBackTimer >= transitionBackDuration)
            {
                transitioningBack = false;

                // Só agora voltamos o LookAt para o player
                vcam.LookAt = player;

                // Restaura damping e posição original do Composer
                ApplyNormalDamping();
                composer.m_ScreenX = originalScreenX;
                composer.m_ScreenY = originalScreenY;
            }
        }
    }

    void LateUpdate()
    {
        if (isInCloseCutscene || transitioningBack)
        {
            Vector3 direction = (lookTarget.position - vcam.transform.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion offsetRotation = Quaternion.Euler(0, horizontalAngleOffset, 0);
            Quaternion finalRotation = baseRotation * offsetRotation;

            float speed = isInCloseCutscene ? lookRotationSpeed : returnLookRotationSpeed;

            vcam.transform.rotation = Quaternion.Slerp(vcam.transform.rotation, finalRotation, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
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
        transitioningBack = false;

        vcam.LookAt = null; // Controle manual da rotação
        ApplyCloseDamping();
    }

    void ExitCloseMode()
    {
        isClose = false;
        isInCloseCutscene = false;

        // Inicia transição suave de volta
        transitioningBack = true;
        transitionBackTimer = 0f;

        vcam.LookAt = null;

        // Temporariamente zera damping para transição limpa
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
}
