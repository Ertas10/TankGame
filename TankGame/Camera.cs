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
        public Matrix projection;
        float yaw;
        float pitch;
        GraphicsDevice graphicsDevice;
        float escalaRadianosPorPixel;
        ClsPlaneTextureIndexStripVB terreno;
        float cameraSpeed = 5;

        public Camera(GraphicsDevice graphicsDevice, ClsPlaneTextureIndexStripVB terreno){
            float aspectRatio = (float)graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.2f, 1000.0f);
            pos = new Vector3(1, 120, 2);
            target = Vector3.UnitZ;
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

            if(keyboard.IsKeyDown(Keys.NumPad8)){                                               //
                pos = pos + dir * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
            }                                                                                   //
            if(keyboard.IsKeyDown(Keys.NumPad5)){                                               //
                pos = pos - dir * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
            }                                                                                   // Controlo de movimento da camara
            if(keyboard.IsKeyDown(Keys.NumPad4)){                                               //
                pos = pos - right * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; //
            }                                                                                   //
            if(keyboard.IsKeyDown(Keys.NumPad6)){                                               //
                pos = pos + right * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; //
            }

            if (pos.X < terreno.vertices[0].Position.X)                                         //
                pos.X = terreno.vertices[0].Position.X;                                         //
            if (pos.Z < terreno.vertices[0].Position.Z)                                         //
                pos.Z = terreno.vertices[0].Position.Z;                                         // Mantém a camara dentro dos limites do terreno
            if (pos.X > terreno.vertices[terreno.vertices.Length - 1].Position.X)               //
                pos.X = terreno.vertices[terreno.vertices.Length - 1].Position.X - 0.0001f;           //
            if (pos.Z > terreno.vertices[terreno.vertices.Length - 1].Position.Z)               //
                pos.Z = terreno.vertices[terreno.vertices.Length - 1].Position.Z - 0.0001f;           //

            Vector3[] vectors = terreno.GetVerticesFromXZ((int)pos.X, (int)pos.Z);              // Vai buscar os vetores que circulam a camara

            float YA = vectors[0].Y;                                                            //
            float YB = vectors[1].Y;                                                            // Posição Y de cada vetor
            float YC = vectors[2].Y;                                                            //
            float YD = vectors[3].Y;                                                            //


            float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);          // Interpolação do Y entre A e B
            float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);          // Interpolação do Y entre C e D

            pos.Y =  ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD) + 2f;    // Interpolação final para encontrar novo Y da camara, adicionando 2 à altura para não haver clipping
            
            target = pos + dir;                                                                 // Atualiza o target

            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);                          // Atualiza a viewMatrix

            Mouse.SetPosition(centroX, centroY);                                                // Coloca o cursor no centro do ecrã
        }


    }
}
