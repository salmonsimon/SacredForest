using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    private TypewriterEffect typewriterEffect;
    public TypewriterEffect TypewriterEffect { get { return typewriterEffect; } }

    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private TMP_Text textLabel;
    public TMP_Text TextLabel { get { return textLabel; } set { textLabel = value; } }

    [SerializeField] GameObject spaceBarIcon;

    [HideInInspector] public Vector3 initialPosition = Vector3.zero;
    [HideInInspector] public Vector3 finalPosition = Vector3.zero;
    [HideInInspector] public Transform lookAt;

    private float minWidth = 500;
    private float maxWidth = 800;
    private float widthIncrease = 25;

    private float minHeight = 200;
    private float maxHeight = 350;
    private float heightIncrease = 50;


    public bool IsOpen { get; private set; }

    private float animationDuration = Config.MEDIUM_DELAY;
    public float AnimationDuration { get; private set; }

    private Vector3 originalScale = Vector3.zero;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        originalScale = dialogueBox.transform.localScale;

        spaceBarIcon.SetActive(false);

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = GetComponent<Image>().sprite.pivot / rectTransform.sizeDelta;

        ResetDialogueBubble();
    }

    public void UpdateDialogueBubble()
    {
        if (!IsOpen)
            return;

        if (lookAt)
        {
            Vector3 displacement = lookAt.position - dialogueBox.transform.position;

            dialogueBox.transform.position += displacement;
        }
    }

    public IEnumerator OpenDialogueBubble(int stringLength)
    {
        //TODO: despues borrar este timer
        yield return new WaitForSeconds(2f);

        ScaleBubbleDimensions(stringLength);

        float startTime = Time.time;
        float endTime = startTime + animationDuration;

        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = originalScale;

        Vector3 initialPosition = lookAt.position;
        Vector3 finalPosition = this.finalPosition;

        dialogueBox.SetActive(true);
        dialogueBox.transform.localScale = initialScale;

        while (Time.time < endTime)
        {
            dialogueBox.transform.localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / animationDuration);
            dialogueBox.transform.position = Vector3.Lerp(initialPosition, finalPosition, (Time.time - startTime) / animationDuration);

            yield return null;
        }

        IsOpen = true;
    }

    public IEnumerator CloseDialogueBubble()
    {
        float startTime = Time.time;
        float endTime = startTime + animationDuration;

        Vector3 initialScale = originalScale;
        Vector3 finalScale = Vector3.zero;

        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = lookAt.position;

        while (Time.time < endTime)
        {
            dialogueBox.transform.localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / animationDuration);
            dialogueBox.transform.position = Vector3.Lerp(initialPosition, finalPosition, (Time.time - startTime) / animationDuration);

            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void ResetDialogueBubble()
    {
        IsOpen = false;

        dialogueBox.SetActive(false);
        spaceBarIcon.SetActive(false);

        TextLabel.text = string.Empty;
    }

    public void ScaleBubbleDimensions(int stringLength)
    {
        RectTransform rectTransform = dialogueBox.GetComponent<RectTransform>();
        rectTransform.sizeDelta = GetBubbleDimensions(stringLength);

        rectTransform.pivot = GetComponent<Image>().sprite.pivot / rectTransform.sizeDelta;
    }

    private Vector2 GetBubbleDimensions(int stringLength)
    {
        float width = minWidth;
        float height = minHeight;

        if (stringLength > 16 && stringLength <= 28)
        {
            stringLength -= 16;

            width += widthIncrease * stringLength;
        }
        else if (stringLength > 28)
        {
            stringLength -= 28;

            width = maxWidth;
            height += heightIncrease * (stringLength / 28);
        }
        else if (stringLength > 112)
        {
            width = maxWidth;
            height = maxHeight;
        }

        return new Vector2(width, height);
    }

    public void ShowSpaceBarIcon(bool value = true)
    {
        spaceBarIcon.SetActive(value);
    }
}