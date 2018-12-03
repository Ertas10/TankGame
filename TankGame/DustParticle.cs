using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame
{
    class DustParticle
    {
        VertexPositionColor[] vertices;

        public int ttl;
        private Vector3 velocity;
        TimeSpan lifeTime;
        DateTime lifeStart;
        BasicEffect effect;
        Matrix worldMatrix;

        public DustParticle(GraphicsDevice device, Vector3 position1, int life, Vector3 velocity)
        {
            this.effect = new BasicEffect(device);
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            lifeStart = DateTime.Now;
            vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(position1, Color.SandyBrown);
            vertices[1] = new VertexPositionColor(position1 - position1 * 0.0001f, Color.SandyBrown);

            this.ttl = life;
            lifeTime = new TimeSpan(0, 0, 0, 0, life);
            this.velocity = velocity;
        }

        public void Update()
        {
            if((DateTime.Now - lifeStart).Milliseconds >= lifeTime.Milliseconds)
                ttl = 0;
            vertices[0].Position += velocity;
            vertices[0].Position.Y += 0.01f;
            vertices[1].Position += velocity;
            vertices[1].Position.Y += 0.01f;
            //Debug.Print(lifeTime.ToString());
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            effect.World = worldMatrix;
            effect.View = camera.viewMatrix;
            effect.Projection = camera.projection;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
