using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(CanvasGroup))]
public class UIFadeInAndOut : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public float fadeInDuration = 1f;
    public float holdDuration = 1f;
    public float fadeOutDuration = 1f;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void OnEnable()
    {
        // Ensure the CanvasGroup is initially invisible
        canvasGroup.alpha = 0;

        // Create the fade-in, hold, and fade-out sequence
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.Append(canvasGroup.DOFade(1, fadeInDuration)) // Fade in
                    .AppendInterval(holdDuration) // Hold at full alpha
                    .Append(canvasGroup.DOFade(0, fadeOutDuration)); // Fade out

        // Optionally, you can set the sequence to loop
        // fadeSequence.SetLoops(-1); // Infinite loop
    }


}
