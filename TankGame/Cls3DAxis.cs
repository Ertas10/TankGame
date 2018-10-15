using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Cls3DAxis
    {
        VertexPositionColor[] vertices;
        BasicEffect effect;
        Matrix worldMatrix;

        public Cls3DAxis(GraphicsDevice device)
        {
            // Vamos usar um efeito básico
            effect = new BasicEffect(device);
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width /
            device.Viewport.Height;
           
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45.0f),
            aspectRatio, 1.0f, 10.0f);
            this.worldMatrix = Matrix.Identity;
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            // Cria os eixos 3D
            CreateGeometry();
        }

        private void CreateGeometry()
        {
            float axisLenght = 1f; // Tamanho da linha em cada sinal do eixo
            int vertexCount = 6; // Vamos usar 6 vértices
            vertices = new VertexPositionColor[vertexCount];
            // Linha sobre o eixo X
            vertices[0] = new VertexPositionColor(new Vector3(-axisLenght, 0.0f, 0.0f),
            Color.White);
            vertices[1] = new VertexPositionColor(new Vector3(axisLenght, 0.0f, 0.0f),
            Color.White);
            // Linha sobre o eixo Y
            vertices[2] = new VertexPositionColor(new Vector3(0.0f, -axisLenght, 0.0f),
            Color.White);
            vertices[3] = new VertexPositionColor(new Vector3(0.0f, axisLenght, 0.0f),
            Color.White);
            // Linha sobre o eixo Z
            vertices[4] = new VertexPositionColor(new Vector3(0.0f, 0.0f, -axisLenght),
            Color.White);
            vertices[5] = new VertexPositionColor(new Vector3(0.0f, 0.0f, axisLenght),
            Color.White);




        }

        public void Draw(GraphicsDevice device, Matrix viewMatrix)
        {
            // World Matrix
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);
        }
    }
}
