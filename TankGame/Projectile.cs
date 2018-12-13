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
        public int ParentId;
        Model model;
        Matrix[] boneTransforms;
        Vector3 dirH;
        public Vector3 pos;
        ClsPlaneTextureIndexStripVB ground;
        Matrix worldMatrix;
        Matrix scaleMatrix;

        public bool dead;
        float secondsSinceBirth;
        float totalHeight;
        float prevHeight;
        float hSpeed;
        float dirAngle;
        public float radius;
        float gravity;
        float force;

        public Projectile(Model model, Vector3 pos, Vector3 dirH, float dirAngle, ClsPlaneTextureIndexStripVB ground, int ParentID){
            this.scaleMatrix = Matrix.CreateScale(0.02f);
            this.model = model;
            this.worldMatrix = Matrix.Identity;
            this.model.Root.Transform = scaleMatrix * worldMatrix;
            this.boneTransforms = new Matrix[model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            this.dirH = dirH;
            this.ground = ground;
            this.pos = pos;
            this.ParentId = ParentID;
            this.hSpeed = 0.8f;
            this.secondsSinceBirth = 0;
            this.totalHeight = 0;
            this.prevHeight = 0;
            this.dead = false;
            this.dirAngle = dirAngle;
            this.radius = 0.2f;
            this.gravity = 9.8f;
            this.force = 0.2f;
        }

        public void Update(GameTime gameTime){
            secondsSinceBirth += (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalHeight = force * secondsSinceBirth * MathHelper.ToDegrees((float)Math.Sin(MathHelper.ToRadians(dirAngle))) - (gravity / 2f) * (float)Math.Pow(secondsSinceBirth, 2f);
            pos.X -= dirH.X * hSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.ToDegrees((float)Math.Cos(MathHelper.ToRadians(dirAngle)));
            pos.Z -= dirH.Z * hSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.ToDegrees((float)Math.Cos(MathHelper.ToRadians(dirAngle)));
            pos.Y += totalHeight - prevHeight;
            prevHeight = totalHeight;
            worldMatrix = Matrix.CreateTranslation(pos);
            model.Root.Transform = scaleMatrix * worldMatrix;
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            if (pos.X < ground.vertices[0].Position.X || pos.Z < ground.vertices[0].Position.Z || pos.X > ground.vertices[ground.vertices.Length - 1].Position.X || pos.Z > ground.vertices[ground.vertices.Length - 1].Position.Z){
                dead = true;
                return;
            }
            Vector3[] vectors = ground.GetVerticesFromXZ((int)pos.X, (int)pos.Z);                          //
            float YA = vectors[0].Y;                                                                        //
            float YB = vectors[1].Y;                                                                        //
            float YC = vectors[2].Y;                                                                        //
            float YD = vectors[3].Y;                                                                        //Interpolação e calculo da posição no eixo do Y do tank
            float YAB = ((((int)pos.Z + 1) - pos.Z) * YA + (pos.Z - (int)pos.Z) * YB);                      //
            float YCD = ((((int)pos.Z + 1) - pos.Z) * YC + (pos.Z - (int)pos.Z) * YD);                      //
            if (pos.Y <= ((((int)pos.X + 1) - pos.X) * YAB + (pos.X - ((int)pos.X)) * YCD) || secondsSinceBirth >= 10)
                dead = true;
             
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
