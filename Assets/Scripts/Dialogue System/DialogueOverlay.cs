using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueOverlay : MonoBehaviour
{
    private TypewriterEffect typewriterEffect;
    public TypewriterEffect TypewriterEffect { get { return typewriterEffect; } }

    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private TMP_Text textLabel;
    public TMP_Text TextLabel { get { return textLabel; } set { textLabel = value; } }

    [SerializeField] GameObject spaceBarIcon;

    public bool IsOpen { get; private set; }

    private float animationDuration = Config.MEDIUM_DELAY;
    public float AnimationDuration { get; private set; }

    private Vector3 originalScale = Vector3.one;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();

        spaceBarIcon.SetActive(false);

        ResetDialogueOverlay();
    }

    public IEnumerator OpenDialogueOverlay()
    {
        yield return new WaitForSeconds(Config.LARGE_DELAY);

        float startTime = Time.time;
        float endTime = startTime + animationDuration;

        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = originalScale;

        dialogueBox.SetActive(true);
        dialogueBox.transform.localScale = initialScale;

        while (Time.time < endTime)
        {
            dialogueBox.transform.localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / animationDuration);

            yield return null;
        }

        dialogueBox.transform.localScale = finalScale;

        IsOpen = true;
    }

    public IEnumerator CloseDialogueOverlay()
    {
        float startTime = Time.time;
        float endTime = startTime + animationDuration;

        Vector3 initialScale = originalScale;
        Vector3 finalScale = Vector3.zero;

        while (Time.time < endTime)
        {
            dialogueBox.transform.localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / animationDuration);

            yield return null;
        }

        dialogueBox.transform.localScale = finalScale;

        ResetDialogueOverlay();
    }

    public void ResetDialogueOverlay()
    {
        IsOpen = false;

        dialogueBox.transform.localScale = Vector3.zero;

        dialogueBox.SetActive(false);
        spaceBarIcon.SetActive(false);

        TextLabel.text = string.Empty;
    }

    public void ShowSpaceBarIcon(bool value = true)
    {
        spaceBarIcon.SetActive(value);
    }
}
