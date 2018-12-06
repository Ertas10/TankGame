using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class ProjectileManager
    {
        Model projectileModel;
        List<Projectile> projectiles;
        ClsPlaneTextureIndexStripVB ground;

        public ProjectileManager(Model model, ClsPlaneTextureIndexStripVB ground){
            this.projectileModel = model;
            this.projectiles = new List<Projectile>();
            this.ground = ground;
        }

        public void Update(GameTime gameTime){
            foreach (Projectile proj in projectiles){
                proj.Update(gameTime);
            }
        }

        public void AddProjectile(Vector3 horizontalAngle, Vector3 verticalAngle, Vector3 pos, int id){
            projectiles.Add(new Projectile(projectileModel, horizontalAngle, verticalAngle, pos, ground, id));
        }

        public void Draw(Camera camera, GraphicsDevice device){
            foreach(Projectile proj in projectiles){
                proj.Draw(camera, device);
            }
        }
    }
}
