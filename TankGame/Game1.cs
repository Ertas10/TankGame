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
        CollisionManager colManager;
        ProjectileManager projManager;
        List<Tank> enemytanks;
        List<Keys> player1Keys;
        List<Keys> player2Keys;

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
            #region P1Keys
            player1Keys = new List<Keys>();
            player1Keys.Add(Keys.A);
            player1Keys.Add(Keys.D);
            player1Keys.Add(Keys.W);
            player1Keys.Add(Keys.S);
            player1Keys.Add(Keys.Space);
            player1Keys.Add(Keys.Left);
            player1Keys.Add(Keys.Right);
            player1Keys.Add(Keys.Up);
            player1Keys.Add(Keys.Down);
            player1Keys.Add(Keys.F6);
            #endregion
            #region P2Keys
            player2Keys = new List<Keys>();
            player2Keys.Add(Keys.J);
            player2Keys.Add(Keys.L);
            player2Keys.Add(Keys.I);
            player2Keys.Add(Keys.K);
            player2Keys.Add(Keys.Enter);
            player2Keys.Add(Keys.OemPlus);
            player2Keys.Add(Keys.NumPad3);
            player2Keys.Add(Keys.NumPad2);
            player2Keys.Add(Keys.NumPad0);
            player2Keys.Add(Keys.F7);
            #endregion
            Cls3DAxis = new Cls3DAxis(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            terrain = new ClsPlaneTextureIndexStripVB(GraphicsDevice, 0.2f, Content.Load<Texture2D>("terreno"), Content.Load<Texture2D>("textura"));
            projManager = new ProjectileManager(Content.Load<Model>("rain"), terrain);
            camera = new Camera(GraphicsDevice, terrain);
            tank = new Tank(Content.Load<Model>("tank"), terrain, new Vector3(15, 15, 15), GraphicsDevice, Tank.PlayerMode.PC, 0, player1Keys);
            tankAI = new Tank(Content.Load<Model>("tank"), terrain, new Vector3(90, 90, 90), GraphicsDevice, Tank.PlayerMode.AI, 1, player2Keys);
            colManager = new CollisionManager(tank, tankAI);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            tank.Update(Keyboard.GetState(), gameTime, tankAI, camera, projManager);
            tankAI.Update(Keyboard.GetState(), gameTime, tank, camera, projManager);
            camera.Update(Keyboard.GetState(), Mouse.GetState(), gameTime, tank);
            projManager.Update(gameTime, colManager);
            colManager.Collision();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Cls3DAxis.Draw(GraphicsDevice, camera.viewMatrix);
            terrain.Draw(GraphicsDevice, camera);
            tank.Draw(camera, GraphicsDevice);
            tankAI.Draw(camera, GraphicsDevice);
            projManager.Draw(camera, GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}