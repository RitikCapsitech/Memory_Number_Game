using UnityEngine;
using TMPro;
using System.Collections;
using GameWise.ShapeMatching;

public class Cell : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro numberText;
    public SpriteRenderer spriteRenderer;

    [Header("Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private int targetNumber;
    public bool IsFilled = false;

    private void Awake()
    {
        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshPro>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTargetNumber(int number)
    {
        targetNumber = number;
        numberText.text = number.ToString();
        numberText.gameObject.SetActive(false);
    }

    public void ShowNumber()
    {
        numberText.gameObject.SetActive(true);
        SetAlpha(0.4f); // faded look

    }

    private void SetAlpha(float alpha)
    {
        Color color = numberText.color;
        color.a = alpha;
        numberText.color = color;
    }

    public void HideNumber()
    {
        numberText.gameObject.SetActive(false);
    }

    public bool TryPlaceNumber(int number)
    {
        if (IsFilled) return false;

        if (number == targetNumber)
        {
            
            IsFilled = true;
            SoundManager.Instance.Matched();
            spriteRenderer.color = correctColor; 
            numberText.gameObject.SetActive(true);
            SetAlpha(1f);

            StartCoroutine(ScaleUpAndDown(gameObject));

           
            FindObjectOfType<GameManager>()?.CheckLevelComplete();
            return true;
        }
        else
        {
            SoundManager.Instance.NotMatched();

            StartCoroutine(ShowWrongFeedback());
            return false;
        }
    }

    System.Collections.IEnumerator ShowWrongFeedback()
{
    Color originalColor = spriteRenderer.color;
    spriteRenderer.color = wrongColor;
    
    // Shake effect
    Vector3 originalPos = transform.position;
    float shakeDuration = 0.5f;
    float shakeAmount = 0.1f;
    float elapsed = 0f;
    
    while (elapsed < shakeDuration)
    {
        float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
        float y = originalPos.y + Random.Range(-shakeAmount, shakeAmount);
        transform.position = new Vector3(x, y, originalPos.z);
        
        elapsed += Time.deltaTime;
        yield return null;
    }
    
    transform.position = originalPos;
    spriteRenderer.color = originalColor;
}
       

    public int GetTargetNumber()
    {
        return targetNumber;
    }
    private IEnumerator ScaleUpAndDown(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float duration = 0.3f;

        // SCALE UP
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smoothstep

            target.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        target.transform.localScale = targetScale;

        // SCALE DOWN
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            target.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        target.transform.localScale = originalScale;
    }
}