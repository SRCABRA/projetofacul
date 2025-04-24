using UnityEngine;
using UnityEngine.EventSystems; // Necessário pra selecionar botão via código

public class PauseMenu : MonoBehaviour
{
    [Header("Referências")]
    public GameObject pauseCanvas; // O canvas do menu de pausa
    public GameObject firstSelectedButton; // O botão que será selecionado automaticamente (ex: "continuar")

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        Time.timeScale = 0.003f;

        // Garante que um botão esteja selecionado para navegação com controle
        EventSystem.current.SetSelectedGameObject(null); // Limpa qualquer seleção anterior
        EventSystem.current.SetSelectedGameObject(firstSelectedButton); // Seleciona o botão inicial
    }

    void ResumeGame()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;

        // Limpa seleção ao sair (opcional)
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    public void continuar(){
        ResumeGame();
    }

    public void FecharJogo()
    {
        Debug.Log("Fechando o jogo...");
        Application.Quit();
    }

}
