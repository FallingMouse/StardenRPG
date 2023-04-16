using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardenRPG.Utilities
{
    public class ParallaxBackground
    {
        private readonly Texture2D[] _layers;
        private readonly float[] _parallaxFactors;
        private readonly Viewport _viewport;

        public int RepeatWidth { get; }

        public ParallaxBackground(Texture2D[] layers, float[] parallaxFactors, Viewport viewport)
        {
            _layers = layers;
            _parallaxFactors = parallaxFactors;
            _viewport = viewport;

            RepeatWidth = layers[0].Width;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Matrix viewMatrix)
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                float parallaxFactor = _parallaxFactors[i];
                Vector2 layerPosition = cameraPosition * parallaxFactor;
                layerPosition.X %= RepeatWidth;

                int numRepeats = (_viewport.Width + RepeatWidth - 1) / RepeatWidth;

                for (int repeat = 0; repeat <= numRepeats; repeat++)
                {
                    Rectangle sourceRectangle = new Rectangle((int)layerPosition.X, (int)layerPosition.Y, _viewport.Width, _viewport.Height);
                    Vector2 layerOrigin = new Vector2(layerPosition.X, layerPosition.Y);
                    Vector2 drawPosition = new Vector2(repeat * RepeatWidth - layerPosition.X, 0);

                    spriteBatch.Draw(_layers[i], drawPosition, sourceRectangle, Color.White, 0, layerOrigin, 1, SpriteEffects.None, 0);

                    // Add this block to handle drawing the next portion of the repeating background
                    if (sourceRectangle.Right >= RepeatWidth)
                    {
                        Rectangle sourceRectangle2 = new Rectangle(0, (int)layerPosition.Y, _viewport.Width - (RepeatWidth - sourceRectangle.Left), _viewport.Height);
                        Vector2 layerOrigin2 = new Vector2(0, layerPosition.Y);
                        spriteBatch.Draw(_layers[i], new Vector2(drawPosition.X + RepeatWidth, 0), sourceRectangle2, Color.White, 0, layerOrigin2, 1, SpriteEffects.None, 0);
                    }
                }
            }
        }
    }

}
