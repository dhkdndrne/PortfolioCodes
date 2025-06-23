using System;
using UnityEngine;

public class SpSlider : SliderHandler
{
    [SerializeField] private RectTransform spBar;
    [SerializeField] private RectTransform durationBar;

    private bool isSkillActive;
    private void Awake()
    {
        var skill =transform.root.GetComponent<Operator>().GetSkill();
        skill?.DurationRatio.Subscribe(val =>
        {
            sliderBar.value = val;
        });
    }
    private void OnEnable()
    {
        spBar.gameObject.SetActive(true);
        durationBar.gameObject.SetActive(false);
    }
    
    public void ToggleSliderBars(bool isSkillActive)
    {
        spBar.gameObject.SetActive(!isSkillActive);
        durationBar.gameObject.SetActive(isSkillActive);
        
        sliderBar.fillRect = isSkillActive ? durationBar : spBar;
        this.isSkillActive = isSkillActive;
    }
    
    protected override void SetSliderValue(float value)
    {
        if (isSkillActive)
            return;
        
        sliderBar.value = value;
    }
}
