using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;
    
    private List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Update()
    {
        foreach(FloatingText txt in floatingTexts)
            txt.UpdateFloatingText();
    }

    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        FloatingText floatingText = GetFloatingText();

        floatingText.go.transform.localScale *= .3f;

        floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;
        floatingText.txt.color = color;

        floatingText.go.transform.position = position;
        floatingText.motion = motion;
        floatingText.duration = duration;

        floatingText.Show();
    }

    private FloatingText GetFloatingText()
    {
        FloatingText floatingText = floatingTexts.Find(x => !x.active);

        if (floatingText == null)
        {
            floatingText = new FloatingText();

            floatingText.go = Instantiate(textPrefab);
            floatingText.go.transform.SetParent(textContainer.transform);
            floatingText.txt = floatingText.go.GetComponent<Text>();

            floatingTexts.Add(floatingText);
        }

        return floatingText;
    }
}
