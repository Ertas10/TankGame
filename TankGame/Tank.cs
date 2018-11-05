using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Tank
    {
        Model model;
        Matrix worldMatrix;
        ClsPlaneTextureIndexStripVB terrain;

        public Tank(Model model, ClsPlaneTextureIndexStripVB terrain, Vector3 startingPos) {
            this.model = model;
            this.terrain = terrain;
            worldMatrix = Matrix.CreateWorld(new Vector3(15, 15, 15), Vector3.Forward, Vector3.Up);
            
        }

        public void Draw(Camera camera){
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects){
                    effect.World = worldMatrix;
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projection;
                }
                mesh.Draw();
            }

        }
    }
}
