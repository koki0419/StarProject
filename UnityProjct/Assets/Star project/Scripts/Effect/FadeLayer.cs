using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeLayer : MonoBehaviour
{
    [SerializeField] private Image image = null;

    public void ForceColor(Color color)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        image.color = color;
    }

    public IEnumerator FadeInEnumerator(float period)
    {
        transform.SetAsLastSibling();
        yield return FadeEnumerator(image.color, Color.clear, period);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutEnumerator(Color color, float period)
    {
        transform.SetAsLastSibling();
        yield return FadeEnumerator(Color.clear, color, period);
    }

    public IEnumerator FadeEnumerator(Color startColor, Color targetColor, float period)
    {
        float t = 0;
        while (t < period)
        {

            t += Time.deltaTime;
            Color color = Color.Lerp(startColor, targetColor, t / period);
            image.color = color;
            yield return null;
        }

        image.color = targetColor;
    }

}
