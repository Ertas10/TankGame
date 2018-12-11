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
        public CollisionManager(){

        }

        public void Collision(Tank tank1, Tank tank2){
            if(Vector3.Distance(tank1.pos, tank2.pos) < (tank1.colRadius + tank2.colRadius)){
                float tangent = (float)(Math.Sqrt(Math.Pow(tank1.pos.X - tank2.pos.X, 2f)) / Math.Sqrt(Math.Pow(tank1.pos.Z - tank2.pos.Z, 2f)));
                float angle1 = MathHelper.ToDegrees((float)Math.Atan(tangent));
                float hypotenuse = (float)Math.Sqrt(Math.Pow(Vector3.Distance(tank1.pos, tank2.pos) - (tank1.colRadius + tank2.colRadius),2));
                float movX = (float)(Math.Sin(angle1) * hypotenuse) / 2f;
                float movZ = (float)(Math.Cos(angle1) * hypotenuse) / 2f;
                if(tank1.pos.X < tank2.pos.X){
                    tank1.pos.X -= movX;
                    tank2.pos.X += movX;
                }
                else{
                    tank1.pos.X += movX;
                    tank2.pos.X -= movX;
                }
                if(tank1.pos.Z < tank2.pos.Z){
                    tank1.pos.Z -= movZ;
                    tank2.pos.Z += movZ;
                }
                else{
                    tank1.pos.Z += movZ;
                    tank2.pos.Z -= movZ;
                }
            }
        }
    }
}
