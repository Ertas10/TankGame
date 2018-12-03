using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TankGame
{
    class GenerateParticle
    {
        private Vector3 center, particleL, particleR;
        Random rnd = new Random(DateTime.Now.Millisecond);
        private float radiuscloud = 0.75f;
        private int density;
        BasicEffect effect;
        List<DustParticle> dustL = new List<DustParticle>();
        List<DustParticle> dustR = new List<DustParticle>();

        public GenerateParticle(GraphicsDevice device, Vector3 position)
        {
            effect = new BasicEffect(device);
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;

            this.center = position;
        }

        public void CreateCloud(Vector3 position, Vector3 speed, GraphicsDevice device)
        {
            density = (int)(radiuscloud * 30);

            for (int i = 0; i < density; i++)
            {
                do
                {
                    particleL = new Vector3((rnd.Next(-density, density) * radiuscloud) / density + position.X, position.Y,
                        (rnd.Next(-density, density) * radiuscloud) / density + position.Z + 1f);

                    particleR = new Vector3((rnd.Next(-density, density) * radiuscloud) / density + position.X, position.Y,
                        (rnd.Next(-density, density) * radiuscloud) / density + position.Z - 1f);
                }
                while (Vector3.Distance(particleL, position) > radiuscloud || Vector3.Distance(particleR, position) > radiuscloud);
                int ttl = 500 + rnd.Next(100);

                dustL.Add(new DustParticle(device, particleL, ttl, speed));
                dustR.Add(new DustParticle(device, particleR, ttl, speed));
            }
        }

        public void Update()
        {

            for (int m = 0; m < dustL.Count; m++)
            {
                dustL[m].Update();
                if (dustL[m].ttl <= 0)
                {
                    dustL.RemoveAt(m);
                    m--;
                }
            }

            for (int m = 0; m < dustR.Count; m++)
            {
                dustR[m].Update();
                if (dustR[m].ttl <= 0)
                {
                    dustR.RemoveAt(m);
                    m--;
                }
            }
        }

        public void DrawCloud(GraphicsDevice device, Camera camera, Matrix worldMatrix)
        {
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projection;

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (DustParticle d in dustL)
                d.Draw(device, camera);

            foreach (DustParticle d in dustR)
                d.Draw(device, camera);
        }
    }
}
