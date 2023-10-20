using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class audioControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider BGM_Slider;
    public Slider SFX_Slider;
    public Slider MASTER_Slider;

    public void AudioControl()
    {
        float BGM_Sound = BGM_Slider.value;
        float SFX_Sound = SFX_Slider.value;
        float MASTER_Sound = MASTER_Slider.value;

        if (BGM_Sound <= -40f) audioMixer.SetFloat("BGM", -80);
        else audioMixer.SetFloat("BGM", BGM_Sound);
        if (SFX_Sound <= -40f) audioMixer.SetFloat("SFX", -80);
        else audioMixer.SetFloat("SFX", SFX_Sound);

        BGM_Slider.maxValue = MASTER_Slider.value;
        SFX_Slider.maxValue = MASTER_Slider.value;
    }
}
