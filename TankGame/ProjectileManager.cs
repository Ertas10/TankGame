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
        Model projectileModel;                                          //Modelo dos projeteis
        public List<Projectile> projectiles;                            //Lista de projeteis
        public List<Projectile> deadProjectiles;                        //Projeteis mortos
        ClsPlaneTextureIndexStripVB ground;                             //Terreno

        public ProjectileManager(Model model, ClsPlaneTextureIndexStripVB ground){
            this.projectileModel = model;
            this.projectiles = new List<Projectile>();
            this.deadProjectiles = new List<Projectile>();
            this.ground = ground;
        }

        public void Update(GameTime gameTime, CollisionManager collManager){
            foreach (Projectile proj in projectiles){
                proj.Update(gameTime);                                  //Atualiza o projetil
                proj.dead = collManager.ProjectileCollision(proj);      //Verifica se colide com os tanks e mata-o se sim
                if (proj.dead)                                          //Se está morto, adiciona à lista de projeteis mortos
                    deadProjectiles.Add(proj);
            }
            foreach(Projectile proj in deadProjectiles)
                projectiles.Remove(proj);                               //Remove projeteis mortos da lista
            deadProjectiles.Clear();                                    //Limpa lista de projeteis mortos
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
