using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class ParticleSystem
    {
        Random random;
        float lifeTime;                 //Tempo de vida das particulas
        int particlesPerCycle;          //Número de particulas instanciadas de uma vez
        List<Particle> particles;       //Lista de particulas em execução
        List<Particle> deadParticles;   //Lista de particulas a remover da lista anterior
        Color color;                    //Cor das particulas
        ClsPlaneTextureIndexStripVB terreno;

        public ParticleSystem(Color particleColor, float particleLifeTime, int particlesPerCycle, ClsPlaneTextureIndexStripVB terreno)
        {
            this.terreno = terreno;
            this.random = new Random(DateTime.Now.Millisecond);
            this.lifeTime = particleLifeTime;
            this.color = particleColor;
            this.particlesPerCycle = particlesPerCycle;
            this.particles = new List<Particle>();
            this.deadParticles = new List<Particle>();
        }

        public void AddParticles(GraphicsDevice device, Vector3 position, Vector3 direction){
                particles.Add(new Particle(device, position, direction, color, lifeTime));
        }

        public void Update(GameTime gameTime, GraphicsDevice device)
        {
            foreach (Particle part in particles)
            {                                                                           //Dá update a todas as particulas
                part.Update(gameTime, terreno);
                if (part.setToDestroy)                                                                                      //Verifica se a particula a ser executada no momento está pronta para ser destruida
                    deadParticles.Add(part);
            }
            foreach (Particle part in deadParticles)                                                                        //Remove todas as particulas mortas da lista de particulas
                particles.Remove(part);
            deadParticles.Clear();                                                                                          //Limpa as particulas mortas da lista
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            foreach (Particle part in particles)
            {
                part.Draw(device, camera);
            }
        }
    }
}
