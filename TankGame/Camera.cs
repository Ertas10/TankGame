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
        public enum PlayerMode
        {
            CameraSurfaceFollow,
            CameraFree,
            CameraTank

        }
        PlayerMode mode;
        public Vector3 pos;
        Vector3 target;
        public Matrix viewMatrix;
        public Matrix projection;
        float yaw;
        float pitch;
        GraphicsDevice graphicsDevice;
        float escalaRadianosPorPixel;
        ClsPlaneTextureIndexStripVB terreno;
        float cameraSpeed = 5;
        public BasicEffect effect;
        Matrix rotacao;
        bool teste;
        public Camera(GraphicsDevice graphicsDevice, ClsPlaneTextureIndexStripVB terreno){
            teste = false;
            float aspectRatio = (float)graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.2f, 1000.0f);
            pos = new Vector3(15, 10, 10);
            target = Vector3.UnitZ;
            yaw = 10;
            pitch = 0;
            viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);
            this.graphicsDevice = graphicsDevice;
            escalaRadianosPorPixel = (float)Math.PI / 1000f;
            Mouse.SetPosition(this.graphicsDevice.Viewport.Width / 2, this.graphicsDevice.Viewport.Height / 2);
            this.terreno = terreno;
            effect = new BasicEffect(graphicsDevice);
            effect.EnableDefaultLighting();
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -0.5f, 0));
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.VertexColorEnabled = false;
        }

        public void Update(KeyboardState keyboard, MouseState mouse, GameTime gameTime, Tank tank1){
            if (keyboard.IsKeyDown(Keys.F3)){
                mode = PlayerMode.CameraSurfaceFollow;
                Mouse.SetPosition(this.graphicsDevice.Viewport.Width / 2, this.graphicsDevice.Viewport.Height / 2);
            }
            if (keyboard.IsKeyDown(Keys.F2)){
                mode = PlayerMode.CameraFree;
                Mouse.SetPosition(this.graphicsDevice.Viewport.Width / 2, this.graphicsDevice.Viewport.Height / 2);
            }
            if (keyboard.IsKeyDown(Keys.F1)) mode = PlayerMode.CameraTank;
            if (mode == PlayerMode.CameraSurfaceFollow)
            {
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

                if (keyboard.IsKeyDown(Keys.NumPad8))                                               //
                {                                                                                   //
                    pos = pos + dir * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
                }                                                                                   //
                if (keyboard.IsKeyDown(Keys.NumPad5))                                               //
                {                                                                                   //
                    pos = pos - dir * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
                }                                                                                   // Controlo de movimento da camara
                if (keyboard.IsKeyDown(Keys.NumPad4))                                               //
                {                                                                                   //
                    pos = pos - right * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; //
                }                                                                                   //
                if (keyboard.IsKeyDown(Keys.NumPad6))
                {                                                                                   //
                    pos = pos + right * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; //
                }
                if (pos.X < terreno.vertices[0].Position.X)                                         //
                    pos.X = terreno.vertices[0].Position.X;                                         //
                if (pos.Z < terreno.vertices[0].Position.Z)                                         //
                    pos.Z = terreno.vertices[0].Position.Z;                                         // Mantém a camara dentro dos limites do terreno
                if (pos.X > terreno.vertices[terreno.vertices.Length - 1].Position.X)               //
                    pos.X = terreno.vertices[terreno.vertices.Length - 1].Position.X - 0.0001f;     //
                if (pos.Z > terreno.vertices[terreno.vertices.Length - 1].Position.Z)               //
                    pos.Z = terreno.vertices[terreno.vertices.Length - 1].Position.Z - 0.0001f;     //

                Vector3[] vectors = terreno.GetVerticesFromXZ((int)pos.X, (int)pos.Z);              // Vai buscar os vetores que circulam a camara
                float YA = vectors[0].Y;                                                            //
                float YB = vectors[1].Y;                                                            // Posição Y de cada vetor
                float YC = vectors[2].Y;                                                            //
                float YD = vectors[3].Y;                                                            //
                float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);          // Interpolação do Y entre A e B
                float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);          // Interpolação do Y entre C e D

                pos.Y = ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD) + 2f;     // Interpolação final para encontrar novo Y da camara, adicionando 2 à altura para não haver clipping
                target = pos + dir;                                                                 // Atualiza o target
                viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);                          // Atualiza a viewMatrix

                Mouse.SetPosition(centroX, centroY);                                                // Coloca o cursor no centro do ecrã
            }
            if (mode == PlayerMode.CameraFree)
            {

                //primeira coisa a ver é quanto o rato andou , mas apenas sabemos a posição dele, podemos tambem no fim da frame o rato sempre a meio da janela portanto prendemos o rato a meio da janela
                //neste caso o rato não mexe por isso usamos o device
                int centroX = graphicsDevice.Viewport.Width / 2;
                int centroY = graphicsDevice.Viewport.Height / 2;
                int deltaX = mouse.X - centroX;//é a nova posição do x - o centro do x
                yaw = yaw - deltaX * escalaRadianosPorPixel;
                int deltaY = mouse.Y - centroY;
                pitch += deltaY * escalaRadianosPorPixel;
                pitch = MathHelper.Clamp(pitch, -1, 1);
                Matrix rotacao = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);   //criar uma rotação
                Vector3 dir = Vector3.Transform(Vector3.UnitZ/*é um vetor base ,direção inicial olhar para frente*/, rotacao/*vetor rotação*/);//transformar vetor base(naão a yaw nem pitch) com a rotação



                Vector3 right = Vector3.Cross(dir, Vector3.Up);//cross do vetor que estamos a andar com a vertical
                                                               //atualização da posição

                target = pos + dir;
                this.viewMatrix = Matrix.CreateLookAt(pos, target, Vector3.Up);

                Mouse.SetPosition(centroX, centroY);
                float vel = 30;
                if (keyboard.IsKeyDown(Keys.NumPad8))
                {
                    pos = pos + dir * vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }


                if (keyboard.IsKeyDown(Keys.NumPad5))
                {
                    pos = pos - dir * vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (keyboard.IsKeyDown(Keys.NumPad4))
                {
                    pos = pos - right * vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (keyboard.IsKeyDown(Keys.NumPad6))
                {
                    pos = pos + right * vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (keyboard.IsKeyDown(Keys.NumPad7))
                {
                    pos.Y += 0.5f;
                }
                if (keyboard.IsKeyDown(Keys.NumPad1))
                {
                    pos.Y -= 0.5f;
                }

            }
            if (mode == PlayerMode.CameraTank){
                Vector3 cameraTarget = tank1.pos + tank1.dir + new Vector3(0, 3, 0);                                        //target da camara calculado com posição e direção do tank e somado um vector3 para não olhar demasiado para baixo
                pos = tank1.pos - Vector3.Normalize(tank1.rotation.Backward) * 3f + new Vector3(0, 4, 0);                   //posição da camara calculado com posição da camara e vetor de tras da rotação da mesma, sendo somado um vector3 para a camara nao estar dentro do tank 
                viewMatrix = Matrix.CreateLookAt(pos, cameraTarget, Vector3.Up);                                       //atualização da viewMatrix
            }
            
            }


    }
}
