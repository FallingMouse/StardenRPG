/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardenRPG.StateManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace StardenRPG.Screens
{
    public class ControlsOptionsScreen : MenuScreen
    {
        MenuEntry line1;
        MenuEntry line2;
        MenuEntry line3;
        MenuEntry line4;

        public ControlsOptionsScreen() : base("")
        {
            MenuEntry back = new MenuEntry("Back");

            line1 = new MenuEntry("Move Forward = D             Interact = E");
            line2 = new MenuEntry("Move Backward = A              Attack = P");
            line3 = new MenuEntry("Jump = W               Switch Elements = Q");
            line4 = new MenuEntry("Run = Left Shift             Pause = Back");

            back.Selected += OnCancel;

            MenuEntries.Add(line1);
            MenuEntries.Add(line2);
            MenuEntries.Add(line3);
            MenuEntries.Add(line4);
            MenuEntries.Add(back);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            base.OnCancel(playerIndex);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }
    }
}*/




/*using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardenRPG.Screens;
using StardenRPG.StateManagement;

public class ControlsOptionsScreen : MenuScreen
{

    public ControlsOptionsScreen() : base("")
    {
        MenuEntry back = new MenuEntry("Back");

        back.Selected += OnCancel;

        MenuEntries.Add(back);
    }


    public override void Draw(GameTime gameTime)
    {
        UpdateMenuEntryLocations();

        var graphics = ScreenManager.GraphicsDevice;
        var spriteBatch = ScreenManager.SpriteBatch;
        var font = ScreenManager.Font;

        spriteBatch.Begin();

        var line1Position = new Vector2(graphics.Viewport.Width / 2, 610);
        var line1Origin = font.MeasureString("Move Forward = D             Interact = E") / 2;
        spriteBatch.DrawString(font, "Move Forward = D             Interact = E", line1Position, Color.White, 0, line1Origin, 1.0f, SpriteEffects.None, 0);

        var line2Position = new Vector2(graphics.Viewport.Width / 2, 660);
        var line2Origin = font.MeasureString("Move Backward = A              Attack = P") / 2;
        spriteBatch.DrawString(font, "Move Backward = A              Attack = P", line2Position, Color.White, 0, line2Origin, 1.0f, SpriteEffects.None, 0);

        var line3Position = new Vector2(graphics.Viewport.Width / 2, 710);
        var line3Origin = font.MeasureString("Jump = W               Switch Elements = Q") / 2;
        spriteBatch.DrawString(font, "Jump = W               Switch Elements = Q", line3Position, Color.White, 0, line3Origin, 1.0f, SpriteEffects.None, 0);

        var line4Position = new Vector2(graphics.Viewport.Width / 2, 760);
        var line4Origin = font.MeasureString("Run = Left Shift             Pause = Back") / 2;
        spriteBatch.DrawString(font, "Run = Left Shift             Pause = Back", line4Position, Color.White, 0, line4Origin, 1.0f, SpriteEffects.None, 0);

        for (int i = 0; i < MenuEntries.Count; i++)
        {
            var menuEntry = MenuEntries[i];
            bool isSelected = IsActive && i == _selectedEntry;
            menuEntry.Draw(this, isSelected, gameTime);
        }

        spriteBatch.End();
    }
}*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardenRPG.Screens;
using StardenRPG.StateManagement;

public class ControlsOptionsScreen : MenuScreen
{
    private MenuEntry back;
    private bool fadeOut;

    public ControlsOptionsScreen() : base("")
    {
        back = new MenuEntry("Back");
        back.Selected += OnCancel;
        back.Selected += OnBackSelected;
        MenuEntries.Add(back);
    }

    private void OnBackSelected(object sender, PlayerIndexEventArgs e)
    {
        fadeOut = true;
    }

    public override void Draw(GameTime gameTime)
    {
        UpdateMenuEntryLocations();

        var graphics = ScreenManager.GraphicsDevice;
        var spriteBatch = ScreenManager.SpriteBatch;
        var font = ScreenManager.Font;

        spriteBatch.Begin();

        if (!fadeOut)
        {
            var line1Position = new Vector2(graphics.Viewport.Width / 2, 610);
            var line1Origin = font.MeasureString("Move Forward = D             Interact = E") / 2;
            spriteBatch.DrawString(font, "Move Forward = D             Interact = E", line1Position, Color.White, 0, line1Origin, 1.0f, SpriteEffects.None, 0);

            var line2Position = new Vector2(graphics.Viewport.Width / 2, 660);
            var line2Origin = font.MeasureString("Move Backward = A              Attack = P") / 2;
            spriteBatch.DrawString(font, "Move Backward = A              Attack = P", line2Position, Color.White, 0, line2Origin, 1.0f, SpriteEffects.None, 0);

            var line3Position = new Vector2(graphics.Viewport.Width / 2, 710);
            var line3Origin = font.MeasureString("Jump = W               Switch Elements = Q") / 2;
            spriteBatch.DrawString(font, "Jump = W               Switch Elements = Q", line3Position, Color.White, 0, line3Origin, 1.0f, SpriteEffects.None, 0);

            var line4Position = new Vector2(graphics.Viewport.Width / 2, 760);
            var line4Origin = font.MeasureString("Run = Left Shift             Pause = Back") / 2;
            spriteBatch.DrawString(font, "Run = Left Shift             Pause = Back", line4Position, Color.White, 0, line4Origin, 1.0f, SpriteEffects.None, 0);
        }

        for (int i = 0; i < MenuEntries.Count; i++)
        {
            var menuEntry = MenuEntries[i];
            bool isSelected = IsActive && i == _selectedEntry;
            menuEntry.Draw(this, isSelected, gameTime);
        }

        spriteBatch.End();
    }
}
