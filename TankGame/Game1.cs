using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TankGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        Cls3DAxis Cls3DAxis;
        ClsPlaneTextureIndexStripVB terrain;
        Tank tank;
        Tank tankAI;
        List<Tank> enemytanks;

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
            tank = new Tank(Content.Load<Model>("tank"), terrain, new Vector3(15, 15, 15), GraphicsDevice, Tank.PlayerMode.PC, 0);
            tankAI = new Tank(Content.Load<Model>("tank"), terrain, new Vector3(90, 90, 90), GraphicsDevice, Tank.PlayerMode.AI, 1);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            tank.Update(Keyboard.GetState(), gameTime, Content, tankAI, camera);
            camera.Update(Keyboard.GetState(), Mouse.GetState(), gameTime, tank);
            tankAI.Update(Keyboard.GetState(), gameTime, Content, tank, camera);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Cls3DAxis.Draw(GraphicsDevice, camera.viewMatrix);
            terrain.Draw(GraphicsDevice, camera);
            tank.Draw(camera, GraphicsDevice);
            tankAI.Draw(camera, GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}