using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame
{
    class DustParticle
    {
        VertexPositionColor[] vertices = new VertexPositionColor[2];

        public int ttl;
        private Vector3 velocity;

        public DustParticle(Vector3 position1, int life, Vector3 velocity)
        {
            vertices[0] = new VertexPositionColor(position1, Color.SandyBrown);
            vertices[1] = new VertexPositionColor(position1 - position1 * 0.0001f, Color.SandyBrown);

            this.ttl = life;
            this.velocity = velocity;
        }

        public void Update()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                ttl--;
                vertices[i].Position += velocity;
                vertices[i].Position.Y += 0.01f;
            }
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
