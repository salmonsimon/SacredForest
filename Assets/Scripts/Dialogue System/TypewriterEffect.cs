using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>()
    {
        {new HashSet<char>() {'.', '!', '?', '-'}, 0.3f },
        {new HashSet<char>() {',', ';', ':'}, 0.1f }
    };

    [SerializeField] private float typewriterSpeed = 15f;

    private Coroutine typingCoroutine;

    private TMP_Text textLabel;
    private string textToType;
    private float duration;

    public bool IsRunning { get; private set; }
    public bool FinishedWaitingTime { get; private set; }

    public void Run(Dialogue dialogue, TMP_Text textLabel)
    {
        this.textToType = dialogue.Text;
        this.textLabel = textLabel;
        this.duration = dialogue.Duration;

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void Reset()
    {
        StopAllCoroutines();

        this.textToType = string.Empty;
        this.textLabel = null;
        this.duration = 0;

        FinishedWaitingTime = false;
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        StopAllCoroutines();
        OnTypingCompleted();
        StartCoroutine(WaitForWaitingTime());
    }

    private IEnumerator TypeText()
    {
        DialogueBubble dialogueBubble = GetComponent<DialogueBubble>();
        dialogueBubble.ScaleBubbleDimensions(textToType.Length);

        IsRunning = true;

        textLabel.maxVisibleCharacters = 0;
        textLabel.text = textToType;

        float time = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            time += Time.deltaTime * typewriterSpeed;

            charIndex = Mathf.FloorToInt(time);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                textLabel.maxVisibleCharacters = i + 1;

                if (IsPunctuation(textToType[i], out float waitDuration) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitDuration);
                }
                else if (IsPunctuation(textToType[i], out float waitDuration2) && !isLast && (textToType[i + 1] == '.'))
                {
                    yield return new WaitForSeconds(waitDuration2 / 2);
                }
            }

            yield return null;
        }

        OnTypingCompleted();
        StartCoroutine(WaitForWaitingTime());
    }

    private void OnTypingCompleted()
    {
        IsRunning = false;
        textLabel.maxVisibleCharacters = textToType.Length;
    }

    private IEnumerator WaitForWaitingTime()
    {
        yield return new WaitForSeconds(duration);

        FinishedWaitingTime = true;
    }

    private bool IsPunctuation(char character, out float waitDuration)
    {
        foreach(KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations)
        {
            if (punctuationCategory.Key.Contains(character))
            {
                waitDuration = punctuationCategory.Value;
                return true;
            }
        }

        waitDuration = default;
        return false;
    }
}
