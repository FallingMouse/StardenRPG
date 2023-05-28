using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardenRPG.Screens;
using StardenRPG.StateManagement;
using Microsoft.Xna.Framework.Input;

public class ControlsOptionsScreen : MenuScreen
{
    private MenuEntry back;
    private bool fadeOut;

    public ControlsOptionsScreen() : base("")
    {
        back = new MenuEntry("BACK");
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
            var line1Position = new Vector2(graphics.Viewport.Width / 2, 640);
            var line1Origin = font.MeasureString("Move   Forward   =   D                     Interact   =   F") / 2;
            spriteBatch.DrawString(font, "Move   Forward   =   D                     Interact   =   F", line1Position, Color.White, 0, line1Origin, 1.0f, SpriteEffects.None, 0);

            var line2Position = new Vector2(graphics.Viewport.Width / 2, 720);
            var line2Origin = font.MeasureString("Move   Backward   =   A              Attack   =   P") / 2;
            spriteBatch.DrawString(font, "Move   Backward   =   A              Attack   =   P", line2Position, Color.White, 0, line2Origin, 1.0f, SpriteEffects.None, 0);

            var line3Position = new Vector2(graphics.Viewport.Width / 2, 800);
            var line3Origin = font.MeasureString("Pause   =   Back   SPACE                                                                     ") / 2;
            spriteBatch.DrawString(font, "Pause   =   Back   SPACE                                                                     ", line3Position, Color.White, 0, line3Origin, 1.0f, SpriteEffects.None, 0);
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
