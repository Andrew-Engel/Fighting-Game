using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]
public class UIComboSliderBehavior : MonoBehaviour
{
    AudioSource _audioSource;
    PlayerStateMachine playerStateMachine;
    [SerializeField] float transitionTime = 0.8f;
    Slider slider;
    // color effect
    public Image sliderFill;
    public Color originalColor;
    public Color glowColor = new Color(1f, 1f, 0f, 1f); // Bright yellow for glow
    public Color specialGlowColor;

    public float transitionDuration = 0.8f;
    [SerializeField] AudioClip maxChargeSound, notEnoughChargeSound, chargingSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = sliderFill.color;
        slider = GetComponent<Slider>();
        playerStateMachine = GameObject.Find("Player").GetComponent<PlayerStateMachine>();
        playerStateMachine.OnSpecialCharge += PlayerStateMachine_OnSpecialCharge;
        playerStateMachine.OnInadequateSpecialPoints += PlayerStateMachine_OnInadequateSpecialPoints;

        _audioSource = GetComponent<AudioSource>();
    }

    private void PlayerStateMachine_OnInadequateSpecialPoints(object sender, System.EventArgs e)
    {
        _audioSource.PlayOneShot(notEnoughChargeSound);
        // Tween the slider color to the glow color
        sliderFill.DOColor(glowColor, transitionDuration).OnComplete(() =>
        {
            // Hold the glow color for a moment
            DOVirtual.DelayedCall(transitionDuration, () =>
            {
                // Tween the slider color back to the original color
                sliderFill.DOColor(originalColor, transitionDuration);
            });
        });
    }

    private void PlayerStateMachine_OnSpecialCharge(int charge)
    {
        _audioSource.PlayOneShot(chargingSound);

        // Tween the slider value and change the color during the transition
        slider.DOValue(charge, transitionDuration).SetEase(Ease.Linear).OnStart(() =>
        {
            // Change the slider color to the glow color at the start of the tween
            sliderFill.DOColor(glowColor, transitionDuration);
        }).OnComplete(() =>
        {
            // Revert the slider color back to the original color
            sliderFill.DOColor(originalColor, transitionDuration).OnComplete(() =>
            {
                // Check if the charge value is 4 or greater
                if (charge >= 4)
                {
                    // Play the sound
                    _audioSource.PlayOneShot(maxChargeSound);

                    // Change the slider color transiently
                    sliderFill.DOColor(specialGlowColor, transitionDuration).OnComplete(() =>
                    {
                        // Revert the slider color back to the original color
                        sliderFill.DOColor(originalColor, transitionDuration);
                    });
                }
            });
        });
    }

}
