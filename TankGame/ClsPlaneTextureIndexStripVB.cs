using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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
            vertexCount = this.terreno.Height * this.terreno.Width;//numero de vertices + a quantidade que ocupa
            vertices = new VertexPositionNormalTexture[vertexCount];



            Color[] heightMapColors = new Color[this.terreno.Width * this.terreno.Height];
            this.terreno.GetData(heightMapColors);
            float[,] heightData = new float[this.terreno.Width, this.terreno.Height];

            for (int X = 0; X < this.terreno.Height; X++)
            {
                for (int Z = 0; Z < this.terreno.Width; Z++)
                {
                    
                    if (Z % 2 != 0) {
                        a = 1.0f; 
                    }
                    if (X % 2 != 0)
                    {
                        b = 1.0f;
                    }
                    heightData[X, Z] = heightMapColors[X + Z * this.terreno.Width].R / 20.0f;
                    vertices[X * this.terreno.Width + Z] = new VertexPositionNormalTexture(new Vector3(X, heightData[X, Z], Z), Vector3.UnitY ,new Vector2(a, b));

                    a = 0.0f;
                    b = 0.0f;
                }
            }
            
            //calculo das normais
            //centro
            for (int z = 1; z <  terreno.Width - 1; z++)
            {
                for (int x = 1; x < terreno.Height - 1; x++)
                {

                    Vector3 pC = vertices[(x * terreno.Height + (z - 1)) + 1].Position;
                    Vector3 vectorC1 = vertices[(x * terreno.Height + (z - 1))].Position - pC;
                    Vector3 vectorC2 = vertices[(x + 1) * terreno.Height + (z - 1)].Position - pC;
                    Vector3 vectorC3 = vertices[(x + 1) * terreno.Height + (z - 1) + 1].Position - pC;
                    Vector3 vectorC4 = vertices[(x + 1) * terreno.Height + z + 1].Position - pC;
                    Vector3 vectorC5 = vertices[(x * terreno.Height + (z - 1)) + 2].Position - pC;
                    Vector3 vectorC6 = vertices[(x - 1) * terreno.Height + z + 1].Position - pC;
                    Vector3 vectorC7 = vertices[(x - 1) * terreno.Height + z].Position - pC;
                    Vector3 vectorC8 = vertices[(x - 1) * terreno.Height + z - 1].Position - pC;

                    Vector3 vector1 = Vector3.Cross(vectorC2, vectorC1);
                    Vector3 vector2 = Vector3.Cross(vectorC3, vectorC2);
                    Vector3 vector3 = Vector3.Cross(vectorC4, vectorC3);
                    Vector3 vector4 = Vector3.Cross(vectorC5, vectorC4);
                    Vector3 vector5 = Vector3.Cross(vectorC6, vectorC5);
                    Vector3 vector6 = Vector3.Cross(vectorC7, vectorC5);
                    Vector3 vector7 = Vector3.Cross(vectorC8, vectorC7);
                    Vector3 vector8 = Vector3.Cross(vectorC8, vectorC1);

                    Vector3 media = (Vector3)(vector1 + vector2 + vector3 + vector4 + vector5 + vector6 + vector7 + vector8) / (float)8;
                    media.Normalize();
                    vertices[(x * terreno.Height + (z - 1)) + 1].Normal = media;

                    //fazer draw a lineList (posTerreno, posTerreno + normal)
                }
            }

            //calculo superior esquerdo das normais 
            Vector3 pSE = vertices[0].Position;
            Vector3 vetorSE1 = vertices[terreno.Width].Position - pSE;
            Vector3 vetorSE2 = vertices[terreno.Width + 1].Position - pSE;
            Vector3 vetorSE3 = vertices[1].Position - pSE;
            Vector3 vSE1 = Vector3.Cross(vetorSE2, vetorSE1);
            Vector3 vSE2 = Vector3.Cross(vetorSE3, vetorSE2);
            Vector3 vSE3 = Vector3.Cross(vetorSE3, vetorSE1);
            Vector3 mediaSE = (Vector3)(vSE1 + vSE2 + vSE3) / (float)3;
            mediaSE.Normalize();
            vertices[0].Normal = mediaSE;



            //calculo superior direito das normais
            Vector3 pSD = vertices[terreno.Width * (terreno.Height - 1)].Position;
            Vector3 vetorSD1 = vertices[terreno.Width * (terreno.Height - 2)].Position - pSD;
            Vector3 vetorSD2 = vertices[terreno.Width * (terreno.Height - 2) + 1].Position - pSD;
            Vector3 vetorSD3 = vertices[terreno.Width * (terreno.Height - 1) + 1].Position - pSD;
            Vector3 vSD1 = Vector3.Cross(vetorSD2, vetorSD1);
            Vector3 vSD2 = Vector3.Cross(vetorSD3, vetorSD2);
            Vector3 vSD3 = Vector3.Cross(vetorSD3, vetorSD1);
            Vector3 mediaSD = (Vector3)(vSD1 + vSD2 + vSD3) / (float)3;
            mediaSD.Normalize();
            vertices[terreno.Width * (terreno.Height - 1)].Normal = mediaSD;


            //calculo inferior direito das normais
            Vector3 pID = vertices[terreno.Width * terreno.Height - 1].Position;
            Vector3 vetorID1 = vertices[terreno.Width * (terreno.Height - 1) - 1].Position - pID;
            Vector3 vetorID2 = vertices[terreno.Width * (terreno.Height - 1) - 2].Position - pID;
            Vector3 vetorID3 = vertices[terreno.Width * terreno.Height - 2].Position - pID;
            Vector3 vID1 = Vector3.Cross(vetorID2, vetorID1);
            Vector3 vID2 = Vector3.Cross(vetorID3, vetorID2);
            Vector3 vID3 = Vector3.Cross(vetorID3, vetorID1);
            Vector3 mediaID = (Vector3)(vID1 + vID2 + vID3) / (float)3;
            mediaID.Normalize();
            vertices[terreno.Width * terreno.Height - 1].Normal = mediaID;

            //calculo inferior esquerdo das normais

            Vector3 pIE = vertices[(terreno.Width) - 1].Position;
            Vector3 vetorIE1 = vertices[terreno.Width - 2].Position - pIE;
            Vector3 vetorIE2 = vertices[terreno.Width * 2 - 2].Position - pIE;
            Vector3 vetorIE3 = vertices[terreno.Width * 2 - 1].Position - pIE;
            Vector3 vIE1 = Vector3.Cross(vetorIE2, vetorIE1);
            Vector3 vIE2 = Vector3.Cross(vetorIE3, vetorIE2);
            Vector3 vIE3 = Vector3.Cross(vetorIE3, vetorIE1);
            Vector3 mediaIE = (Vector3)(vIE1 + vIE2 + vIE3) / (float)3;
            mediaIE.Normalize();
            vertices[(terreno.Width) - 1].Normal = mediaIE;

            //coluna de cima
            for (int topo = 1; topo < terreno.Width - 1; topo++)
            {
                Vector3 pt = vertices[topo * terreno.Height].Position;
                Vector3 vectorTopo1 = vertices[((topo + 1) * terreno.Height)].Position - pt;
                Vector3 vectorTopo2 = vertices[(((topo + 1) * terreno.Height)) + 1].Position - pt;
                Vector3 vectorTopo3 = vertices[(((topo) * terreno.Height)) + 1].Position - pt;
                Vector3 vectorTopo4 = vertices[(topo - 1) * terreno.Height + 1].Position - pt;
                Vector3 vectorTopo5 = vertices[(topo - 1) * terreno.Height].Position - pt;

                Vector3 vt1 = Vector3.Cross(vectorTopo2, vectorTopo1);
                Vector3 vt2 = Vector3.Cross(vectorTopo3, vectorTopo2);
                Vector3 vt3 = Vector3.Cross(vectorTopo4, vectorTopo3);
                Vector3 vt4 = Vector3.Cross(vectorTopo5, vectorTopo4);
                Vector3 vt5 = Vector3.Cross(vectorTopo5, vectorTopo1);
                
                Vector3 mediaTopo = (Vector3)(vt1 + vt2 + vt3 + vt4 + vt5) / (float)5;
                mediaTopo.Normalize();
                vertices[topo * terreno.Height].Normal = mediaTopo;

            }

            //coluna da direita
            for (int direita = 1; direita < terreno.Width - 1; direita++)
            {
                Vector3 PD = vertices[(terreno.Width - 1) * terreno.Height + direita].Position;
                Vector3 vetordireita1 = vertices[(terreno.Width - 1) * terreno.Height + direita - 1].Position - PD;
                Vector3 vetordireita2 = vertices[(terreno.Width - 2) * terreno.Height + direita - 1].Position - PD;
                Vector3 vetordireita3 = vertices[(terreno.Width - 2) * terreno.Height + direita].Position - PD;
                Vector3 vetordireita4 = vertices[(terreno.Width - 2) * terreno.Height + direita + 1].Position - PD;
                Vector3 vetordireita5 = vertices[(terreno.Width - 1) * terreno.Height + direita + 1].Position - PD;
                Vector3 vd1 = Vector3.Cross(vetordireita1, vetordireita2);
                Vector3 vd2 = Vector3.Cross(vetordireita2, vetordireita3);
                Vector3 vd3 = Vector3.Cross(vetordireita3, vetordireita4);
                Vector3 vd4 = Vector3.Cross(vetordireita4, vetordireita5);
                Vector3 vd5 = Vector3.Cross(vetordireita5, vetordireita1);
                Vector3 mediaRight = (Vector3)(vd1 + vd2 + vd3 + vd4 + vd5) / (float)5;
                mediaRight.Normalize();
                vertices[(terreno.Width - 1) * terreno.Height + direita].Normal = mediaRight;
            }

            
            for (int chao = 1; chao < terreno.Width - 1; chao++)// coluna de baixo
            {

                Vector3 Pch = vertices[(((chao + 1) * terreno.Height)) - 1].Position;
                Vector3 vetorchao1 = vertices[((chao) * terreno.Height) - 1].Position - Pch;
                Vector3 vetorchao2 = vertices[((chao) * terreno.Height) - 2].Position - Pch;
                Vector3 vetorchao3 = vertices[((chao + 1) * terreno.Height) - 2].Position - Pch;
                Vector3 vetorchao4 = vertices[((chao + 2) * terreno.Height) - 2].Position - Pch;
                Vector3 vetorchao5 = vertices[((chao + 2) * terreno.Height) - 1].Position - Pch;

                Vector3 nCh1 = Vector3.Cross(vetorchao2, vetorchao1);
                Vector3 nCh2 = Vector3.Cross(vetorchao3, vetorchao2);
                Vector3 nCh3 = Vector3.Cross(vetorchao4, vetorchao3);
                Vector3 nCh4 = Vector3.Cross(vetorchao5, vetorchao4);
                Vector3 nCh5 = Vector3.Cross(vetorchao5, vetorchao1);

                Vector3 mediaChao = (Vector3)(nCh1 + nCh2 + nCh3 + nCh4 + nCh5) / (float)5;
                mediaChao.Normalize();
                vertices[(((chao + 1) * terreno.Height)) - 1].Normal = mediaChao;
            }

            //calculo das normais no limite esquerdo do terreno
            for (int esquerda = 1; esquerda < terreno.Width; esquerda++)
            {

                Vector3 PE = vertices[esquerda].Position;
                Vector3 vetorE1 = vertices[(esquerda - 1)].Position - PE;
                Vector3 vetorE2 = vertices[(esquerda + 1) + terreno.Height - 2].Position - PE;
                Vector3 vetorE3 = vertices[esquerda + terreno.Height].Position - PE;
                Vector3 vetorE4 = vertices[(esquerda - 1) + terreno.Height + 2].Position - PE;
                Vector3 vetorE5 = vertices[(esquerda + 1)].Position - PE;



                Vector3 vE1 = Vector3.Cross(vetorE2, vetorE1);
                Vector3 vE2 = Vector3.Cross(vetorE3, vetorE2);
                Vector3 vE3 = Vector3.Cross(vetorE4, vetorE3);
                Vector3 vE4 = Vector3.Cross(vetorE5, vetorE4);
                vE1 = vE1 / vE1.Length();
                vE2 = vE2 / vE2.Length();
                vE3 = vE3 / vE3.Length();
                vE4 = vE4 / vE4.Length();
                Vector3 mediaE = (Vector3)(vE1 + vE2 + vE3 + vE4) / (float)4;
                mediaE.Normalize();
                vertices[esquerda].Normal = mediaE;
            }
        


        vertexBufferFaces = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            vertexBufferFaces.SetData<VertexPositionNormalTexture>(vertices);

            //indices
            indexCount = 2 * ((this.terreno.Width* this.terreno.Height)-1);
            short[] indices = new short[2 * (this.terreno.Height * this.terreno.Width)];//para cada lado meto dois incies vertice de baixo e o vertice de cima o +1 é para fechar o triangulo
            int posicao = 0;
            for (int i = 0; i < this.terreno.Width ; i++)
            {
                for (int j = 0; j < this.terreno.Height; j++)
                {
                    //primeiros indices
                    indices[posicao++] = (short)((i * this.terreno.Height) + j);
                    indices[posicao++] = (short)(((1+ i) * this.terreno.Height) + j);
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

        public Vector3[] GetNormalsFromXZ(int x, int z){
            Vector3[] vectors = new Vector3[4];
            vectors[0] = vertices[z + x * terreno.Width].Normal;
            vectors[1] = vertices[z + 1 + x * terreno.Width].Normal;
            vectors[2] = vertices[z + (x + 1) * terreno.Width].Normal;
            vectors[3] = vertices[z + 1 + (x + 1) * terreno.Width].Normal;

            return vectors;
        }
    }
}
