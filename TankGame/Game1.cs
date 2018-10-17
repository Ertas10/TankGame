using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        Cls3DAxis Cls3DAxis;
        ClsPlaneTextureIndexStripVB terrain;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Cls3DAxis = new Cls3DAxis(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            terrain = new ClsPlaneTextureIndexStripVB(GraphicsDevice, 0.2f, Content.Load<Texture2D>("terreno"), Content.Load<Texture2D>("textura"));
            camera = new Camera(GraphicsDevice, terrain);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            camera.Update(Keyboard.GetState(), Mouse.GetState(), gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Cls3DAxis.Draw(GraphicsDevice, camera.viewMatrix);
            terrain.Draw(GraphicsDevice, camera.viewMatrix);
            base.Draw(gameTime);
        }
    }
}