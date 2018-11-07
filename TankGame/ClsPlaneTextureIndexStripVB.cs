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
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1.0f/*maior numero afasta a camara*/, 20.0f/*maior numero ver top*/, 20.0f),Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.2f, 1000.0f);
            effect.LightingEnabled = true;//iluminação  ligada
            //effect.EnableDefaultLighting();
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction =Vector3.Normalize( new Vector3(1, -0.5f, 0));
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);// Color.LightYellow.ToVector3();
            //effect.DirectionalLight0.SpecularColor = new Vector3(0.6f, 0.6f, 0.6f);
            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
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
            
                //calculo das normais
                for (int x = 1; x < terreno.Width - 1; x++)
                {
                    for (int z = 1; z < terreno.Height - 1; z++)
                    {
                        Vector3 posCenter = vertices[x * terreno.Height + z].Position;
                        Vector3 vectorCenter1 = vertices[(x - 1) * terreno.Height + z].Position - posCenter;
                        Vector3 vectorCenter2 = vertices[(x - 1) * terreno.Height + z + 1].Position - posCenter;
                        Vector3 vectorCenter3 = vertices[x * terreno.Height + z + 1].Position - posCenter;
                        Vector3 vectorCenter4 = vertices[(x + 1) * terreno.Height + z + 1].Position - posCenter;
                        Vector3 vectorCenter5 = vertices[(x + 1) * terreno.Height + z].Position - posCenter;
                        Vector3 vectorCenter6 = vertices[(x + 1) * terreno.Height + z - 1].Position - posCenter;
                        Vector3 vectorCenter7 = vertices[x * terreno.Height + z - 1].Position - posCenter;
                        Vector3 vectorCenter8 = vertices[(x - 1) * terreno.Height + z - 1].Position - posCenter;
                       

                        Vector3 vector1 = Vector3.Cross(vectorCenter1, vectorCenter2); vector1.Normalize();
                        Vector3 vector2 = Vector3.Cross(vectorCenter2, vectorCenter3); vector2.Normalize();
                        Vector3 vector3 = Vector3.Cross(vectorCenter3, vectorCenter4); vector3.Normalize();
                        Vector3 vector4 = Vector3.Cross(vectorCenter4, vectorCenter5); vector4.Normalize();
                        Vector3 vector5 = Vector3.Cross(vectorCenter5, vectorCenter6); vector5.Normalize();
                        Vector3 vector6 = Vector3.Cross(vectorCenter6, vectorCenter7); vector6.Normalize();
                        Vector3 vector7 = Vector3.Cross(vectorCenter7, vectorCenter8); vector7.Normalize();
                        Vector3 vector8 = Vector3.Cross(vectorCenter8, vectorCenter1); vector8.Normalize();

                        Vector3 media = (Vector3)(vector1 + vector2 + vector3 + vector4 + vector5 + vector6 + vector7 + vector8) / (float)8;
                        media.Normalize();
                        vertices[x * terreno.Height + z].Normal = media;

                        //fazer draw a lineList (posTerreno, posTerreno + normal)
                    }
                }

                //calculo superior esquerdo das normais

                Vector3 posSE = vertices[0].Position;
                Vector3 vetorSupE1 = vertices[1].Position - posSE;
                Vector3 vetorSupE2 = vertices[terreno.Height + 1].Position - posSE;
                Vector3 vetorSupE3 = vertices[terreno.Height].Position - posSE;
                Vector3 vSE1 = Vector3.Cross(vetorSupE1, vetorSupE2);
                Vector3 vSE2 = Vector3.Cross(vetorSupE2, vetorSupE3);
                Vector3 vSE3 = Vector3.Cross(vetorSupE3, vetorSupE1);
                Vector3 mediaSE = (Vector3)(vSE1 + vSE2 + vSE3) / (float)3;
                mediaSE.Normalize(); 
                vertices[0].Normal = mediaSE;



                //calculo superior direito das normais
                Vector3 posSD = vertices[terreno.Width - 1].Position;
                Vector3 vetorSupD1 = vertices[2 * terreno.Height - 1].Position - posSD;
                Vector3 vetorSupD2 = vertices[2 * terreno.Height - 2].Position - posSD;
                Vector3 vetorSupD3 = vertices[terreno.Height - 2].Position - posSD;
                Vector3 vSD1 = Vector3.Cross(vetorSupD1, vetorSupD2);
                Vector3 vSD2 = Vector3.Cross(vetorSupD2, vetorSupD3);
                Vector3 vSD3 = Vector3.Cross(vetorSupD3, vetorSupD1);
                Vector3 mediaSupD = (Vector3)(vSD1 + vSD2 + vSD3) / (float)3;
                mediaSupD.Normalize();
                vertices[terreno.Height - 1].Normal = mediaSupD;


                //calculo inferior direito das normais
                Vector3 PInfD = vertices[terreno.Width * terreno.Height - 1].Position;
                Vector3 vetorInfD1 = vertices[terreno.Width * terreno.Height - 2].Position - PInfD;
                Vector3 vetorInfD2 = vertices[(terreno.Width - 1) * terreno.Height - 2].Position - PInfD;
                Vector3 vetorInfD3 = vertices[(terreno.Width - 1) * terreno.Height - 1].Position - PInfD;
                Vector3 vID1 = Vector3.Cross(vetorInfD1, vetorInfD2);
                Vector3 vID2 = Vector3.Cross(vetorInfD2, vetorInfD3);
                Vector3 vID3 = Vector3.Cross(vetorInfD3, vetorInfD1);
                Vector3 mediaInfD = (Vector3)(vID1 + vID2 + vID3) / (float)3;
                mediaInfD.Normalize();
                vertices[terreno.Width * terreno.Height - 1].Normal = mediaInfD;

                //calculo inferior esquerdo das normais

                Vector3 posInfE = vertices[(terreno.Width - 1) * terreno.Height].Position;
                Vector3 vetorInfE1 = vertices[(terreno.Width - 2) * terreno.Height].Position - posInfE;
                Vector3 vetorInfE2 = vertices[(terreno.Width - 2) * terreno.Height + 1].Position - posInfE;
                Vector3 vetorInfE3 = vertices[(terreno.Width - 1) * terreno.Height + 1].Position - posInfE;
                Vector3 vIE1 = Vector3.Cross(vetorInfE1, vetorInfE2);
                Vector3 vIE2 = Vector3.Cross(vetorInfE2, vetorInfE3);
                Vector3 vIE3 = Vector3.Cross(vetorInfE3, vetorInfE1);
                Vector3 mediaInfE = (Vector3)(vIE1 + vIE2 + vIE3) / (float)3;
                mediaInfE.Normalize();
                vertices[(terreno.Width - 1) * terreno.Height].Normal = mediaInfE;





                //calculo das normais da esquerda
                for (int topRow = 1; topRow < terreno.Width - 1; topRow++)
                {
                    Vector3 PTopo = vertices[topRow].Position;
                    Vector3 vetorTopo1 = PTopo - vertices[topRow + 1].Position;
                    Vector3 vetorTopo2 = PTopo - vertices[topRow + 1 + terreno.Width].Position;
                    Vector3 vetorTopo3 = PTopo - vertices[topRow + terreno.Width].Position;
                    Vector3 vetorTopo4 = PTopo - vertices[topRow - 1 + terreno.Width].Position;
                    Vector3 vetorTopo5 = PTopo - vertices[topRow - 1].Position;

                    Vector3 nTopo1 = -Vector3.Cross(vetorTopo2, vetorTopo1);
                    Vector3 nTopo2 = -Vector3.Cross(vetorTopo3, vetorTopo2);
                    Vector3 nTopo3 = -Vector3.Cross(vetorTopo4, vetorTopo3);
                    Vector3 nTopo4 = -Vector3.Cross(vetorTopo5, vetorTopo4);
                    Vector3 nTopo5 = -Vector3.Cross(vetorTopo1, vetorTopo5);
                    Vector3 mediaTopo = (Vector3)(nTopo1 + nTopo2 + nTopo3 + nTopo4 + nTopo5) / (float)5;
                    mediaTopo.Normalize();
                    vertices[topRow].Normal = mediaTopo;



                }



                //calculo das normais no limite inferior do terreno
                for (int bottonRow = 1; bottonRow < terreno.Width - 2; bottonRow++)
                {
                    Vector3 Pchao = vertices[(terreno.Width - 1) * terreno.Height + bottonRow].Position;
                    Vector3 vetorchao1 = Pchao - vertices[(terreno.Width - 1) * terreno.Height + bottonRow - 1].Position;
                    Vector3 vetorchao2 = Pchao - vertices[(terreno.Width - 2) * terreno.Height + bottonRow - 1].Position;
                    Vector3 vetorchao3 = Pchao - vertices[(terreno.Width - 2) * terreno.Height + bottonRow].Position;
                    Vector3 vetorchao4 = Pchao - vertices[(terreno.Width - 2) * terreno.Height + bottonRow + 1].Position;
                    Vector3 vetorchao5 = Pchao - vertices[(terreno.Width - 1) * terreno.Height + bottonRow + 1].Position;
                    Vector3 nChao1 = Vector3.Cross(vetorchao1, vetorchao2);
                    Vector3 nChao2 = Vector3.Cross(vetorchao2, vetorchao3);
                    Vector3 nChao3 = Vector3.Cross(vetorchao3, vetorchao4);
                    Vector3 nChao4 = Vector3.Cross(vetorchao4, vetorchao5);
                    Vector3 nChao5 = Vector3.Cross(vetorchao5, vetorchao1);
                    Vector3 mediaChao = (Vector3)(nChao1 + nChao2 + nChao3 + nChao4 + nChao5) / (float)5;
                    mediaChao.Normalize();
                    vertices[(terreno.Width - 1) * terreno.Height + bottonRow].Normal = mediaChao;
                }


                //calculo das normais no limite direito do terreno
                for (int ladoDir = 1; ladoDir < terreno.Width - 2; ladoDir++)
                {
                    Vector3 PD = vertices[ladoDir * terreno.Height].Position;
                    Vector3 vetorD1 = vertices[(ladoDir - 1) * terreno.Height].Position - PD;
                    Vector3 vetorD2 = vertices[(ladoDir - 1) * terreno.Height + 1].Position - PD;
                    Vector3 vetorD3 = vertices[ladoDir * terreno.Height + 1].Position - PD;
                    Vector3 vetorD4 = vertices[(ladoDir + 1) * terreno.Height + 1].Position - PD;
                    Vector3 vetorD5 = vertices[(ladoDir + 1) * terreno.Height].Position - PD;

                    Vector3 nD1 = -Vector3.Cross(vetorD1, vetorD2);
                    Vector3 nD2 = -Vector3.Cross(vetorD2, vetorD3);
                    Vector3 nD3 = -Vector3.Cross(vetorD3, vetorD4);
                    Vector3 nD4 = -Vector3.Cross(vetorD4, vetorD5);
                    Vector3 nD5 = -Vector3.Cross(vetorD5, vetorD1);
                    Vector3 mediaD = (Vector3)(nD1 + nD2 + nD3 + nD4 + nD5) / (float)5;
                    mediaD.Normalize();
                    vertices[ladoDir * terreno.Height].Normal = mediaD;
                }


                //calculo das normais no limite esquerdo do terreno (actually é o direito)
                for (int ladoEsq = 2; ladoEsq < terreno.Width - 1; ladoEsq++)
                {
                    Vector3 PE = vertices[ladoEsq * terreno.Height - 1].Position;
                    Vector3 vetorE1 = vertices[(ladoEsq + 1) * terreno.Height - 1].Position - PE;
                    Vector3 vetorE2 = vertices[(ladoEsq + 1) * terreno.Height - 2].Position - PE;
                    Vector3 vetorE3 = vertices[ladoEsq * terreno.Height - 1].Position - PE;
                    Vector3 vetorE4 = vertices[(ladoEsq - 1) * terreno.Height - 2].Position - PE;
                    Vector3 vetorE5 = vertices[(ladoEsq - 1) * terreno.Height - 1].Position - PE;

                    Vector3 nE1 = -Vector3.Cross(vetorE1, vetorE2);
                    Vector3 nE2 = -Vector3.Cross(vetorE2, vetorE3);
                    Vector3 nE3 = -Vector3.Cross(vetorE3, vetorE4);
                    Vector3 nE4 = -Vector3.Cross(vetorE4, vetorE5);
                    Vector3 nE5 = -Vector3.Cross(vetorE5, vetorE1);
                    Vector3 mediaD = (Vector3)(nE1 + nE2 + nE3 + nE4 + nE5) / (float)5;
                    mediaD.Normalize();
                    vertices[ladoEsq * terreno.Height].Normal = mediaD;
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


           
        }
    

        public void Draw(GraphicsDevice device, Camera camera)
        {
            // World Matrix
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projection;
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
