using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_manager : UnitySinleton<audio_manager>
{
    private AudioSource music = null;
    private int max_effects = 10; //当钱允许最大播放10个
    
    //存放我们每隔音效 AudioSource
    private Queue<AudioSource> effects;

    private AudioClip now_music_clip = null;
    private bool now_music_loop = false;

    public override void Awake()
    {
        base.Awake();

        this.music = this.gameObject.AddComponent<AudioSource>();
        
        this.effects = new Queue<AudioSource>();
        for (int i = 0; i < max_effects; i++)
        {
            AudioSource source = this.gameObject.AddComponent<AudioSource>();
            this.effects.Enqueue(source);
        }
    }

    public void play_music(AudioClip clip, bool loop = true)
    {
        if (this.music == null || clip == null || (this.music.clip && this.music.clip.name == clip.name))
        {
            return;
        }

        this.music.clip = clip;
        this.music.loop = loop;
        this.music.volume = 1.0f;
        this.music.Play();

        this.now_music_clip = clip;
        this.now_music_loop = loop;
    }

    public void stop_music()
    {
        if (this.music == null || this.music.clip == null)
            return;
        this.music.Stop();
    }

    public AudioSource play_effect(AudioClip clip)
    {
        AudioSource source = this.effects.Dequeue(); //弹出

        this.effects.Enqueue(source); //放到尾
        return source;
    }

    public void enable_music(bool enable)
    {
        if (this.music == null || this.music.enabled == enable)
            return;

        this.music.enabled = enable;
        if (enable)
        {
            this.music.clip = this.now_music_clip;
            this.music.loop = this.now_music_loop;
            this.music.Play();
        }
    }

    public void enable_effect(bool enabled)
    {
        AudioSource[] effect_set = this.effects.ToArray();

        foreach (AudioSource source in this.effects)
        {
            if(source.enabled == enabled)
                continue;

            if (enabled)
            {
                source.clip = null;
            }
            source.enabled = enabled;
        }
    }
}
