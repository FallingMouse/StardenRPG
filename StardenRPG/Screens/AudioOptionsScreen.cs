using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardenRPG.StateManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace StardenRPG.Screens
{
    public class AudioOptionsScreen : MenuScreen
    {
        
        MenuEntry masterVolume;
        MenuEntry musicVolume;
        MenuEntry sfxVolume;

        InputAction sliderUp;
        InputAction sliderDown;

        protected float volumeDelta = .005f;

        public AudioOptionsScreen() : base("")
        {            
            MenuEntry back = new MenuEntry("BACK");

            back.Selected += OnCancel;

            masterVolume = new MenuEntry($"MASTER   VOLUME   ");
            musicVolume = new MenuEntry($"MUSIC   VOLUME   ");
            sfxVolume = new MenuEntry($"SFX   VOLUME   ");

            masterVolume.Selected += SetMasterVolume;

            MenuEntries.Add(back);
            MenuEntries.Add(masterVolume);
            MenuEntries.Add(musicVolume);
            MenuEntries.Add(sfxVolume);
            

            sliderUp = new InputAction(
               new[] { Keys.Right }, false);

            sliderDown = new InputAction(
               new[] { Keys.Left }, false);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            base.OnCancel(playerIndex);

            // save the audio settings.
            ScreenManager.SaveAudioSettings();

        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
            PlayerIndex playerIndex;

            if (sliderDown.Occurred(input, ControllingPlayer, out playerIndex))
            {
                if(_selectedEntry == 1)
                    ScreenManager.audioManager.MasterVolume = MathF.Max(0f, ScreenManager.audioManager.MasterVolume - volumeDelta);
                else if(_selectedEntry == 2)
                    ScreenManager.audioManager.MusicVolume = MathF.Max(0f, ScreenManager.audioManager.MusicVolume - volumeDelta);
                else if(_selectedEntry == 3)
                    ScreenManager.audioManager.SFXVolume = MathF.Max(0f, ScreenManager.audioManager.SFXVolume - volumeDelta);
            }

            if (sliderUp.Occurred(input, ControllingPlayer, out playerIndex))
            {
                if (_selectedEntry == 1)
                    ScreenManager.audioManager.MasterVolume = MathF.Max(0f, ScreenManager.audioManager.MasterVolume + volumeDelta);
                else if (_selectedEntry == 2)
                    ScreenManager.audioManager.MusicVolume = MathF.Max(0f, ScreenManager.audioManager.MusicVolume + volumeDelta);
                else if (_selectedEntry == 3)
                    ScreenManager.audioManager.SFXVolume = MathF.Max(0f, ScreenManager.audioManager.SFXVolume + volumeDelta);
            }
        }

        protected void SetMasterVolume(object sender, PlayerIndexEventArgs args)
        {
            // do nowt..
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            masterVolume.Text = $"MASTER   VOLUME   {(int)(ScreenManager.audioManager.MasterVolume * 100f)}%";
            musicVolume.Text = $"MUSIC   VOLUME   {(int)(ScreenManager.audioManager.MusicVolume * 100f)}%";
            sfxVolume.Text = $"SFX   VOLUME   {(int)(ScreenManager.audioManager.SFXVolume * 100f)}%";

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            
        }
    }
}
