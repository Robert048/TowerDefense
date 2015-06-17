﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TowerDefense
{
    class FastEnemy : Enemy
    {
        public FastEnemy(Vector2 position, int waveNumber, Queue<Vector2> waypoints)
            : base()
        {
            startHealth = 4 + (waveNumber * 3);
            currentHealth = startHealth;
            bountyGiven = 2 + (waveNumber * 2);
            speed = 2.5f;
            this.waypoints = waypoints;
            this.position = position;
            waypoint = position;
        }
    }
}
