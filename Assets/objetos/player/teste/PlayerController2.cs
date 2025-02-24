using UnityEngine;


public class PlayerController2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float speed = 15.0f; //velocidade do personagem
    public float gravity = -10f; //gravidade 
    private bool isGrounded; //verifica se o personagem está no chão 
    public float jumpForce = 5f; //força do pulo   
    [SerializeField] private Transform foot; //verifica se o personagem está no chão
    [SerializeField] private LayerMask colisaoLayer;

    

    private Transform MyCamera; //camera do personagem
    private CharacterController controller; //controlador de personagem            
    public Vector3 cameraOffset; // offset da câmera em relação ao jogador

    void Start(){
        controller = GetComponent<CharacterController>(); //pega o controlador de personagem
        MyCamera = Camera.main.transform; //pega a camera principal
        cameraOffset = MyCamera.position - transform.position; // calcula o offset inicial da câmera
    }   


    void Update(){
        float horizontal = Input.GetAxis("Horizontal"); //pega o input do teclado para movimento horizontal
        float vertical = Input.GetAxis("Vertical"); //pega o input do teclado para movimento vertical
        Vector3 position = new Vector3(horizontal, 0, vertical);  //cria um vetor de posição com os inputs do teclado
       

        position = MyCamera.TransformDirection(position); //pega a direção da camera
        position.y = 0f; //zera o eixo y    

        controller.Move(position * speed * Time.deltaTime); //movimenta o personagem

        
        


        if (position != Vector3.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10); //rotaciona o personagem na direção do movimento
        }
        
        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer); //verifica se o personagem está no chão

        if(Input.GetButtonDown("Jump") && isGrounded){ //verifica se o personagem está no chão e aplica uma força para pular
            gravity = jumpForce;
        }
        if(gravity  > -10f){
            gravity += -25f * Time.deltaTime;
        }
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime); //aplica a gravidade no personagem







        // Atualiza a posição da câmera para seguir o jogador
        Vector3 newCameraPosition = transform.position + cameraOffset;
        newCameraPosition.y = transform.position.y + cameraOffset.y; // ajusta a altura da câmera para seguir o jogador
        MyCamera.position = newCameraPosition;
    }
}