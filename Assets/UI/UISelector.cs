using UnityEngine;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    public GameObject firstSelected;

    void OnEnable()
    {
        // Aguarda 1 frame pra garantir que tudo esteja inicializado
        StartCoroutine(SelectFirst());
    }

    System.Collections.IEnumerator SelectFirst()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
