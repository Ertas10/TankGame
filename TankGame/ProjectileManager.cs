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
        public List<Projectile> projectiles;
        public List<Projectile> deadProjectiles;
        ClsPlaneTextureIndexStripVB ground;

        public ProjectileManager(Model model, ClsPlaneTextureIndexStripVB ground){
            this.projectileModel = model;
            this.projectiles = new List<Projectile>();
            this.deadProjectiles = new List<Projectile>();
            this.ground = ground;
        }

        public void Update(GameTime gameTime, CollisionManager collManager){
            foreach (Projectile proj in projectiles){
                proj.Update(gameTime);
                proj.dead = collManager.ProjectileCollision(proj);
                if (proj.dead)
                    deadProjectiles.Add(proj);
            }
            foreach(Projectile proj in deadProjectiles){
                projectiles.Remove(proj);
            }
            deadProjectiles.Clear();
        }

        public void AddProjectile(Vector3 pos, Vector3 dirH, float dirAngle, int id){
            projectiles.Add(new Projectile(projectileModel, pos, dirH, dirAngle, ground, id));
        }

        public void Draw(Camera camera, GraphicsDevice device){
            foreach(Projectile proj in projectiles){
                proj.Draw(camera, device);
            }
        }
    }
}
