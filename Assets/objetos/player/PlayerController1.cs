using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TerrainUtils;
using UnityEngine.TextCore.Text;

public class PlayerController1 : MonoBehaviour
{
    // Variáveis de movimentação e física
    public float speed = 15.0f; // velocidade do personagem
    public float gravity = -10f; // gravidade normal
    public float jumpForce = 5f; // força do pulo
    private bool isGrounded; // verifica se o personagem está no chão 

    [SerializeField] private Transform foot; // ponto para verificação de colisão com o chão
    [SerializeField] private LayerMask colisaoLayer;

    private Transform MyCamera; // câmera do personagem
    private CharacterController controller; // controlador de personagem            
    public Vector3 cameraOffset; // offset da câmera em relação ao jogador

    // Variáveis do STOMP
    public float abilityActivationHeight = 5.0f; // altura mínima para ativar a habilidade
    public float extraGravityForce = -30f; // força extra para simular aumento da gravidade
    public float abilityCooldown = 2f; // tempo de recarga da habilidade

    public GameObject AreaAttack; // prefab da área de ataque
    private float lastAbilityTime = -10f; // armazena o último tempo em que a habilidade foi acionada
    private bool stompActivated = false; // variável para rastrear se a habilidade de STOMP foi ativada



    void Start()
    {
        controller = GetComponent<CharacterController>(); // pega o controlador de personagem
        MyCamera = Camera.main.transform; // pega a câmera principal
        cameraOffset = MyCamera.position - transform.position; // calcula o offset inicial da câmera
    }   

    void Update()
    {
        // Captura inputs de movimentação e calcula a direção relativa à câmera
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 position = new Vector3(horizontal, 0, vertical);
        position = MyCamera.TransformDirection(position);
        position.y = 0f; // zera o eixo y

        // Movimenta o personagem
        controller.Move(position * speed * Time.deltaTime);

        // Rotaciona o personagem na direção do movimento
        if (position != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10);
        }
        
        // Verifica se o personagem está no chão
        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer);

        // Código de pulo: se o personagem estiver no chão e o botão de pulo for pressionado
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            gravity = jumpForce;
        }

        // Nova feature: se o jogador pressionar a tecla E, ativa a habilidade se estiver em altura suficiente
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastAbilityTime + abilityCooldown)
        {
            if (transform.position.y >= abilityActivationHeight)
            {
                gravity = extraGravityForce;
                lastAbilityTime = Time.time;
                stompActivated = true; // marca que a habilidade de STOMP foi ativada
                Debug.Log("Habilidade ativada: aumentando a gravidade!");
            }
            else
            {
                Debug.Log("Altura insuficiente para ativar a habilidade.");
            }
        }

        // Atualiza a gravidade: aplica aceleração para simular efeito de queda
        if (gravity > -10f)
        {
            gravity += -25f * Time.deltaTime;
        }
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        // Atualiza a posição da câmera para seguir o jogador
        Vector3 newCameraPosition = transform.position + cameraOffset;
        newCameraPosition.y = transform.position.y + cameraOffset.y;
        MyCamera.position = newCameraPosition;
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifica se o jogador colidiu com o chão após usar a habilidade de STOMP
        if (stompActivated && ((1 << hit.gameObject.layer) & colisaoLayer) != 0)
        {
            Vector3 areaAttackPosition = new Vector3(transform.position.x, foot.position.y, transform.position.z);
            Instantiate(AreaAttack, areaAttackPosition, Quaternion.identity); // cria o objeto AreaAttack na posição do pé
            stompActivated = false; // reseta a variável stompActivated
        }

        // Verifica se o jogador colidiu com um inimigo
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player colidiu com inimigo!");
        }
    }
}