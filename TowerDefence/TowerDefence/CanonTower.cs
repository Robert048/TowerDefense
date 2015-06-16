﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense
{
    class CanonTower : Tower
    {
        public CanonTower(Texture2D tower, Texture2D canon, Vector2 position)
        {
            this.position = position;
            damage = 20;
            attackSpeed = 2000;
            range = 150;
            ground = true;
            air = false;
            cost = 40;
            projectileList = new List<Projectile>();
            texture = tower;
            projectileTexture = canon;
            towerString = "canonTower";
        }
    }
}
