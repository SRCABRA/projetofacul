using System.Collections;
using UnityEngine;
using TMPro;

public class ImpactTextEffect : MonoBehaviour
{
    public TextMeshProUGUI[] words;
    public float delayBetweenWords = 0.3f;
    public float impactScale = 1.5f;
    public float scaleDuration = 0.2f;

    [Header("Tela de Tremor")]
    public RectTransform panelTransform;
    public float shakeDuration = 0.2f;
    public float shakeAmount = 10f;

    [Header("Configuração de Impacto")]
    public bool simultaneousImpact = false;

    private Vector3 originalPanelPos;

    void OnEnable()
    {
        originalPanelPos = panelTransform.anchoredPosition;

        if (panelTransform.gameObject.activeSelf)
        {
            StartCoroutine(ShowTextWithImpact());
        }
    }

    IEnumerator ShowTextWithImpact()
    {
        if (simultaneousImpact)
        {
            // Ativa todos os textos
            foreach (TextMeshProUGUI word in words)
            {
                word.gameObject.SetActive(true);
            }

            // Inicia a animação de escala para todos ao mesmo tempo
            foreach (TextMeshProUGUI word in words)
            {
                StartCoroutine(ScaleEffect(word.transform));
            }

            // Faz o shake uma única vez
            yield return StartCoroutine(PanelShake());
        }
        else
        {
            for (int i = 0; i < words.Length; i++)
            {
                TextMeshProUGUI word = words[i];
                word.gameObject.SetActive(true);

                yield return StartCoroutine(ScaleEffect(word.transform));
                yield return StartCoroutine(PanelShake());

                yield return new WaitForSecondsRealtime(delayBetweenWords);
            }
        }
    }

    IEnumerator ScaleEffect(Transform textTransform)
    {
        Vector3 originalScale = textTransform.localScale;
        Vector3 targetScale = originalScale * impactScale;

        float elapsedTime = 0;
        while (elapsedTime < scaleDuration)
        {
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        textTransform.localScale = targetScale;

        elapsedTime = 0;
        while (elapsedTime < scaleDuration)
        {
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        textTransform.localScale = originalScale;
    }

    IEnumerator PanelShake()
    {
        float elapsed = 0;
        while (elapsed < shakeDuration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeAmount;
            panelTransform.anchoredPosition = originalPanelPos + new Vector3(randomOffset.x, randomOffset.y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = originalPanelPos;
    }
}
