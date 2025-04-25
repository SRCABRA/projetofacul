using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public enum TransitionDirection { Vertical, Horizontal }
    public enum TransitionType { Close, Open }

    [Header("Configurações da Transição")]
    public TransitionDirection direction = TransitionDirection.Vertical;
    public TransitionType transitionType = TransitionType.Close;
    public float transitionSpeed = 1f;
    public Color barColor = Color.black;
    public bool autoDisableOnEnd = true;

    private RectTransform bar1;
    private RectTransform bar2;
    private Canvas canvas;

    private bool isTransitioning = false;
    private Action onTransitionComplete;

    void Awake()
    {
        CreateCanvasAndBars();
    }

    void CreateCanvasAndBars()
    {
        canvas = new GameObject("ScreenTransitionCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        bar1 = CreateBar("Bar1");
        bar2 = CreateBar("Bar2");

        SetAnchorsAndPivot();
    }

    RectTransform CreateBar(string name)
    {
        GameObject go = new GameObject(name, typeof(Image));
        go.transform.SetParent(canvas.transform);
        Image img = go.GetComponent<Image>();
        img.color = barColor;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        return rt;
    }

    void SetAnchorsAndPivot()
    {
        if (direction == TransitionDirection.Vertical)
        {
            bar1.anchorMin = new Vector2(0, 0.5f);
            bar1.anchorMax = new Vector2(1, 0.5f);
            bar1.pivot = new Vector2(0.5f, 0.5f);

            bar2.anchorMin = new Vector2(0, 0.5f);
            bar2.anchorMax = new Vector2(1, 0.5f);
            bar2.pivot = new Vector2(0.5f, 0.5f);
        }
        else
        {
            bar1.anchorMin = new Vector2(0.5f, 0);
            bar1.anchorMax = new Vector2(0.5f, 1);
            bar1.pivot = new Vector2(0.5f, 0.5f);

            bar2.anchorMin = new Vector2(0.5f, 0);
            bar2.anchorMax = new Vector2(0.5f, 1);
            bar2.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    public void StartTransition(Action callback = null)
    {
        if (isTransitioning) return;

        onTransitionComplete = callback;
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        float duration = 1f / transitionSpeed;
        float time = 0f;

        Vector2 startSize, endSize;

        float halfScreenHeight = Screen.height / 2f;
        float halfScreenWidth = Screen.width / 2f;

        if (direction == TransitionDirection.Vertical)
        {
            if (transitionType == TransitionType.Close)
            {
                startSize = Vector2.zero;
                endSize = new Vector2(0, halfScreenHeight);
            }
            else // Open
            {
                startSize = new Vector2(0, halfScreenHeight);
                endSize = Vector2.zero;
            }
        }
        else
        {
            if (transitionType == TransitionType.Close)
            {
                startSize = Vector2.zero;
                endSize = new Vector2(halfScreenWidth, 0);
            }
            else // Open
            {
                startSize = new Vector2(halfScreenWidth, 0);
                endSize = Vector2.zero;
            }
        }

        // Set bars at start
        bar1.sizeDelta = startSize;
        bar2.sizeDelta = startSize;

        while (time < duration)
        {
            float t = time / duration;
            Vector2 currentSize = Vector2.Lerp(startSize, endSize, t);
            bar1.sizeDelta = currentSize;
            bar2.sizeDelta = currentSize;

            if (direction == TransitionDirection.Vertical)
            {
                bar1.anchoredPosition = new Vector2(0, currentSize.y / 2f);
                bar2.anchoredPosition = new Vector2(0, -currentSize.y / 2f);
            }
            else
            {
                bar1.anchoredPosition = new Vector2(-currentSize.x / 2f, 0);
                bar2.anchoredPosition = new Vector2(currentSize.x / 2f, 0);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Garante que terminou exatamente com o tamanho final
        bar1.sizeDelta = endSize;
        bar2.sizeDelta = endSize;

        onTransitionComplete?.Invoke();

        if (autoDisableOnEnd)
        {
            Destroy(canvas.gameObject); // Remove o canvas e os elementos UI
        }

        isTransitioning = false;
    }
}
