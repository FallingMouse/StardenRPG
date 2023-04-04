using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace StardenRPG.StateManagement
{
    public interface IAudioManager
    {
        Song CurrentSong { get; set; }

        bool loopCurrenSong { get; set; }

        SoundEffectInstance CurrentSFXInstance { get; set; }

        bool IsMusicPlaying { get; }

        bool IsMusicStopped { get; }

        bool IsMusicPaused { get; }

        MediaState MediaState { get; }

        float MasterVolume { get; set; }

        float SFXVolume { get; set; }

        float MusicVolume { get; set; }

        string CurrentSongAsset { get; }

        void PlaySong(string songAsset, float volume = 1f, bool loop = true);

        void PlaySong(Song song);

        void PlaySFX(string sfxAsset, float volume = 1f, AudioListener listener = null, AudioEmitter emitter = null, float pitch = 0f, float pan = 0f);

        void PlaySound(string sfxAsset, float volume = 1f, AudioListener listener = null, AudioEmitter emitter = null, bool loop = false, float pitch = 0f, float pan = 0f);

        void StopSound();

        void StopMusic();

        void PauseMusic(string audio);

        void PauseSound();

        void ResumeMusic();

        void ResumeSound();
    }
}
