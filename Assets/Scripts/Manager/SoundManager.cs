// Author : Renaud THIERRY
// Created : 28-01-2016

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

	// Singleton
	// 

	static private SoundManager instance;
	static public SoundManager Instance{
		get{ return SoundManager.instance;}

	}

    // Properties
    //
    
    public bool SoundOn = true;
    public bool MusicOn = true;
    private bool musicPlaying = false;

    [Header("Mixer")]
    public AudioMixer MixerMaster;

    //
    [Header("Sound Lists")]
    public List<AudioClip> ClickMenu = new List<AudioClip>();
    public List<AudioClip> PopupOpen = new List<AudioClip>();
    public List<AudioClip> PopupClose = new List<AudioClip>();

    public List<AudioClip> LaunchFromPlanet = new List<AudioClip>();
    public List<AudioClip> ValidationPlanet = new List<AudioClip>();

    public List<AudioClip> DeathPlayer = new List<AudioClip>();
    public List<AudioClip> DeathGhost = new List<AudioClip>();
    [Tooltip("how far can we hear the GhostDeath")]
    public float GhostDeathRange = 5f;

    public List<AudioClip> RocketSound = new List<AudioClip>();


    [Header("Jingles")]
    public List<AudioClip> WinGame = new List<AudioClip>();
    public List<AudioClip> LooseGame = new List<AudioClip>();

    public List<AudioClip> BonusUsed = new List<AudioClip>();
    public List<AudioClip> BuyBack = new List<AudioClip>();

    [Tooltip("The music will lower to this volume while a jingle is playing")]
    [Range(0f, 1f)]
    public float MusicVolumeWhileJinglePlay;

    [Header("Music")]
    public AudioClip MenuMusic;
    public AudioClip InGameMusic;


    // AudioSource
    [Header("Audio Source")]
    public AudioSource PlayerSource;
    public AudioSource GhostSource;
    public AudioSource WorldSource;
    public AudioSource JingleSource;

    public AudioSource MusicSource;


    private float defaultMusicVolume;
    private AudioListener audioListener;

    private AudioClip nextMusicToPlay;

    private bool isInit = false;

    public enum FadeState { none, FadeIn, FadeOut };
    private FadeState currentFadeState = FadeState.none;

    private float currentFadeTime = 0f;
    private float fadeTime = 1;

    private float hintPopTimer = -1f;

    // Methods
    //
    
    void Awake()
    {
		// Singleton logic
		if(SoundManager.instance == null){

			SoundManager.instance = this;
			GameObject.DontDestroyOnLoad(this.gameObject);

		}
		else{
			GameObject.Destroy(this.gameObject);
			return;
		}

        SoundManager.instance = this;

        this.InitSoundManager();

    }

    // Use this for initialization
    public void InitSoundManager()
    {

        // If the sound manager hasn't been init yet
        if (!this.isInit)
        {

            this.searchAudioListener();

            // Add the Audio source to this god object

            this.PlayerSource = this.gameObject.AddComponent<AudioSource>();
            this.WorldSource = this.gameObject.AddComponent<AudioSource>();
            this.GhostSource = this.gameObject.AddComponent<AudioSource>();
            this.JingleSource = this.gameObject.AddComponent<AudioSource>();

            this.MusicSource = this.gameObject.AddComponent<AudioSource>();

            this.MusicSource.loop = true;

            this.defaultMusicVolume = 1f;


            // Init is done
            this.isInit = true;

        }
    }

    private void searchAudioListener()
    {

        AudioListener camAL = Camera.main.GetComponent<AudioListener>();

        // If we found an AudioListener
        if (camAL != null)
        {
            this.audioListener = camAL;
        }
        // Search in the wole scene, more costly
        else {
            this.audioListener = GameObject.FindObjectOfType<AudioListener>();

        }
    }

    public void Update()
    {

        // If the music is playing
        if (this.MusicOn)
        {

            // Re up the volume if a jingle sound stopped playing or an ad is finished
            if (this.musicPlaying && this.MusicSource.volume != this.defaultMusicVolume && !this.JingleSource.isPlaying)
            {

                this.MusicSource.volume = this.defaultMusicVolume;

            }
        }

        // Music Fade In/Out
        if (this.currentFadeState != FadeState.none)
        {

            this.currentFadeTime += Time.deltaTime;

            // Fade in
            if (this.currentFadeState == FadeState.FadeIn)
            {

                this.MusicSource.volume = Mathf.Clamp01(this.currentFadeTime / this.fadeTime);


            }
            // Fade out
            else if (this.currentFadeState == FadeState.FadeOut)
            {
                this.MusicSource.volume = Mathf.Clamp01((this.fadeTime - this.currentFadeTime) / this.fadeTime);
            }


            // If the fade in operation have ended, or where overwritten
            if (this.MusicSource.volume == 1f)
            {
                Debug.Log("SoundManager.Update - End Fade In Music");

                this.currentFadeState = FadeState.none;
                this.currentFadeTime = 0f;
            }

        }

        if (this.hintPopTimer > 0)
        {
            // Diminue the timer during no hintPop sound should be played
            this.hintPopTimer -= Time.deltaTime;
        }

    }

    public void TransitionPause()
    {

        this.MixerMaster.FindSnapshot("Music Pause").TransitionTo(0.1f);

    }

    public void TransitionNormal()
    {

        this.MixerMaster.FindSnapshot("Music Normal").TransitionTo(0.1f);

    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (this.audioListener != null)
        {
            this.audioListener.enabled = !pauseStatus;
        }
    }
	// TODO Deprecated
	public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Find this scene audio listener
        this.searchAudioListener();
    }

    #region Music
    //
    // Music
    //

    public void StopMusic()
    {

        // If a music is already playing, we stop it
        if (this.MusicSource.isPlaying)
        {

            this.MusicSource.Stop();
            this.MusicSource.time = 0f;
            this.MusicSource.clip = null;
        }

        this.musicPlaying = false;

    }

    private void LowerMusicWhileJinglePlay()
    {

        if (this.musicPlaying)
        {
            // Debug.Log("SoundManager.LowerMusicWhileJinglePlay - Lowered music");
            this.MusicSource.volume = this.MusicVolumeWhileJinglePlay;
            // this.MusicSource.ignoreListenerVolume = false;
        }

    }

    public void MuteMusicDuringAds()
    {

        if (this.musicPlaying)
        {
            // Debug.Log("SoundManager.MuteMusicDuringAds - Lowered music");
            this.MusicSource.volume = 0;
            // this.MusicSource.ignoreListenerVolume = false;
        }

    }

    private void PlayNextMusic()
    {

        this.MusicSource.volume = 1f;

        this.MusicSource.clip = this.nextMusicToPlay;
        this.MusicSource.Play();
    }

    public void ChooseNextMusicLevel(int lvl)
    {
        // RTBK implement

    }

    public void PlayMenuMusic()
    {
        this.StopRocketSound();

        this.musicPlaying = false;

        // If we should play it
        if (this.MusicOn && this.MenuMusic != null && !(this.MusicSource.isPlaying && this.MusicSource.clip.Equals(this.MenuMusic)))
        {

            Debug.Log("SoundManager.PlayMenuMusic - Let's play the menu music");

            this.MusicSource.Stop();

            this.FadeInMusic(0.5f);


            this.MusicSource.clip = this.MenuMusic;

            this.MusicSource.Play((ulong)0.1);

        }

    }

    public void PlayInGameMusic()
    {
        this.StopRocketSound();

        this.musicPlaying = false;

        // If we should play it
        if (this.MusicOn && this.InGameMusic != null && !(this.MusicSource.isPlaying && this.MusicSource.clip.Equals(this.InGameMusic)))
        {

            Debug.Log("SoundManager.PlayInGameMusic - Let's play the in game music");

            this.MusicSource.Stop();

            this.FadeInMusic(0.5f);


            this.MusicSource.clip = this.InGameMusic;

            this.MusicSource.Play((ulong)0.1);

        }

    }

    public void FadeInMusic(float timer)
    {

        this.currentFadeTime = 0f;
        this.fadeTime = timer;

        this.currentFadeState = FadeState.FadeIn;

    }
    #endregion



    #region PlayMethod
    // -_-_-_-_-_-_-_-
    // Play methods
    //

    /// <summary>
    /// Play one click menu sound, randomly selected from the list
    /// </summary>
    public void PlayClickMenu()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.ClickMenu.Count > 0 && this.audioListener != null)
        {

            //Debug.Log("SoundManager.PlayHeroHit - Hero hit sound");

            AudioClip ac = this.ClickMenu[Random.Range(0, this.ClickMenu.Count)];
            this.GhostSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one popup open sound, randomly selected from the list
    /// </summary>
    public void PlayPopupOpen()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.PopupOpen.Count > 0 && this.audioListener != null)
        {

            //Debug.Log("SoundManager.PlayHeroHit - Hero hit sound");

            AudioClip ac = this.PopupOpen[Random.Range(0, this.PopupOpen.Count)];
            this.GhostSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one popup close sound, randomly selected from the list
    /// </summary>
    public void PlayPopupClose()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.PopupClose.Count > 0 && this.audioListener != null)
        {

            //Debug.Log("SoundManager.PlayHeroHit - Hero hit sound");

            AudioClip ac = this.PopupClose[Random.Range(0, this.PopupClose.Count)];
            this.GhostSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one launch from planet sound, randomly selected from the list
    /// </summary>
    public void PlayLaunchFromPlanet()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.LaunchFromPlanet.Count > 0 && this.audioListener != null)
        {            

            AudioClip ac = this.LaunchFromPlanet[Random.Range(0, this.LaunchFromPlanet.Count)];
            this.WorldSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one validation planet sound, randomly selected from the list
    /// </summary>
    public void PlayValidationPlanet()
    {
        this.StopRocketSound();

        // If the hero can play a sound
        if (this.SoundOn && this.ValidationPlanet.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.ValidationPlanet[Random.Range(0, this.ValidationPlanet.Count)];
            this.WorldSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one death player sound, randomly selected from the list
    /// </summary>
    public void PlayDeathPlayer()
    {

        this.StopRocketSound();

        // If the hero can play a sound
        if (this.SoundOn && this.DeathPlayer.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.DeathPlayer[Random.Range(0, this.DeathPlayer.Count)];
            this.PlayerSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one death ghost sound, randomly selected from the list
    /// </summary>
    public void PlayDeathGhost()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.DeathGhost.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.DeathGhost[Random.Range(0, this.DeathGhost.Count)];
            this.GhostSource.PlayOneShot(ac);

        }

    }

    /// <summary>
    /// Play one rocket sound, randomly selected from the list. Will loop until the method StopRocketSound is called
    /// </summary>
    public void StartRocketSound()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.RocketSound.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.RocketSound[Random.Range(0, this.RocketSound.Count)];

            this.PlayerSource.clip = ac;
            this.PlayerSource.loop = true;

            this.PlayerSource.Play();

        }

    }

    public void StopRocketSound() {
                
        this.PlayerSource.Stop();

    }


    #endregion

    #region Jingles

    /// <summary>
    /// Play one Win game jingle, randomly selected from the list
    /// </summary>
    public void PlayWinGame()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.WinGame.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.WinGame[Random.Range(0, this.WinGame.Count)];
            this.JingleSource.clip = ac;
            this.JingleSource.Play();
            this.LowerMusicWhileJinglePlay();

        }

    }

    /// <summary>
    /// Play one Loose game jingle, randomly selected from the list
    /// </summary>
    public void PlayLooseGame()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.LooseGame.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.LooseGame[Random.Range(0, this.LooseGame.Count)];
            this.JingleSource.clip = ac;
            this.JingleSource.Play();
            this.LowerMusicWhileJinglePlay();

        }

    }

    /// <summary>
    /// Play one bonus used jingle, randomly selected from the list
    /// </summary>
    public void PlayBonusUsed()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.BonusUsed.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.BonusUsed[Random.Range(0, this.BonusUsed.Count)];
            this.JingleSource.clip = ac;
            this.JingleSource.Play();
            this.LowerMusicWhileJinglePlay();

        }

    }

    /// <summary>
    /// Play one buyback jingle, randomly selected from the list
    /// </summary>
    public void PlayBuyBack()
    {

        // If the hero can play a sound
        if (this.SoundOn && this.BuyBack.Count > 0 && this.audioListener != null)
        {

            AudioClip ac = this.BuyBack[Random.Range(0, this.BuyBack.Count)];
            this.JingleSource.clip = ac;
            this.JingleSource.Play();
            this.LowerMusicWhileJinglePlay();

        }

    }

    #endregion


}