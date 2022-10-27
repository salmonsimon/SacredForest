using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>()
    {
        {new HashSet<char>() {'.', '!', '?'}, 0.6f },
        {new HashSet<char>() {',', ';', ':'}, 0.3f }
    };

    [SerializeField] private float typewriterSpeed = 50f;

    private Coroutine typingCoroutine;
    private TMP_Text textLabel;

    private string textToType;

    public bool IsRunning { get; private set; }

    public void Run(string textToType, TMP_Text textLabel)
    {
        this.textToType = textToType;
        this.textLabel = textLabel;

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        StopCoroutine(typingCoroutine);
        OnTypingCompleted();
    }

    private IEnumerator TypeText()
    {
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
            }

            yield return null;
        }

        OnTypingCompleted();
    }

    private void OnTypingCompleted()
    {
        IsRunning = false;
        textLabel.maxVisibleCharacters = textToType.Length;
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
