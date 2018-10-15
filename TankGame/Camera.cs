using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Camera
    {
        Vector3 pos;
        Vector3 target;
        public Matrix viewMatrix;
        float yaw;
        float pitch;
        GraphicsDevice graphicsDevice;
        float escalaRadianosPorPixel;
        Terrain terreno;

        public Camera(GraphicsDevice graphicsDevice, Terrain terreno){
            pos = new Vector3(1, 1, 2);
            target = -Vector3.UnitZ;
            yaw = 0;
            pitch = 0;
            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);
            this.graphicsDevice = graphicsDevice;
            escalaRadianosPorPixel = (float)Math.PI / 500f;
            Mouse.SetPosition(this.graphicsDevice.Viewport.Width / 2, this.graphicsDevice.Viewport.Height / 2);
            this.terreno = terreno;
        }

        public void Update(KeyboardState keyboard, MouseState mouse, GameTime gameTime){

            int centroX = graphicsDevice.Viewport.Width / 2;
            int centroY = graphicsDevice.Viewport.Height / 2;

            int deltaX = mouse.X - centroX;
            int deltaY = mouse.Y - centroY;
            yaw = yaw - deltaX * escalaRadianosPorPixel;
            pitch = pitch - deltaY * escalaRadianosPorPixel;

            Matrix rotacao = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotacao);

            Vector3 right = Vector3.Cross(dir, Vector3.Up);
            float speed = 1;
            if(keyboard.IsKeyDown(Keys.W)){
                pos = pos + dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(keyboard.IsKeyDown(Keys.S)){
                pos = pos - dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(keyboard.IsKeyDown(Keys.A)){
                pos = pos - right * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(keyboard.IsKeyDown(Keys.D)){
                pos = pos + right * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            target = pos + dir;

            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);

            Debug.Print("PosX: " + pos.X + "; PosY: " + pos.Y + "\n");

            Mouse.SetPosition(centroX, centroY);
        }


    }
}
