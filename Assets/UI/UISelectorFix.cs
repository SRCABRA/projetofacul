using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectorFix : MonoBehaviour
{
    public GameObject defaultSelectedButton;

    void Update()
    {
        // Se não houver botão selecionado no EventSystem
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detecta se há input do controle/teclado (horizontal/vertical navigation)
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                // Força o botão padrão a ser selecionado
                EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            }
        }
    }
}
