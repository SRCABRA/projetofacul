using UnityEngine;
using System.Collections;

public class ManualCameraShake : MonoBehaviour
{
    public static ManualCameraShake Instance { get; private set; }
    private Transform camTransform;
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        camTransform = GetComponent<Transform>();
        originalPos = camTransform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-3f, 3f) * magnitude;
            float y = Random.Range(-3f, 3f) * magnitude;

            camTransform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame(); // <- antes era yield null, mas pra shake rápido é melhor
        }

        camTransform.localPosition = originalPos;
    }

}
