using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class CollisionManager
    {
        Tank tank1;
        Tank tank2;
        public CollisionManager(Tank tank1, Tank tank2){
            this.tank1 = tank1;
            this.tank2 = tank2;
        }

        public void Collision()
        {
            if (Vector3.Distance(tank1.pos, tank2.pos) < (tank1.colRadius + tank2.colRadius))
            {
                float tangent = (float)(Math.Sqrt(Math.Pow(tank1.pos.X - tank2.pos.X, 2f)) / Math.Sqrt(Math.Pow(tank1.pos.Z - tank2.pos.Z, 2f)));       //tangente para encontrar o angulo
                float angle1 = (float)Math.Atan(tangent);                                                                                               //angulo, em radianos, a partir de tangent
                float hypotenuse = (float)Math.Sqrt(Math.Pow(Vector3.Distance(tank1.pos, tank2.pos) - (tank1.colRadius + tank2.colRadius), 2f)) / 2f;   //hipotenusa para calcular movX e movZ
                float movX = (float)(Math.Sin(angle1) * hypotenuse);                                                                                    //movimento que será aplicado no x para não colidir os tanks
                float movZ = (float)(Math.Cos(angle1) * hypotenuse);                                                                                    //movimento que será aplicado no z para não colidir os tanks
                if (tank1.pos.X < tank2.pos.X){
                    tank1.pos.X -= movX;
                    tank2.pos.X += movX;
                }
                else{
                    tank1.pos.X += movX;
                    tank2.pos.X -= movX;
                }
                if (tank1.pos.Z < tank2.pos.Z){
                    tank1.pos.Z -= movZ;
                    tank2.pos.Z += movZ;
                }
                else{
                    tank1.pos.Z += movZ;
                    tank2.pos.Z -= movZ;
                }
            }
        }

        public bool ProjectileCollision(Projectile proj) {
            if (proj.ParentId != tank1.Id && Vector3.Distance(tank1.pos, proj.pos) < (tank1.colRadius + proj.radius))                                   //testa colisão entre o projétil e o tank1
                return true;
            
                
            if (proj.ParentId != tank2.Id && Vector3.Distance(tank2.pos, proj.pos) < (tank2.colRadius + proj.radius))                                   //testa colisão entre o projétil e o tank2
                if (proj.ParentId != tank1.Id && Vector3.Distance(tank1.pos, proj.pos) < (tank1.colRadius + proj.radius))
                    return true;
            return false;                                                                                                                               //se não colide
        }
    }
}
