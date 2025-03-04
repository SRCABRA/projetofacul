using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private float timer;
    public GameObject bullet;
    void Start()
    {
       timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1f && GameObject.FindGameObjectsWithTag("Enemy").Length > 0){ //verifica se o tempo é maior que 1 segundo e se tem inimigos na cena
                Shoot();
                timer = 0;
            }
        }

    void Shoot(){
        Instantiate(bullet, transform.position, transform.rotation);
        Debug.Log("Atirou");
    }   
}
