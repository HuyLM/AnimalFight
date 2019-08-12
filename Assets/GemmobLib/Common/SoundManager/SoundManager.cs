using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gemmob.Common.Data;

public partial class SoundManager : SingletonBindAlive<SoundManager> {
    [Header("==== Audio Source ====")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource soundEffect;

    [Header("==== Audio Clip ====")]
    [SerializeField] private AudioClip clickEffect;

    private List<AudioSource> loopAudios;

    private bool backgroundPausing;
    
    public enum PlaySoundType { None, Override, Duplicate }
    
    protected override void OnAwake() {
    }
    
    #region PrefData
    public bool BackgroundMusicEnable {
        get { return PersitenData.GetBool("BackgroundMusicEnable", true) && BackgroundVolume > 0; }
        set {
            bool current = PersitenData.GetBool("BackgroundMusicEnable", true);
            if (current == value) return;
            PersitenData.SetBool("BackgroundMusicEnable", value);

            if (value) PlayBackgroundMusic();
            else PauseBackgroundMusic();
        }
    }

    public bool SoundEffectEnable {
        get { return PersitenData.GetBool("SoundEffectEnable", true) && SoundEffectVolume > 0; }
        set { PersitenData.SetBool("SoundEffectEnable", value); }
    }

    public float BackgroundVolume {
        get { return PersitenData.GetFloat("BackgroundMusicVolume", 1); }
        set {
            float v = Mathf.Clamp01(value);
            PersitenData.SetFloat("BackgroundMusicVolume", v);
            if (backgroundMusic) backgroundMusic.volume = v;
        }
    }

    public float SoundEffectVolume {
        get { return PersitenData.GetFloat("SoundEffectVolume", 1); }
        set {
            float v = Mathf.Clamp01(value);
            PersitenData.SetFloat("SoundEffectVolume", v);
            if (soundEffect) soundEffect.volume = v;
        }
    }
    #endregion

    #region Background Music
    public void PlayBackgroundMusic(bool resume = false, bool fadein = false, float fadeDuration = 1) {
        if (backgroundMusic == null || !BackgroundMusicEnable || backgroundMusic.isPlaying) return;

        backgroundPausing = false;
        if (resume) backgroundMusic.UnPause();
        else backgroundMusic.Play();

        if (!fadein) backgroundMusic.volume = BackgroundVolume;
        else FadeInBackgroundMusic(fadeDuration);
    }

    public void StopBackgroundMusic(bool fadeout = false, float fadeDuration = 1) {
        if (backgroundMusic == null || !backgroundMusic.isPlaying) return;

        if (!fadeout) { backgroundMusic.Stop(); }
        else FadeOutBackgroundMusic(fadeDuration, () => { backgroundMusic.Stop(); });
    }

    public void PauseBackgroundMusic(bool fadeout = false, float fadeDuration = 1) {
        if (backgroundMusic == null || !backgroundMusic.isPlaying) return;

        backgroundPausing = true;
        if (!fadeout) {
            backgroundMusic.Pause();
        }
        else FadeOutBackgroundMusic(fadeDuration, () => {
            backgroundMusic.Pause();
        });
    }
    #endregion

    #region Other Loop audio
    public AudioSource Loop(AudioClip clip) {
        var source = (Instantiate(backgroundMusic.gameObject) as GameObject).GetComponent<AudioSource>();
        source.volume = BackgroundVolume;
        source.transform.SetParent(transform);
        source.clip = clip;
        source.Play();

        if (loopAudios != null) loopAudios.Add(source);
        else {
            loopAudios = new List<AudioSource>();
            loopAudios.Add(source);
        }

        return source;
    }

    public void StopLoop(AudioSource source) {
        if (source == null) return;
        source.Stop();
        loopAudios.Remove(source);
        Destroy(source.gameObject);
    }
    #endregion

    #region Sound Effect
    public void PlaySoundEffect(AudioClip audio, PlaySoundType playSoundType = PlaySoundType.Override) {
        if (soundEffect == null || !SoundEffectEnable) return;
        if (soundEffect.isPlaying) {
            if (playSoundType == PlaySoundType.Override) soundEffect.Stop();
            else if (playSoundType == PlaySoundType.Duplicate) {
                var dup = soundEffect.Spawn().GetComponent<AudioSource>();
                if (dup) dup.PlayOneShot(audio, SoundEffectVolume);
                return;
            }
        }
        soundEffect.PlayOneShot(audio, SoundEffectVolume);
    }

    public virtual void PlayClickEffect() {
        PlaySoundEffect(clickEffect);
    }

    #endregion

    #region Volume Fade Effect
    private void FadeInBackgroundMusic(float duration = 1, System.Action callback = null) {
        FadeInSound(backgroundMusic, BackgroundVolume, duration, callback);
    }

    private void FadeOutBackgroundMusic(float duration = 1, System.Action callback = null) {
        FadeOutSound(backgroundMusic, duration, callback);
    }

    private void FadeInSound(AudioSource audio, float toVolume, float duration = 1, System.Action callback = null) {
        StartCoroutine(IEFadeSound(audio, 0, toVolume, duration, callback));
    }

    private void FadeOutSound(AudioSource audio, float duration = 1, System.Action callback = null) {
        StartCoroutine(IEFadeSound(audio, audio.volume, 0, duration, callback));
    }

    IEnumerator IEFadeSound(AudioSource audio, float froVolume, float toVolume, float duration = 1, System.Action callback = null) {
        if (audio == null) {
            Debug.LogWarning(string.Format("[SoundManager] Fade Sound Fall! Null audio {0}", audio.name));
            yield break;
        }
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(froVolume, toVolume, t / duration);
            yield return null;
        }
        audio.volume = toVolume;
        if (callback != null) callback.Invoke();
    }
    #endregion
}