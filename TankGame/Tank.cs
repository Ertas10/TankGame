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
     class Tank
    {
        float Speed = 3f;
        Vector3 pos;
        Model model;
        ClsPlaneTextureIndexStripVB terrain;
        BasicEffect effect;
        float yaw = 0;
        //Bones
        ModelBone turretBone, cannonBone;
        //matrixes
        float scale = 0.005f;
        //Default Transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        //Keeps all transforms
        Matrix[] boneTransforms;

        public Tank(Model model, ClsPlaneTextureIndexStripVB terrain, Vector3 startingPos, GraphicsDevice graphicsDevice) {
            this.model = model;
            this.terrain = terrain;

            pos = startingPos;
            effect = new BasicEffect(graphicsDevice);


            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            boneTransforms = new Matrix[model.Bones.Count];

            model.Root.Transform = Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(startingPos);

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            

        }

        public void Update(KeyboardState keyboard, GameTime gameTime)
        {
            if (keyboard.IsKeyDown(Keys.A))
                yaw += 4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboard.IsKeyDown(Keys.D))
                yaw -= 4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);

            Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotation);

            Vector3 right = Vector3.Cross(dir, Vector3.Up);

            if (keyboard.IsKeyDown(Keys.W))
                pos = pos - dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboard.IsKeyDown(Keys.S))
                pos = pos + dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (pos.X < terrain.vertices[0].Position.X)                                         //
                pos.X = terrain.vertices[0].Position.X;                                         //
            if (pos.Z < terrain.vertices[0].Position.Z)                                         //
                pos.Z = terrain.vertices[0].Position.Z;                                         //
            if (pos.X > terrain.vertices[terrain.vertices.Length - 1].Position.X)               //
                pos.X = terrain.vertices[terrain.vertices.Length - 1].Position.X - 0.0001f;     //
            if (pos.Z > terrain.vertices[terrain.vertices.Length - 1].Position.Z)               //
                pos.Z = terrain.vertices[terrain.vertices.Length - 1].Position.Z - 0.0001f;     //

            Vector3[] vectors = terrain.GetVerticesFromXZ((int)pos.X, (int)pos.Z);              //
            float YA = vectors[0].Y;                                                            //
            float YB = vectors[1].Y;                                                            //
            float YC = vectors[2].Y;                                                            //
            float YD = vectors[3].Y;                                                            //
            float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);          //
            float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);          //
            pos.Y = ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD);          //

            Vector3[] normals = terrain.GetNormalsFromXZ((int)pos.X, (int)pos.Z);

            Vector3 NAB = ((((int)pos.Z + 1) - pos.Z) * normals[0] + (pos.Z - (int)pos.Z) * normals[1]);
            Vector3 NCD = ((((int)pos.Z + 1) - pos.Z) * normals[2] + (pos.Z - (int)pos.Z) * normals[3]);
            Vector3 normal = ((((int)pos.X + 1) - pos.X) * NAB + (pos.X - ((int)pos.X)) * NCD);
            normal.Normalize();

            Vector3 dirH = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationY(yaw));
            dirH.Normalize();

            right = Vector3.Cross(dirH, normal);
            right.Normalize();
            dir = Vector3.Cross(normal, right);
            dir.Normalize();
            rotation = Matrix.Identity;
            rotation.Forward = dir;
            rotation.Up = normal;
            rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(pos);

            model.Root.Transform = Matrix.CreateScale(scale) * rotation * translation;
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }
        public void Draw(Camera camera)
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

        }

      
    }


}
