using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardenRPG.Utilities
{
    public class ParallaxBackground
    {
        private readonly Texture2D[] _layers;
        private readonly float[] _parallaxFactors;
        private readonly Viewport _viewport;

        public ParallaxBackground(Texture2D[] layers, float[] parallaxFactors, Viewport viewport)
        {
            _layers = layers;
            _parallaxFactors = parallaxFactors;
            _viewport = viewport;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Matrix transformationMatrix)
        {

            for (int i = 0; i < _layers.Length; i++)
            {
                float parallaxFactor = _parallaxFactors[i];
                Vector2 layerPosition = cameraPosition * parallaxFactor;
                layerPosition.X = layerPosition.X % _layers[i].Width;

                // identify if they are being drawn correctly but are being obscured by other layers.
                //Color semiTransparentWhite = new Color(255, 255, 255, 128); // 50% transparent white
                Color semiTransparentWhite = Color.White;

                spriteBatch.Draw(_layers[i], _viewport.Bounds, new Rectangle((int)layerPosition.X, 0, _layers[i].Width, _layers[i].Height), semiTransparentWhite, 0, Vector2.Zero, SpriteEffects.None, 0);

                if (layerPosition.X > 0)
                {
                    spriteBatch.Draw(_layers[i], new Rectangle((int)(_viewport.Width - layerPosition.X), 0, _layers[i].Width, _layers[i].Height), new Rectangle(0, 0, _layers[i].Width - (int)layerPosition.X, _layers[i].Height), semiTransparentWhite, 0, Vector2.Zero, SpriteEffects.None, 0);
                }
            }
        }

    }

}
