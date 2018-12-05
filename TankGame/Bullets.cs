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
    class Bullets
    {
        Model model;
        Matrix world;
        Matrix[] boneTransforms;
        Matrix rotation;
        float scale = 0.025f;
        public Vector3 position;
        Vector3 dir;
        ClsPlaneTextureIndexStripVB Ground;
        float velocityI = 10f;
        Vector3 velocity;
        float gravity = -9.8f;
        float time = 0;

        public Bullets(ContentManager content, Vector3 position, ClsPlaneTextureIndexStripVB ground, float angle, Matrix viewMatrix, float tankangle)
        {
            model = content.Load<Model>("rain");
            world = Matrix.Identity;
            this.Ground = ground;
            this.position = position;

            dir = viewMatrix.Forward;
            rotation = viewMatrix * Matrix.CreateRotationY(1.5708f);
            rotation *= Matrix.CreateFromAxisAngle(rotation.Right, -angle);
            rotation *= Matrix.CreateRotationY(tankangle);

            dir = rotation.Forward;

            velocity = velocityI * dir;

            boneTransforms = new Matrix[model.Bones.Count];
            this.position.Y += 1f;
            model.Root.Transform = Matrix.CreateScale(scale) * world;
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }

        public void Update(GameTime gameTime)
        {
            velocity.Y += gravity * time;//fix?
            position += velocity * time;

            world = Matrix.CreateTranslation(position);
            model.Root.Transform = Matrix.CreateScale(scale) * world;
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public bool HitGround(Vector3 position, float limitwall)
        {
            //limit control
            if (position.X > limitwall - 1 || position.Z < -(limitwall - 1) || position.X < 0 || position.Z > 0)
            {
                return true;
            }

            //ground hit control
            Vector3 A = Ground.vertices[(int)position.X + (int)position.Z * (-1) * Ground.terreno.Width].Position;
            Vector3 B = Ground.vertices[(int)position.X + (int)position.Z * (-1) * Ground.terreno.Width + 1].Position;
            Vector3 C = Ground.vertices[(int)position.X + ((int)position.Z * (-1) + 1) * Ground.terreno.Width].Position;
            Vector3 D = Ground.vertices[(int)position.X + 1 + ((int)position.Z * (-1) + 1) * Ground.terreno.Width].Position;

            float dxa = position.X - A.X;
            float dxb = 1 - dxa;
            float ydx1 = A.Y * dxb + B.Y * dxa;
            float ydx2 = C.Y * dxb + D.Y * dxa;

            float dzcd = (position.Z * (-1) - B.Z * (-1));
            float dzcd2 = 1 - dzcd;

            float limitY = ydx2 * dzcd + ydx1 * dzcd2;

            if (position.Y <= limitY)
            {
                return true;
            }
            return false;
        }

        public void Draw(Camera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projection;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }
    }
}
