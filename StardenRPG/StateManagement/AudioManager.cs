using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace StardenRPG.StateManagement
{
    public class AudioManager : GameComponent, IAudioManager
    {
        private string _CurrentSongAsset;

        protected float _MasterVolume = 1f;

        protected float _SFXVolume = 1f;

        protected float _MusicVolume = 1f;

        public Song CurrentSong { get; set; }

        public bool loopCurrenSong { get; set; }

        public SoundEffectInstance CurrentSFXInstance { get; set; }

        public bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        public bool IsMusicStopped => MediaPlayer.State == MediaState.Stopped;

        public bool IsMusicPaused => MediaPlayer.State == MediaState.Paused;

        public string CurrentSongAsset => _CurrentSongAsset;

        public MediaState MediaState => MediaPlayer.State;

        public float MasterVolume
        {
            get
            {
                return _MasterVolume;
            }
            set
            {
                float num = MathHelper.Max(0f, MathHelper.Min(1f, value));
                if (_MasterVolume != num)
                {
                    _MasterVolume = num;
                    SFXVolume = _SFXVolume;
                    MusicVolume = _MusicVolume;
                }
            }
        }

        public float SFXVolume
        {
            get
            {
                return _SFXVolume;
            }
            set
            {
                _SFXVolume = MathHelper.Max(0f, MathHelper.Min(1f, value));
                if (CurrentSFXInstance != null)
                {
                    CurrentSFXInstance.Volume = SFXVolume * MasterVolume;
                }
            }
        }

        public float MusicVolume
        {
            get
            {
                return _MusicVolume;
            }
            set
            {
                _MusicVolume = MathHelper.Max(0f, MathHelper.Min(1f, value));
                MediaPlayer.Volume = MusicVolume * MasterVolume;
            }
        }

        public AudioManager(Game game, float masterVolume = 1f, float musicVolume = 1f, float sfxVolume = 1f)
            : base(game)
        {
            SFXVolume = sfxVolume;
            MusicVolume = musicVolume;
            MasterVolume = masterVolume;
            game.Services.AddService((IAudioManager)this);
        }

        public void PlaySong(string songAsset, float volume = 1f, bool loop = true)
        {
            _CurrentSongAsset = songAsset;
            CurrentSong = base.Game.Content.Load<Song>(songAsset);
            MediaPlayer.Volume = volume * MusicVolume * MasterVolume;
            loopCurrenSong = loop;
            PlaySong(CurrentSong);
        }

        public void PlaySong(Song song)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(song);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (CurrentSong != null && loopCurrenSong && IsMusicStopped)
            {
                PlaySong(CurrentSong);
            }
        }

        public void PlaySFX(string sfxAsset, float volume = 1f, AudioListener listener = null, AudioEmitter emitter = null, float pitch = 0f, float pan = 0f)
        {
            SoundEffectInstance soundEffectInstance = base.Game.Content.Load<SoundEffect>(sfxAsset).CreateInstance();
            soundEffectInstance.Volume = volume * SFXVolume * MasterVolume;
            soundEffectInstance.Pitch = pitch;
            soundEffectInstance.Pan = pan;
            if (listener != null && emitter != null)
            {
                soundEffectInstance.Apply3D(listener, emitter);
            }

            soundEffectInstance.Play();
        }

        public void PlaySound(string sfxAsset, float volume = 1f, AudioListener listener = null, AudioEmitter emitter = null, bool loop = false, float pitch = 0f, float pan = 0f)
        {
            CurrentSFXInstance = base.Game.Content.Load<SoundEffect>(sfxAsset).CreateInstance();
            CurrentSFXInstance.IsLooped = loop;
            CurrentSFXInstance.Pan = pan;
            CurrentSFXInstance.Pitch = pitch;
            CurrentSFXInstance.Volume = volume * SFXVolume * MasterVolume;
            if (listener != null && emitter != null)
            {
                CurrentSFXInstance.Apply3D(listener, emitter);
            }

            CurrentSFXInstance.Play();
        }

        public void StopSound()
        {
            if (CurrentSFXInstance.State != SoundState.Stopped)
            {
                CurrentSFXInstance.Stop(immediate: true);
            }
        }

        public void StopMusic()
        {
            if (!IsMusicStopped)
            {
                loopCurrenSong = false;
                MediaPlayer.Stop();
            }
        }

        public void PauseMusic(string audio)
        {
            if (IsMusicPlaying)
            {
                MediaPlayer.Pause();
            }
        }

        public void PauseSound()
        {
            if (CurrentSFXInstance.State == SoundState.Playing)
            {
                CurrentSFXInstance.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (IsMusicPaused)
            {
                MediaPlayer.Resume();
            }
        }

        public void ResumeSound()
        {
            if (CurrentSFXInstance.State == SoundState.Paused)
            {
                CurrentSFXInstance.Play();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsMusicStopped)
            {
                MediaPlayer.Stop();
            }

            base.Dispose(disposing);
        }
    }
}
