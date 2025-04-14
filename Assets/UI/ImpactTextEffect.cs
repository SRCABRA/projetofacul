using System.Collections;
using UnityEngine;
using TMPro;

public class ImpactTextEffect : MonoBehaviour
{
    public TextMeshProUGUI[] words; // Array de palavras (Cada palavra separada em um TextMeshProUGUI)
    public float delayBetweenWords = 0.3f; // Tempo entre a aparição de cada palavra
    public float impactScale = 1.5f; // O tamanho máximo da "explosão"
    public float scaleDuration = 0.2f; // Tempo do efeito de "batida"

    [Header("Tela de Tremor")]
    public RectTransform panelTransform; // O painel que vai tremer (UI)
    public float shakeDuration = 0.2f; // Tempo do tremor (agora menor para sincronizar)
    public float shakeAmount = 10f; // Intensidade do tremor

    private Vector3 originalPanelPos; // Guarda a posição original do painel

    void Start()
    {
        originalPanelPos = panelTransform.anchoredPosition; // Salva posição original
        StartCoroutine(ShowTextWithImpact());
    }

    IEnumerator ShowTextWithImpact()
    {
        for (int i = 0; i < words.Length; i++)
        {
            TextMeshProUGUI word = words[i];
            word.gameObject.SetActive(true); // Ativa a palavra

            yield return StartCoroutine(ScaleEffect(word.transform)); // 🔥 Impacta a palavra primeiro
            yield return StartCoroutine(PanelShake()); // 🔥 Depois o tremor da tela
            
            yield return new WaitForSeconds(delayBetweenWords); // Espera um pouco antes da próxima palavra
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
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textTransform.localScale = targetScale;

        elapsedTime = 0;
        while (elapsedTime < scaleDuration)
        {
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
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

            elapsed += Time.deltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = originalPanelPos; // Retorna à posição original
    }
}
