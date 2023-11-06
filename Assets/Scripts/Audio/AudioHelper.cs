using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelper
{
    // src: https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    public static IEnumerator StartFade(AudioSource audioSource, float durationMS, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < durationMS)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / durationMS);
            yield return null;
        }
        yield break;
    }
}
