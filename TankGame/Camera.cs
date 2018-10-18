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
        ClsPlaneTextureIndexStripVB terreno;

        public Camera(GraphicsDevice graphicsDevice, ClsPlaneTextureIndexStripVB terreno){
            pos = new Vector3(1, 120, 2);
            target = -Vector3.UnitZ;
            yaw = 0;
            pitch = 0;
            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);
            this.graphicsDevice = graphicsDevice;
            escalaRadianosPorPixel = (float)Math.PI / 1000f;
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
            pitch = MathHelper.Clamp(pitch, -1, 1);
            Matrix rotacao = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotacao);

            Vector3 right = Vector3.Cross(dir, Vector3.Up);
            float speed = 5;
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

            if (pos.X < terreno.vertices[0].Position.X)
                pos.X = terreno.vertices[0].Position.X;
            if (pos.Z < terreno.vertices[0].Position.Z)
                pos.Z = terreno.vertices[0].Position.Z;
            if (pos.X > terreno.vertices[terreno.vertices.Length - 1].Position.X)
                pos.X = terreno.vertices[terreno.vertices.Length - 1].Position.X - 1;
            if (pos.Z > terreno.vertices[terreno.vertices.Length - 1].Position.Z)
                pos.Z = terreno.vertices[terreno.vertices.Length - 1].Position.Z - 1;

            Vector3[] vectors = terreno.GetVerticesFromXZ((int)pos.X, (int)pos.Z);

            float YA = vectors[0].Y;
            float YB = vectors[1].Y;
            float YC = vectors[2].Y;
            float YD = vectors[3].Y;


            float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);
            float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);

            pos.Y =  ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD) + 2f;
            Debug.Print("AX: " + vectors[0].X + "\nBX: " + vectors[1].X + "\nPosX: " + pos.X);
            Debug.Print("AZ: " + vectors[0].Z + "\nBZ: " + vectors[1].Z + "\nPosZ: " + pos.Z);            //Debug.Print("AZ: " + vectors[0].Z + "\nBZ: " + vectors[1].Z);
            //Debug.Print("YA: " + YA + ";YB: " + YB + ";YC: " + YC + ";YD: " + YD + ";YAB: " + YAB + ";YCD: " + YCD + ";pos.Y: " + pos.Y + ";X: " + pos.X + ";Z: " + pos.Z);
            target = pos + dir;

            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);

            Mouse.SetPosition(centroX, centroY);
        }


    }
}
