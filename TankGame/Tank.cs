using System;
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
        Vector3 sp, spz, spy;
        //Keeps all transforms
        Matrix[] boneTransforms;
        public float cannonRot = 0f;
        public float turretRot = 0f;
        public float tankangle = 0f;
        public float colRadius = 1.75f;
        GenerateParticle dustcloud;
        public Matrix rotation;
        public Matrix translation;
        KeyboardState prevKB;
        List<Keys> keys;
        public Vector3 dir;
        public int hp;

        public int Id { get => id; }

        public Tank(Model model, ClsPlaneTextureIndexStripVB terrain, Vector3 startingPos, GraphicsDevice graphicsDevice, PlayerMode playermode, int ID, List<Keys> keys, int hp){
            this.hp = hp;
            this.pos = startingPos;                                                                                                         //posição inicial do tank no terreno
            this.mode = playermode;                                                                                                         //indica se o tank está em modo "AI" ou modo controlado por jogador
            this.model = model;                                                                                                             //modelo do tank
            this.model.Root.Transform = Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(startingPos);    //matriz inicial de posição, rotação e escala do tank
            boneTransforms = new Matrix[model.Bones.Count];                                                                                 //bone transforms do tank
            this.model.CopyAbsoluteBoneTransformsTo(boneTransforms);                                                                        //
            this.terrain = terrain;                                                                                                         //terreno ao qual o tank está "bound"
            this.id = ID;
            this.keys = keys;
            turretBone = model.Bones["turret_geo"];                                                                                         //bones da turret
            cannonBone = model.Bones["canon_geo"];                                                                                          //bones do canhão
            turretTransform = turretBone.Transform;                                                                                         //bone transforms da turret
            cannonTransform = cannonBone.Transform;                                                                                         //bone transforms do canhão
            dustcloud = new GenerateParticle(graphicsDevice, pos);
        }

        public void Update(KeyboardState keyboard, GameTime gameTime, Tank otherTank, Camera camara, ProjectileManager projMan)
        {
            ChangeMode(keyboard);
            if(yaw > 2 * Math.PI){
                int div = (int)(yaw / (2 * Math.PI));
                yaw = yaw - (div * 2 * (float)Math.PI);
            }
            if(yaw < 0){
                int div = (int)(yaw / (2 * Math.PI));
                yaw = (float)((2 * Math.PI) - (yaw - (2 * Math.PI * -div)));
            }
            if (mode == PlayerMode.AI){
                float cos = (pos.Z - otherTank.pos.Z) / Vector3.Distance(pos, otherTank.pos);
                float angle = (float)Math.Acos(cos);
                    
                if(angle - yaw < (yaw + 2 * (Math.PI)) - angle){
                    yaw += angle - yaw;
                }
                else{
                    yaw -= ((float)(yaw + 2 * Math.PI) - angle);
                }
                
                Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotation);

                sp = new Vector3(2f, 0, 0);
                spz = new Vector3(0, 0, 2f);
                Vector3 a = dir * sp;

                if (pos.X < otherTank.pos.X)
                {//limite
                    pos += sp * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    dustcloud.CreateCloud(pos, a);
                }
                //inimigo pra trás
                else if (pos.X > otherTank.pos.X)
                {//limite
                    pos -= sp * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    dustcloud.CreateCloud(pos, -a);
                }
                if (pos.Z < otherTank.pos.Z)
                {//limite
                    pos += spz * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    dustcloud.CreateCloud(pos, a);
                }
                //inimigo pra trás
                else if (pos.Z > otherTank.pos.Z)
                {//limite
                    pos -= spz * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    dustcloud.CreateCloud(pos, -a);
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
                float YC = vectors[2].Y;                                                                        //Interpolação e calculo da posição no eixo do Y do tank
                float YD = vectors[3].Y;                                                                        //
                float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);                      //
                float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);                      //
                pos.Y = ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD);                      //

                Vector3[] normals = terrain.GetNormalsFromXZ((int)pos.X, (int)pos.Z);                           //normais dos pontos onde o tank se encontra

                Vector3 NAB = ((((int)pos.Z + 1) - pos.Z) * normals[0] + (pos.Z - (int)pos.Z) * normals[1]);    //
                Vector3 NCD = ((((int)pos.Z + 1) - pos.Z) * normals[2] + (pos.Z - (int)pos.Z) * normals[3]);    //Interpolação das normais
                Vector3 normal = ((((int)pos.X + 1) - pos.X) * NAB + (pos.X - ((int)pos.X)) * NCD);             //
                normal.Normalize();                                                                             //

                rotation = Matrix.Identity;
                Vector3 dirH = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationY(yaw));                                     //
                dirH.Normalize();                                                                               //
                Vector3 right = Vector3.Cross(dirH, normal);                                                    //
                right.Normalize();                                                                              //
                dir = Vector3.Cross(normal, right);                                                     //Vetores axiais para calculo da rotação do tank
                dir.Normalize();                                                                                //
                rotation.Forward = dir;                                                                         //
                rotation.Up = normal;                                                                           //
                rotation.Right = right;
                Matrix translation = Matrix.CreateTranslation(pos);                                             //Translação do tank através da sua posição
                Matrix teste = rotation * Matrix.CreateTranslation(pos);
                
                //inimigo direita
                if ((Vector3.Cross(sp, teste.Up) + pos - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {
                    sp = Vector3.Transform(sp, Matrix.CreateRotationY(+0.025f));
                }
                //inimigo esquerda
                if ((-Vector3.Cross(sp, teste.Up) + pos - otherTank.pos).Length() < (pos - otherTank.pos).Length())
                {
                    sp = Vector3.Transform(sp, Matrix.CreateRotationY(-0.025f));
                }

                cannonBone.Transform = Matrix.CreateRotationX(cannonRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * cannonTransform;
                turretBone.Transform = Matrix.CreateRotationY(turretRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * turretTransform;

                model.Root.Transform = Matrix.CreateScale(scale) * rotation * translation;                      //Atualização da posição, rotação e escala da matriz do tank
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
                dustcloud.Update();                                                                             //
            }
            if (mode == PlayerMode.PC)
            {
                if (keyboard.IsKeyDown(keys[0])){                                                                                                //
                    yaw += (float)Math.PI * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }                                                                                                //Calculo do yaw
                if (keyboard.IsKeyDown(keys[1])){                                                                                                //
                    yaw -= (float)Math.PI * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }                                                                                                //

                rotation = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);                                     //Rotação para movimentaçao do tank

                dir = Vector3.Transform(-Vector3.UnitZ, rotation);                                      //Vetor de direção do tank a partir da rotação
                sp = new Vector3(0.1f, 0.01f, 0f);
                Vector3 a = dir * sp;
                if (keyboard.IsKeyDown(keys[2])){                                                                                               //
                    pos = pos - dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                     //Movimentação do tank
                    dustcloud.CreateCloud(pos, a);
                }
                if (keyboard.IsKeyDown(keys[3])){                                                                                               //
                    pos = pos + dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                     //
                    dustcloud.CreateCloud(pos, -a);
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

                if (keyboard.IsKeyDown(keys[5]))                                                                                              //
                   turretRot += 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;    //
                if (keyboard.IsKeyDown(keys[6]))                                                                                             //Rotação da torre do tank
                    turretRot -= 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;   //
                if (keyboard.IsKeyDown(keys[7]))                                                                                                //
                    cannonRot -= 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;                                                                                                             //
                if (keyboard.IsKeyDown(keys[8]))                                                                                              //Rotação do canhão do tank
                    cannonRot += 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;                                                                                                             //
                cannonRot = MathHelper.Clamp(cannonRot, -45, 25);                                                                               //
                cannonBone.Transform = Matrix.CreateRotationX(cannonRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * cannonTransform;      //
                turretBone.Transform = Matrix.CreateRotationY(turretRot * (float)gameTime.ElapsedGameTime.TotalSeconds) * turretTransform;

                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
                dustcloud.Update();                                                                                                             //
                if (keyboard.IsKeyDown(keys[4]) && !prevKB.IsKeyDown(keys[4])) {
                    Vector3 dirShoot = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationY(yaw + (turretRot * (float)gameTime.ElapsedGameTime.TotalSeconds)));
                    dirShoot.Normalize();
                    Vector3 newRight = Vector3.Cross(dirShoot, normal);
                    newRight.Normalize();
                    Vector3 newDir = Vector3.Cross(normal, right);
                    newDir.Normalize();
                    float shootAngle = MathHelper.ToDegrees((float)Math.Asin(-newDir.Y)) - cannonRot;
                    projMan.AddProjectile(boneTransforms[model.Bones["canon_geo"].Index].Translation, dirShoot, shootAngle, id);
                }
            }
            prevKB = keyboard;
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
        }

        private void ChangeMode(KeyboardState keyboard){
            if (keyboard.IsKeyDown(keys[9]) && !prevKB.IsKeyDown(keys[9]) && mode == PlayerMode.AI){
                mode = PlayerMode.PC;
                return;
            }
            if (keyboard.IsKeyDown(keys[9]) && !prevKB.IsKeyDown(keys[9]) && mode == PlayerMode.PC)
                mode = PlayerMode.AI;
        }
    }
}
