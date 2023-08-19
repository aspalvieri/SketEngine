using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace Sket.Graphics
{
    public sealed class Sprites : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private SpriteBatch sprites;
        private BasicEffect effect;

        public Sprites(Game game)
        {
            if (game is null)
                throw new ArgumentNullException("game");

            this.game = game;
            isDisposed = false;
            sprites = new SpriteBatch(this.game.GraphicsDevice);

            effect = new BasicEffect(this.game.GraphicsDevice);
            effect.FogEnabled = false;
            effect.TextureEnabled = true;
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.Projection = Matrix.Identity;
            effect.View = Matrix.Identity;
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            effect?.Dispose();
            sprites?.Dispose();
            isDisposed = true;
        }

        public void Begin(Camera camera, bool isTextureFilteringEnabled)
        {
            SamplerState sampler = SamplerState.PointClamp;
            if (isTextureFilteringEnabled)
                sampler = SamplerState.LinearClamp;

            if (camera is null) {
                Viewport viewport = game.GraphicsDevice.Viewport;
                effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0f, 1f);
                effect.View = Matrix.Identity;
            }
            else {
                camera.UpdateMatrices();
                effect.View = camera.View;
                effect.Projection = camera.Projection;
            }

            sprites.Begin(blendState: BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone, effect: effect);
        }

        public void End()
        {
            sprites.End();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            //Texture, Position, sourceRectangle, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth
            sprites.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, Vector2 scale, Color color)
		{
			sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
		}
        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color)
        {
            sprites.Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
		public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle destinationRectangle, float rotation, Vector2 origin, Color color)
		{
			sprites.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteEffects.None, 0f);
		}
	}
}
