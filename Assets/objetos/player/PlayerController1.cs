using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TerrainUtils;
using UnityEngine.TextCore.Text;

public class PlayerController1 : MonoBehaviour
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

    void Start(){
        controller = GetComponent<CharacterController>(); //pega o controlador de 
        MyCamera = Camera.main.transform; //pega a camera principal

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




    }
}




