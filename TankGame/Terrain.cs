using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    class Terrain
    {
        Texture2D heightMap;
        Texture2D terrainTexture;

        public Terrain(Texture2D heightMap, Texture2D terrainTexture){
            this.terrainTexture = terrainTexture;
            this.heightMap = heightMap;
        }

        public Terrain(){ }

        private void BuildTerrain(){
            Color[] greyValues = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData(greyValues);
        }

        public float GetAltura(Vector3 pos){
            
            return 0f;
        }
    }
}
