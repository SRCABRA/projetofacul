using UnityEngine;

public class StartTransitionTrigger : MonoBehaviour
{
    public ScreenTransition.TransitionDirection direction = ScreenTransition.TransitionDirection.Vertical;
    public ScreenTransition.TransitionType type = ScreenTransition.TransitionType.Close;
    public float delay = 0f;

    void Start()
    {
        Invoke(nameof(TriggerTransition), delay);
    }

    void TriggerTransition()
    {
        ScreenTransition transition = FindFirstObjectByType<ScreenTransition>();
        if (transition != null)
        {
            transition.direction = direction;
            transition.transitionType = type;
            transition.StartTransition();
        }
        else
        {
            Debug.LogWarning("ScreenTransition não encontrado na cena!");
        }
    }
}
