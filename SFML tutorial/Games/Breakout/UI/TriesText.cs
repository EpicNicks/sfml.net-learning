﻿using SFML.Graphics;

using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Scheduling.Coroutines;
using SFML_tutorial.Properties;
using System.Collections;

namespace SFML_tutorial.Games.Breakout.UI;
public class TriesText : UIAnchored
{
    private Coroutine? redFlashCoroutine;
    private int currentTriesAmount;
    public int CurrentTriesAmount 
    { 
        get => currentTriesAmount;
        set
        {
            if (currentTriesAmount != value)
            {
                if (value == 1)
                {
                    redFlashCoroutine = StartCoroutine(TextFlashRed());
                }
                else
                {
                    if (redFlashCoroutine is not null)
                    {
                        if (StopCoroutine(redFlashCoroutine))
                        {
                            redFlashCoroutine = null;
                        }
                    }
                    if (value == 0)
                    {
                        // GameWindow.LoadScene(gameOverSceneName);
                    }
                }
            }
            currentTriesAmount = value;
        }
    }
    private readonly int maximumTries;
    private readonly Text displayText;

    public TriesText(int maximumTries)
    {
        this.maximumTries = maximumTries;
        CurrentTriesAmount = maximumTries;
        displayText = new Text
        {
            Font = new(Resources.Roboto_Black),
            CharacterSize = 24,
            FillColor = Color.White,
        };
    }
    public override void Update()
    {
        displayText.Position = PositionLocally(displayText.GetLocalBounds());
        displayText.DisplayedString = $"Tries: {CurrentTriesAmount}";
    }
    public override (UIAnchor x, UIAnchor y) Anchors => (UIAnchor.END, UIAnchor.END);
    public override List<Drawable> Drawables => [displayText];

    private IEnumerator TextFlashRed()
    {
        Color curFillColor = displayText.FillColor;
        while (true)
        {
            displayText.FillColor = Color.Red;
            yield return new WaitForSeconds(0.1f);
            displayText.FillColor = curFillColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
