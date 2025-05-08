using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private float fadeInSpeed = 1f;
    [SerializeField] private float fadeOutSpeed = 1f;

    private TextMeshProUGUI text;
    private Coroutine fadeCoroutine;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            Color color = text.color;
            color.a = 0f; // Start with the text fully transparent
            text.color = color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTextToAlpha(1f, fadeInSpeed));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTextToAlpha(0f, fadeOutSpeed));
        }
    }

    private IEnumerator FadeTextToAlpha(float targetAlpha, float duration)
    {
        if (text == null) yield break;

        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;
    }
}
