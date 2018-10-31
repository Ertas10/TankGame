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
        public VertexPositionNormalTexture[] vertices;

        public ClsPlaneTextureIndexStripVB(GraphicsDevice device, float planeLength, Texture2D terreno, Texture2D textura)
        {
            // Vamos usar um efeito básico
            effect = new BasicEffect(device);
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width /
            device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1.0f/*maior numero afasta a camara*/, 20.0f/*maior numero ver top*/, 20.0f),Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(90.0f)/*angulo da camara (aumentar o numero faz un zoom)*/, aspectRatio, 0.2f, 1000.0f)/*relação da altura*/;
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
            vertices = new VertexPositionNormalTexture[vertexCount];

            

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
                    vertices[X * terreno.Width + Z] = new VertexPositionNormalTexture(new Vector3(X, heightData[X, Z], Z), Vector3.UnitY ,new Vector2(a, b));

                    a = 0.0f;
                    b = 0.0f;
                }
            }
            


            vertexBufferFaces = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            vertexBufferFaces.SetData<VertexPositionNormalTexture>(vertices);

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


            //calculo das normais no "centro" do terreno
            for (int y = 1; y < terreno.Width - 1; y++)
            {
                for (int t = 1; t < terreno.Height - 1; t++)
                {
                    Vector3 P = vertices[y * terreno.Height + t].Position;
                    Vector3 vetor1 = vertices[(y - 1) * terreno.Height + t].Position - P;       // 
                    Vector3 vetor2 = vertices[(y - 1) * terreno.Height + t + 1].Position - P;   //
                    Vector3 vetor3 = vertices[y * terreno.Height + t + 1].Position - P;         //
                    Vector3 vetor4 = vertices[(y + 1) * terreno.Height + t + 1].Position - P;   //
                    Vector3 vetor5 = vertices[(y + 1) * terreno.Height + t].Position - P;       //
                    Vector3 vetor6 = vertices[(y + 1) * terreno.Height + t - 1].Position - P;   //
                    Vector3 vetor7 = vertices[y * terreno.Height + t - 1].Position - P;         //
                    Vector3 vetor8 = vertices[(y - 1) * terreno.Height + t - 1].Position - P;   //

                    Vector3 v1 = Vector3.Cross(vetor1, vetor2); v1.Normalize();
                    Vector3 v2 = Vector3.Cross(vetor2, vetor3); v2.Normalize();
                    Vector3 v3 = Vector3.Cross(vetor3, vetor4); v3.Normalize();
                    Vector3 v4 = Vector3.Cross(vetor4, vetor5); v4.Normalize();
                    Vector3 v5 = Vector3.Cross(vetor5, vetor6); v5.Normalize();
                    Vector3 v6 = Vector3.Cross(vetor6, vetor7); v6.Normalize();
                    Vector3 v7 = Vector3.Cross(vetor7, vetor8); v7.Normalize();
                    Vector3 v8 = Vector3.Cross(vetor8, vetor1); v8.Normalize();

                    Vector3 media = (v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8) / 8;
                    vertices[y * terreno.Height + t].Normal = media;
                }
            }
            //calculo das normais no canto suprior esquerdo do terreno
            Vector3 PSE = vertices[0].Position;
            Vector3 vetorSE1 = vertices[1].Position - PSE;
            Vector3 vetorSE2 = vertices[terreno.Height + 1].Position - PSE;
            Vector3 vetorSE3 = vertices[terreno.Height].Position - PSE;
            Vector3 vSE1 = Vector3.Cross(vetorSE1, vetorSE2); vSE1.Normalize();
            Vector3 vSE2 = Vector3.Cross(vetorSE2, vetorSE3); vSE2.Normalize();
            Vector3 vSE3 = Vector3.Cross(vetorSE3, vetorSE1); vSE3.Normalize();
            Vector3 mediaSE = (vSE1 + vSE2 + vSE3) / 3;
            vertices[0].Normal = mediaSE;

            //calculo das normais no canto suprior direito do terreno
            Vector3 PSD = vertices[terreno.Width - 1].Position;
            Vector3 vetorSD1 = vertices[2 * (terreno.Height - 1)].Position - PSD;
            Vector3 vetorSD2 = vertices[2 * (terreno.Height - 1) - 1].Position - PSD;
            Vector3 vetorSD3 = vertices[terreno.Height - 2].Position - PSD;
            Vector3 vSD1 = Vector3.Cross(vetorSD1, vetorSD2); vSD1.Normalize();
            Vector3 vSD2 = Vector3.Cross(vetorSD2, vetorSD3); vSD2.Normalize();
            Vector3 vSD3 = Vector3.Cross(vetorSD3, vetorSD1); vSD3.Normalize();
            Vector3 mediaSD = (vSD1 + vSD2 + vSD3) / 3;
            vertices[terreno.Width - 1].Normal = mediaSD;

            //calculo das normais no canto Iinferior esquerdo do terreno
            Vector3 PIE = vertices[(terreno.Width - 1) * terreno.Height].Position;
            Vector3 vetorIE1 = vertices[(terreno.Width - 2) * terreno.Height].Position - PIE;
            Vector3 vetorIE2 = vertices[(terreno.Width - 2) * terreno.Height + 1].Position - PIE;
            Vector3 vetorIE3 = vertices[(terreno.Width - 1) * terreno.Height + 1].Position - PIE;
            Vector3 vIE1 = Vector3.Cross(vetorIE1, vetorIE2); vIE1.Normalize();
            Vector3 vIE2 = Vector3.Cross(vetorIE2, vetorIE3); vIE2.Normalize();
            Vector3 vIE3 = Vector3.Cross(vetorIE3, vetorIE1); vIE3.Normalize();
            Vector3 mediaIE = (vIE1 + vIE2 + vIE3) / 3;
            vertices[(terreno.Width - 1) * terreno.Height].Normal = mediaIE;

            //calculo das normais no canto Iinferior esquerdo do terreno
            Vector3 PID = vertices[terreno.Width * terreno.Height - 1].Position;
            Vector3 vetorID1 = vertices[terreno.Width * terreno.Height - 2].Position - PID;
            Vector3 vetorID2 = vertices[(terreno.Width - 1) * terreno.Height - 2].Position - PID;
            Vector3 vetorID3 = vertices[(terreno.Width - 1) * terreno.Height - 1].Position - PID;
            Vector3 vID1 = Vector3.Cross(vetorID1, vetorID2); vID1.Normalize();
            Vector3 vID2 = Vector3.Cross(vetorID2, vetorID3); vID2.Normalize();
            Vector3 vID3 = Vector3.Cross(vetorID3, vetorID1); vID3.Normalize();
            Vector3 mediaID = (vID1 + vID2 + vID3) / 3;
            vertices[terreno.Width * terreno.Height - 1].Normal = mediaID;


            //calculo das normais no limite suprior do terreno
            for (int topo = 1; topo < terreno.Width - 1; topo++)
            {
                Vector3 PTopo = vertices[topo].Position;
                Vector3 vetorTopo1 = vertices[topo + 1].Position - PTopo;
                Vector3 vetorTopo2 = vertices[topo + 1 + terreno.Width].Position - PTopo;
                Vector3 vetorTopo3 = vertices[topo + terreno.Width].Position - PTopo;
                Vector3 vetorTopo4 = vertices[topo - 1 + terreno.Width].Position - PTopo;
                Vector3 vetorTopo5 = vertices[topo - 1].Position - PTopo;
                Vector3 nTopo1 = -Vector3.Cross(vetorTopo1, vetorTopo2); nTopo1.Normalize();
                Vector3 nTopo2 = -Vector3.Cross(vetorTopo2, vetorTopo3); nTopo2.Normalize();
                Vector3 nTopo3 = -Vector3.Cross(vetorTopo3, vetorTopo4); nTopo3.Normalize();
                Vector3 nTopo4 = -Vector3.Cross(vetorTopo4, vetorTopo5); nTopo4.Normalize();
                Vector3 nTopo5 = -Vector3.Cross(vetorTopo5, vetorTopo1); nTopo5.Normalize();
                Vector3 mediaTopo = (nTopo1 + nTopo2 + nTopo3 + nTopo4 + nTopo5) / 5;
                vertices[topo].Normal = mediaTopo;
            }

            //calculo das normais no limite inferior do terreno
            for (int chao = 1; chao < terreno.Width - 1; chao++)
            {
                Vector3 Pchao = vertices[(terreno.Width - 1) * terreno.Height + chao].Position;
                Vector3 vetorchao1 = vertices[(terreno.Width - 1) * terreno.Height + chao - 1].Position - Pchao;
                Vector3 vetorchao2 = vertices[(terreno.Width - 2) * terreno.Height + chao - 1].Position - Pchao;
                Vector3 vetorchao3 = vertices[(terreno.Width - 2) * terreno.Height + chao].Position - Pchao;
                Vector3 vetorchao4 = vertices[(terreno.Width - 2) * terreno.Height + chao + 1].Position - Pchao;
                Vector3 vetorchao5 = vertices[(terreno.Width - 1) * terreno.Height + chao + 1].Position - Pchao;
                Vector3 nChao1 = -Vector3.Cross(vetorchao1, vetorchao2); nChao1.Normalize();
                Vector3 nChao2 = -Vector3.Cross(vetorchao2, vetorchao3); nChao2.Normalize();
                Vector3 nChao3 = -Vector3.Cross(vetorchao3, vetorchao4); nChao3.Normalize();
                Vector3 nChao4 = -Vector3.Cross(vetorchao4, vetorchao5); nChao4.Normalize();
                Vector3 nChao5 = -Vector3.Cross(vetorchao5, vetorchao1); nChao5.Normalize();
                Vector3 mediaChao = (nChao1 + nChao2 + nChao3 + nChao4 + nChao5) / 5;
                vertices[(terreno.Width - 1) * terreno.Height + chao].Normal = mediaChao;
            }


            //calculo das normais no limite direito do terreno
            for (int lado = 1; lado < terreno.Width - 1; lado++)
            {
                Vector3 PD = vertices[lado * terreno.Height].Position;
                Vector3 vetorD1 = vertices[(lado - 1) * terreno.Height].Position - PD;
                Vector3 vetorD2 = vertices[(lado - 1) * terreno.Height + 1].Position - PD;
                Vector3 vetorD3 = vertices[lado * terreno.Height + 1].Position - PD;
                Vector3 vetorD4 = vertices[(lado + 1) * terreno.Height + 1].Position - PD;
                Vector3 vetorD5 = vertices[(lado + 1) * terreno.Height].Position - PD;

                Vector3 nD1 = -Vector3.Cross(vetorD1, vetorD2); nD1.Normalize();
                Vector3 nD2 = -Vector3.Cross(vetorD2, vetorD3); nD2.Normalize();
                Vector3 nD3 = -Vector3.Cross(vetorD3, vetorD4); nD3.Normalize();
                Vector3 nD4 = -Vector3.Cross(vetorD4, vetorD5); nD4.Normalize();
                Vector3 nD5 = -Vector3.Cross(vetorD5, vetorD1); nD5.Normalize();
                Vector3 mediaD = (nD1 + nD2 + nD3 + nD4 + nD5) / 5;
                vertices[lado * terreno.Height].Normal = mediaD;
            }


            //calculo das normais no limite direito do terreno
            for (int lado = 2; lado < terreno.Width; lado++)
            {
                Vector3 PE = vertices[lado * terreno.Height - 1].Position;
                Vector3 vetorE1 = vertices[(lado + 1) * terreno.Height - 1].Position - PE;
                Vector3 vetorE2 = vertices[(lado + 1) * terreno.Height - 2].Position - PE;
                Vector3 vetorE3 = vertices[lado * terreno.Height - 1].Position - PE;
                Vector3 vetorE4 = vertices[(lado - 1) * terreno.Height - 2].Position - PE;
                Vector3 vetorE5 = vertices[(lado - 1) * terreno.Height - 1].Position - PE;

                Vector3 nE1 = -Vector3.Cross(vetorE1, vetorE2); nE1.Normalize();
                Vector3 nE2 = -Vector3.Cross(vetorE2, vetorE3); nE2.Normalize();
                Vector3 nE3 = -Vector3.Cross(vetorE3, vetorE4); nE3.Normalize();
                Vector3 nE4 = -Vector3.Cross(vetorE4, vetorE5); nE4.Normalize();
                Vector3 nE5 = -Vector3.Cross(vetorE5, vetorE1); nE5.Normalize();
                Vector3 mediaD = (nE1 + nE2 + nE3 + nE4 + nE5) / 5;
                vertices[lado * terreno.Height].Normal = mediaD;
            }
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
        /// <summary>
        /// Retorna um array de 4 vértices que revolvem o ponto com x e z pedidos
        /// </summary>
        /// <param name="x">Coordenada x do ponto</param>
        /// <param name="z">Coordenada z do ponto</param>
        /// <returns></returns>
        public Vector3[] GetVerticesFromXZ(int x, int z)
        {
            Vector3[] vectors = new Vector3[4];
            vectors[0] = vertices[z + x * terreno.Width].Position;
            vectors[1] = vertices[z + 1 + x * terreno.Width].Position;
            vectors[2] = vertices[z + (x + 1) * terreno.Width].Position;
            vectors[3] = vertices[z + 1 + (x + 1) * terreno.Width].Position;

            return vectors;
        }
    }
}
