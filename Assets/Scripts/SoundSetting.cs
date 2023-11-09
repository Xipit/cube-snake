
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider Slider_Volume;
    [SerializeField] AudioMixer masterMixer;

    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", 100));
    }

    public void SetVolume(float _value)
    {
        if (_value < 1)
        {
            _value = .001f;
        }
        Refreshslider(_value);
        PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(_value / 100) * 20f);
    }

    public void SetVolumeFromSlider()
    {
        SetVolume(Slider_Volume.value);
    }
    public void Refreshslider(float _value)
    {
        Slider_Volume.value = _value;
    }
}