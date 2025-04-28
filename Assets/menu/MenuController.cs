using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour

{
    public void EntrarNoJogo(){
        SceneManager.LoadScene("Testecenario");
    }

    public void Opções(){

    }

    public void SairDoJogo(){
        Application.Quit();
        Debug.Log("saindo");
    }
}
