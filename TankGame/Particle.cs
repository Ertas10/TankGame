using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Particle
    {
        VertexPositionColor[] vertices;     //array de VertexPositionColor que contém os 2 pontos que fazem a particula
        Matrix worldMatrix;                 //worldMatrix da particula
        Vector3 pos;                        //posição da particula, usada para criar e mexer na worldMatrix, e para dar o 2º ponto do array de VertexPositionColor
        Vector3 prevPos;                    //posição anterior da particula, usada para dar o 1º ponto do array de VertexPositionColor
        Vector3 dir;                        //direção de movimento da particula
        Color color;                        //cor da particula
        DateTime timeStamp;                 //momento da criação da particula
        BasicEffect effect;                 //BasicEffect da particula
        float lifeTime;                     //tempo de vida máximo da particula, em segundos
        public bool setToDestroy;           //serve para verificar se a particula morreu

        public Particle(GraphicsDevice device, Vector3 startingPos, Vector3 speed, Color color, float lifeTime)
        {
            this.effect = new BasicEffect(device);
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.vertices = new VertexPositionColor[2];
            this.vertices[0] = new VertexPositionColor(startingPos, color);
            this.pos = startingPos + new Vector3(0, 1, 0);
            this.worldMatrix = Matrix.CreateWorld(pos, Vector3.Forward, Vector3.Up);
            this.dir = speed;
            this.color = color;
            this.lifeTime = lifeTime;
            this.timeStamp = DateTime.Now;
            this.prevPos = startingPos;
            this.setToDestroy = false;
        }

        public void Update(GameTime gameTime, ClsPlaneTextureIndexStripVB terreno)
        {
            prevPos = pos;                                                                                          //
            pos += dir * (float)gameTime.ElapsedGameTime.TotalSeconds;                                      //Move a particula
            worldMatrix = Matrix.CreateWorld(pos, Vector3.Forward, Vector3.Up);                                     //

            vertices[1] = new VertexPositionColor(pos, color);                                                      //Atualiza o array
            vertices[0] = new VertexPositionColor(prevPos, color);

            Vector3[] vectors = terreno.GetVerticesFromXZ((int)pos.X, (int)pos.Z);              // Vai buscar os vetores que circulam a camara
            float YA = vectors[0].Y;                                                            //
            float YB = vectors[1].Y;                                                            // Posição Y de cada vetor
            float YC = vectors[2].Y;                                                            //
            float YD = vectors[3].Y;                                                            //
            float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);          // Interpolação do Y entre A e B
            float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);          // Interpolação do Y entre C e D
            float terrainY = (((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD;
            Debug.Print(pos.ToString());
            if ((DateTime.Now - timeStamp) > new TimeSpan(0, 0, 0, (int)lifeTime) || pos.Y <= terrainY)             //Verifica se a particula deve ser destruida
                setToDestroy = true;
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projection;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
