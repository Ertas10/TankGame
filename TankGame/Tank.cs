﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TankGame
{
    class Tank
    {
        public enum PlayerMode{
        AI,
        PC
        }
        int id;
        PlayerMode mode;
        float Speed = 3f;
        public Vector3 pos;
        Model model;
        ClsPlaneTextureIndexStripVB terrain;
        public float yaw = 0;
        //Bones
        ModelBone turretBone, cannonBone;
        //matrices
        float scale = 0.005f;
        //Default Transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        //Keeps all transforms
        Matrix[] boneTransforms;
        public float cannonRot = 0f;
        public float turretRot = 0f;
        public float tankangle = 0f;
        GenerateParticle dustcloud;
        public Matrix rotation;
        public Matrix translation;
        public List<Bullets> bullets;

        public Tank(Model model, ClsPlaneTextureIndexStripVB terrain, Vector3 startingPos, GraphicsDevice graphicsDevice, PlayerMode playermode, int ID){

            this.pos = startingPos;                                                                                                         //posição inicial do tank no terreno
            this.mode = playermode;                                                                                                         //indica se o tank está em modo "AI" ou modo controlado por jogador
            this.model = model;                                                                                                             //modelo do tank
            this.model.Root.Transform = Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(startingPos);    //matriz inicial de posição, rotação e escala do tank
            boneTransforms = new Matrix[model.Bones.Count];                                                                                 //bone transforms do tank
            this.model.CopyAbsoluteBoneTransformsTo(boneTransforms);                                                                        //
            this.terrain = terrain;                                                                                                         //terreno ao qual o tank está "bound"
            this.id = ID;
            bullets = new List<Bullets>();

            turretBone = model.Bones["turret_geo"];                                                                                         //bones da turret
            cannonBone = model.Bones["canon_geo"];                                                                                          //bones do canhão
            turretTransform = turretBone.Transform;                                                                                         //bone transforms da turret
            cannonTransform = cannonBone.Transform;                                                                                         //bone transforms do canhão
            dustcloud = new GenerateParticle(graphicsDevice, pos);
        }

        public void Update(KeyboardState keyboard, GameTime gameTime,  ContentManager content, Tank otherTank, Camera camara)
        {
            for (int i = 0; i < bullets.Count; i++) //each tanks bullets ground or walls
            {
                bullets[i].Update(gameTime);
                //if (bullets[i].HitGround(bullets[i].position, terrain.terreno.Height))
                //{
                //    bullets.Remove(bullets[i]);
                //}
            }


            if (mode == PlayerMode.AI){

                Vector3 sp = new Vector3(0.1f, 0, 0);
                if ((pos - sp - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {//limite
                    
                        pos -= sp;
                        dustcloud.CreateCloud(pos, sp);
                    
                }
                //inimigo pra trás
                else if ((pos + sp - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {//limite
                    
                        pos += sp;
                        dustcloud.CreateCloud(pos, sp);
                    
                }
                //inimigo direita
                if ((Vector3.Cross(sp, camara.viewMatrix.Up) + pos - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {
                    sp = Vector3.Transform(sp, Matrix.CreateRotationY(+0.025f));
                }
                //inimigo esquerda
                else if ((-Vector3.Cross(sp, camara.viewMatrix.Up) + pos - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {
                    sp = Vector3.Transform(sp, Matrix.CreateRotationY(-0.025f));
                }



                Vector3[] normals = terrain.GetNormalsFromXZ((int)pos.X, (int)pos.Z);                           //normais dos pontos onde o tank se encontra

                Vector3 NAB = ((((int)pos.Z + 1) - pos.Z) * normals[0] + (pos.Z - (int)pos.Z) * normals[1]);    //
                Vector3 NCD = ((((int)pos.Z + 1) - pos.Z) * normals[2] + (pos.Z - (int)pos.Z) * normals[3]);    //Interpolação das normais
                Vector3 normal = ((((int)pos.X + 1) - pos.X) * NAB + (pos.X - ((int)pos.X)) * NCD);             //
                normal.Normalize();                                                                             //


                rotation = Matrix.Identity;
                Vector3 dirH = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationY(yaw));                  //
                dirH.Normalize();                                                                               //
                Vector3 right = Vector3.Cross(dirH, normal);                                                    //
                right.Normalize();                                                                              //
                Vector3 dir = Vector3.Cross(normal, right);                                                     //Vetores axiais para calculo da rotação do tank
                dir.Normalize();                                                                                //
                rotation.Forward = dir;                                                                         //
                rotation.Up = normal;                                                                           //
                rotation.Right = right;                                                                         //

                if (pos.X < terrain.vertices[0].Position.X)                                                     //
                    pos.X = terrain.vertices[0].Position.X;                                                     //
                if (pos.Z < terrain.vertices[0].Position.Z)                                                     //
                    pos.Z = terrain.vertices[0].Position.Z;                                                     //Limitação do tank no terreno
                if (pos.X > terrain.vertices[terrain.vertices.Length - 1].Position.X)                           //
                    pos.X = terrain.vertices[terrain.vertices.Length - 1].Position.X - 0.0001f;                 //
                if (pos.Z > terrain.vertices[terrain.vertices.Length - 1].Position.Z)                           //
                    pos.Z = terrain.vertices[terrain.vertices.Length - 1].Position.Z - 0.0001f;                 //

                Vector3[] vectors = terrain.GetVerticesFromXZ((int)pos.X, (int)pos.Z);                          //
                float YA = vectors[0].Y;                                                                        //
                float YB = vectors[1].Y;                                                                        //
                float YC = vectors[2].Y;                                                                        //Interpolação e calculo da posição no eixo do Y do tank
                float YD = vectors[3].Y;                                                                        //
                float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);                      //
                float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);                      //
                pos.Y = ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD);                      //

                Matrix translation = Matrix.CreateTranslation(pos);                                             //Translação do tank através da sua posição

                model.Root.Transform = Matrix.CreateScale(scale) * rotation * translation;                      //Atualização da posição, rotação e escala da matriz do tank
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
                dustcloud.Update();                                                                             //
            }
            if (mode == PlayerMode.PC)
            {
                if (keyboard.IsKeyDown(Keys.Space)){
                        Fire(content);    
                }


                if (keyboard.IsKeyDown(Keys.A)){                                                                                                //
                    yaw += 4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }                                                                                                //Calculo do yaw
                if (keyboard.IsKeyDown(Keys.D)){                                                                                                //
                    yaw -= 4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }                                                                                                //

                rotation = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);                                     //Rotação para movimentaçao do tank

                Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotation);                                      //Vetor de direção do tank a partir da rotação
                Vector3 sp = new Vector3(0.1f, 0.01f, 0f);
                if (keyboard.IsKeyDown(Keys.W)){                                                                                               //
                    pos = pos - dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                     //Movimentação do tank
                    dustcloud.CreateCloud(pos, sp);
                }
                if (keyboard.IsKeyDown(Keys.S)){                                                                                               //
                    pos = pos + dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                     //
                    dustcloud.CreateCloud(pos, sp);
                }
                if (pos.X < terrain.vertices[0].Position.X)                                                     //
                    pos.X = terrain.vertices[0].Position.X;                                                     //
                if (pos.Z < terrain.vertices[0].Position.Z)                                                     //
                    pos.Z = terrain.vertices[0].Position.Z;                                                     //Limitação do tank no terreno
                if (pos.X > terrain.vertices[terrain.vertices.Length - 1].Position.X)                           //
                    pos.X = terrain.vertices[terrain.vertices.Length - 1].Position.X - 0.0001f;                 //
                if (pos.Z > terrain.vertices[terrain.vertices.Length - 1].Position.Z)                           //
                    pos.Z = terrain.vertices[terrain.vertices.Length - 1].Position.Z - 0.0001f;                 //

                Vector3[] vectors = terrain.GetVerticesFromXZ((int)pos.X, (int)pos.Z);                          //
                float YA = vectors[0].Y;                                                                        //
                float YB = vectors[1].Y;                                                                        //
                float YC = vectors[2].Y;                                                                        //
                float YD = vectors[3].Y;                                                                        //Interpolação e calculo da posição no eixo do Y do tank
                float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);                      //
                float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);                      //
                pos.Y = ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD);                      //

                Vector3[] normals = terrain.GetNormalsFromXZ((int)pos.X, (int)pos.Z);                           //
                Vector3 NAB = ((((int)pos.Z + 1) - pos.Z) * normals[0] + (pos.Z - (int)pos.Z) * normals[1]);    //
                Vector3 NCD = ((((int)pos.Z + 1) - pos.Z) * normals[2] + (pos.Z - (int)pos.Z) * normals[3]);    //Interpolação das normais
                Vector3 normal = ((((int)pos.X + 1) - pos.X) * NAB + (pos.X - ((int)pos.X)) * NCD);             //
                normal.Normalize();                                                                             //

                Vector3 dirH = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationY(yaw));                  //
                dirH.Normalize();                                                                               //
                Vector3 right = Vector3.Cross(dirH, normal);                                                    //
                right.Normalize();                                                                              //
                dir = Vector3.Cross(normal, right);                                                             //
                dir.Normalize();                                                                                //Vetores axiais para calculo da rotação do tank
                rotation = Matrix.Identity;                                                                     //
                rotation.Forward = dir;                                                                         //
                rotation.Up = normal;                                                                           //
                rotation.Right = right;                                                                         //
                translation = Matrix.CreateTranslation(pos);                                             //Translação do tank através da sua posição

                model.Root.Transform = Matrix.CreateScale(scale) * rotation * translation;                      //Atualização da posição, rotação e escala da matriz do tank

                if (keyboard.IsKeyDown(Keys.Left))                                                                                              //
                   turretRot += 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;    //
                if (keyboard.IsKeyDown(Keys.Right))                                                                                             //Rotação da torre do tank
                    turretRot -= 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
                if (keyboard.IsKeyDown(Keys.Up))                                                                                                //
                    cannonRot -= 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;                                                                                                             //
                if (keyboard.IsKeyDown(Keys.Down))                                                                                              //Rotação do canhão do tank
                    cannonRot += 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;                                                                                                             //
                cannonRot = MathHelper.Clamp(cannonRot, -45, 25);                                                                               //
                cannonBone.Transform = Matrix.CreateRotationX(cannonRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * cannonTransform;      //
                turretBone.Transform = Matrix.CreateRotationY(turretRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * turretTransform;
                
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
                dustcloud.Update();                                                                                                             //
                if (colisao(pos, otherTank))
                {
                    pos -= new Vector3(0.1f, 0.0f, 0.1f);
                    System.Diagnostics.Debug.WriteLine("a fazer");
                }
            }
        }
        public bool colisao(Vector3 pos, Tank tank2)
        {
            if (pos.Length() - tank2.pos.Length() < 10)
            {
                
                return true;
            }
            System.Diagnostics.Debug.WriteLine("NAO fazer");
            return false;
        }


        public void Fire(ContentManager c)//change position to look like coming from cannon
        {
            Bullets b = new Bullets(c, pos, terrain, cannonRot, turretTransform * Matrix.CreateRotationY(yaw), tankangle);
            bullets.Add(b);
        }




        public void Draw(Camera camera, GraphicsDevice device)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects){
                   // effect.World = worldMatrix;
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
                    //effect.Projection = camera.projection;
                    effect.Projection = camera.projection;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.DiffuseColor = camera.effect.DirectionalLight0.DiffuseColor;
                    effect.DirectionalLight0.SpecularColor = camera.effect.DirectionalLight0.SpecularColor;
                    effect.DirectionalLight0.Direction = camera.effect.DirectionalLight0.Direction;
                    effect.AmbientLightColor = camera.effect.AmbientLightColor;
                }
                mesh.Draw();
            }
            dustcloud.DrawCloud(device, camera, terrain.worldMatrix);   
            foreach (Bullets b in bullets)
            {
                b.Draw(camera);
            }

        }

      
    }


}
