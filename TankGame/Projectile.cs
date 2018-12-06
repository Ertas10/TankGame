using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Projectile
    {
        int ParentId;
        Model model;
        Matrix[] boneTransforms;
        Vector3 horizontal;
        Vector3 vertical;
        Vector3 pos;
        ClsPlaneTextureIndexStripVB ground;

        public Projectile(Model model, Vector3 horizontalAngle, Vector3 verticalAngle, Vector3 pos, ClsPlaneTextureIndexStripVB ground, int ParentID){
            this.model = model;
            this.boneTransforms = new Matrix[model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            this.horizontal = horizontalAngle;
            this.vertical = verticalAngle;
            this.ground = ground;
            this.pos = pos;
            this.ParentId = ParentID;
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(Camera camera, GraphicsDevice device){
            foreach(ModelMesh mesh in model.Meshes){
                foreach(BasicEffect effect in mesh.Effects){
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
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
