using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities.Weapons
{
    public abstract class Weapon
    {
        public string Tag { get; set; }
        public Dictionary<string, List<Rectangle>> HitboxSizes { get; protected set; }
        public Vertices vertices { get; protected set; }

        public abstract void CreateHitboxSizes();

        public Rectangle GetCurrentHitbox(string animationName, int frameIndex)
        {
            return HitboxSizes[animationName][frameIndex];
        }

        //final method that actually work in real code
        public abstract Vertices findSwordVertices(Vector2 position);

    }

}
