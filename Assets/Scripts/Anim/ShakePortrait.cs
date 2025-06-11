using System.Collections;
using UnityEngine;

public class ShakePortrait : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.3f;
    public float shakeSpeed = 30f;

    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    // 🔥 Call this function to make it shake
    public void TriggerShake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            float offsetY = Mathf.Cos(Time.time * shakeSpeed * 1.1f) * shakeIntensity;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset to original position after shake
        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
