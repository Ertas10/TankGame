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
        float Speed = 5;
        Vector3 pos;
        Model model;
        ClsPlaneTextureIndexStripVB terrain;
        BasicEffect effect;
        float aspectRatio;
        //Bones
        ModelBone turretBone, cannonBone;
        //matrixes
        Matrix worldMatrix;
        //Default Transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        //Keeps all transforms
        Matrix[] boneTransforms;

        public Tank(Model model, ClsPlaneTextureIndexStripVB terrain, Vector3 startingPos, GraphicsDevice graphicsDevice) {
            this.model = model;
            this.terrain = terrain;
            worldMatrix = Matrix.CreateWorld(new Vector3(15, 15, 15), Vector3.Forward, Vector3.Up);

            pos = startingPos;
            effect = new BasicEffect(graphicsDevice);
            aspectRatio = (float)graphicsDevice.Viewport.Width /(float) graphicsDevice.Viewport.Height;


            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            boneTransforms = new Matrix[model.Bones.Count];

            model.Root.Transform = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(1.5f) * Matrix.CreateRotationX(0.2f) * Matrix.CreateTranslation(new Vector3(30f, 6f, 30f));

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            

        }

        public void Update(KeyboardState keyboard, GameTime gameTime)
        {


            Vector3 dir = Vector3.Transform(-Vector3.UnitZ, worldMatrix);
            
            Vector3 right = Vector3.Cross(dir, Vector3.Up);

            if (keyboard.IsKeyDown(Keys.W))
            {                                               
                pos = pos + dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;   
            }                                                                                   
            if (keyboard.IsKeyDown(Keys.S))
            {                                               
                pos = pos - dir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;   
            }                                                                                   
            if (keyboard.IsKeyDown(Keys.A))
            {                                               
                 
            }                                                                                   
            if (keyboard.IsKeyDown(Keys.D))
            {                                               
                 
            }

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
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), aspectRatio, 1, 1000);
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

        }

      
    }


}
