using System;
using System.Collections;
using UnityEngine;

public class ButtonPlat : MonoBehaviour
{
    public ButtonSequenceController buttonSequenceController;
    public ButtonSequenceController.ButtonType buttonType;

    //animation
    public Transform artTransform;
    public float bounceDuration = 0.2f;
    public float bounceHeight = 0.1f;

    public Vector3 scaleFactor = new Vector3(1.0f, 0.8f, 1.0f);

    //color
    public Transform spriteTransform;
    public SpriteRenderer spriteRenderer;

    public float fadeSpeed = 0.5f;
    private float fadeAmount = 0.0f;
    public Vector3 spriteScaleFactor = new Vector3(1.0f, 0.8f, 1.0f);

    private bool isSuccess = false;
    public bool isButtonActive = false;

    private void Start()
    {
        EventManager.Instance.OnButtonSequenceCompleted += ButtonUnActive;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnButtonSequenceCompleted -= ButtonUnActive;
    }

    private void ButtonUnActive()
    {
        isSuccess = true;
        isButtonActive = false;
    }

    public void ButtonActive()
    {
        if (isSuccess == true) return;
        isButtonActive = true;
        //一些变化
        // Start the bounce animation
        StartCoroutine(BounceAnimation());
        StartCoroutine(ColorAnimation());

        // Call the ButtonSequenceController method
        buttonSequenceController.ProcessButtonPress(buttonType);
    }

    private IEnumerator BounceAnimation()
    {
        Vector3 originalPosition = artTransform.position;
        Vector3 originalScale = artTransform.localScale;
        Vector3 spriteOriginalScale = spriteTransform.localScale;

        Vector3 targetPosition = originalPosition - new Vector3(0, bounceHeight, 0);
        Vector3 targetScale = Vector3.Scale(originalScale, scaleFactor);
        Vector3 spriteTargetScale = Vector3.Scale(spriteOriginalScale, spriteScaleFactor);

        float elapsedTime = 0;
        while (elapsedTime < bounceDuration / 2)
        {
            artTransform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / (bounceDuration / 2));
            artTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (bounceDuration / 2));
            spriteTransform.localScale =
                Vector3.Lerp(spriteOriginalScale, spriteTargetScale, elapsedTime / (bounceDuration / 2));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < bounceDuration / 2)
        {
            artTransform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / (bounceDuration / 2));
            artTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / (bounceDuration / 2));
            spriteTransform.localScale =
                Vector3.Lerp(spriteTargetScale, spriteOriginalScale, elapsedTime / (bounceDuration / 2));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the position and scale
        artTransform.position = originalPosition;
        artTransform.localScale = originalScale;
        spriteTransform.localScale = spriteOriginalScale;
    }

    private IEnumerator ColorAnimation()
    {
        float elapsedTime = 0;
        elapsedTime = 0;
        while (elapsedTime < bounceDuration)
        {
            // Color animation
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.yellow, elapsedTime / bounceDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color=Color.yellow;
    }

    private void Update()
    {
        if (!isButtonActive)
        {
            fadeAmount += Time.deltaTime * fadeSpeed;
            fadeAmount = Mathf.Clamp01(fadeAmount);
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, fadeAmount);
        }
        else
        {
            fadeAmount = 0.0f;
        }
    }
}