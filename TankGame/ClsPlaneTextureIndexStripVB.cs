using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class ClsPlaneTextureIndexStripVB/*vertex buffer alloca memoria na placa grafica em vez do cpu a enviar 60 vezes por segundo para o gpu e no draw digo para usar o index buffer da gpu*/
    {
        VertexBuffer vertexBufferFaces;
        IndexBuffer indexBufferFaces;
        BasicEffect effect;
        Matrix worldMatrix;
        public Texture2D terreno;
        Texture2D textura;
        int vertexCount;
        int indexCount;
        public VertexPositionTexture[] vertices;

        public ClsPlaneTextureIndexStripVB(GraphicsDevice device, float planeLength, Texture2D terreno, Texture2D textura)
        {
            // Vamos usar um efeito básico
            effect = new BasicEffect(device);
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width /
            device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1.0f/*maior numero afasta a camara*/, 20.0f/*maior numero ver top*/, 20.0f),Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(90.0f)/*angulo da camara (aumentar o numero faz un zoom)*/, aspectRatio, 1.0f, 1000.0f)/*relação da altura*/;
            effect.LightingEnabled = false;//iluminação  ligada
            effect.VertexColorEnabled = false;//cor 
            this.worldMatrix = Matrix.Identity;//guardar na classe uma matrix identidade = diagonal com 1
            effect.TextureEnabled = true;
            this.terreno = terreno;
            this.textura = textura;
            // Cria os eixos 3D
            CreateGeometry(device, planeLength);
        }


        private void CreateGeometry(GraphicsDevice device, float planeLength)
        {

            float a = 0.0f, b=0.0f;
            vertexCount = terreno.Height * terreno.Width;//numero de vertices + a quantidade que ocupa
            vertices = new VertexPositionTexture[vertexCount];

            

            Color[] heightMapColors = new Color[terreno.Width * terreno.Height];
            terreno.GetData(heightMapColors);
            float[,] heightData = new float[terreno.Width, terreno.Height];

            for (int X = 0; X < terreno.Height; X++)
            {
                for (int Z = 0; Z < terreno.Width; Z++)
                {
                    
                    if (Z % 2 != 0) {
                        a = 1.0f; 
                    }
                    if (X % 2 != 0)
                    {
                        b = 1.0f;
                    }
                    heightData[X, Z] = heightMapColors[X + Z * terreno.Width].R / 10.0f;
                    vertices[X * terreno.Width + Z] = new VertexPositionTexture(new Vector3(X, heightData[X, Z], Z), new Vector2(a, b));

                    a = 0.0f;
                    b = 0.0f;
                }
            }
            


            vertexBufferFaces = new VertexBuffer(device, typeof(VertexPositionTexture), vertexCount, BufferUsage.None);
            vertexBufferFaces.SetData<VertexPositionTexture>(vertices);

            //indices
            indexCount = 2 * ((terreno.Width*terreno.Height)-1);
            short[] indices = new short[2 * (terreno.Height * terreno.Width)];//para cada lado meto dois incies vertice de baixo e o vertice de cima o +1 é para fechar o triangulo
            int posicao = 0;
            for (int i = 0; i < terreno.Width ; i++)
            {
                for (int j = 0; j < terreno.Height; j++)
                {
                    //primeiros indices
                    indices[posicao++] = (short)((i * terreno.Height) + j);
                    indices[posicao++] = (short)(((1+i) * terreno.Height) + j);
                }



            }
            indexBufferFaces = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            indexBufferFaces.SetData<short>(indices);
            Debug.Print("X: " + vertices[0].Position.X + "\nY: " + vertices[0].Position.Z);
            Debug.Print("\nX: " + vertices[vertices.Length-1].Position.X + "\nY: " + vertices[vertices.Length - 1].Position.Z);
        }

        public void Draw(GraphicsDevice device, Matrix viewMatrix)
        {
            // World Matrix
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Texture = this.textura;



            device.SetVertexBuffer(vertexBufferFaces);
            device.Indices = indexBufferFaces;
            for (int i = 0; i < terreno.Width-1; i++)
            {
                int startstrip = i * 2 * terreno.Height;
              //  device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2/*numero de triangulos*/);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0,startstrip,terreno.Height*2-2);
                
            }

        }

        public Vector3[] GetYFromXZ(int x, int z)
        {
            Vector3[] vectors = new Vector3[4];
            vectors[0] = vertices[x + z * terreno.Width].Position;
            vectors[1] = vertices[x + z * terreno.Width].Position;
            vectors[2] = vertices[x + (z + 1) * terreno.Width].Position;
            vectors[3] = vertices[x + 1 + (z + 1) * terreno.Width].Position;

            return vectors;
        }
    }
}
