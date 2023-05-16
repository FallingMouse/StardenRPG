using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardenRPG.Entities.Weapons
{
    public class Sword : Weapon
    {
        public Sword()
        {
            Tag = "Sword";
            CreateHitboxSizes();
        }

        public override void CreateHitboxSizes()
        {
            HitboxSizes = new Dictionary<string, List<Rectangle>>
            {
                { "PlayerAttack", new List<Rectangle> {
                    new Rectangle(0 + 110, 256 + 106, 24, 18),
                    new Rectangle(288 + 111, 256 + 104, 15, 17),
                    new Rectangle(576+ 122, 256 + 88, 11, 35),
                    new Rectangle(864 + 118, 256 + 69, 46, 20),
                    new Rectangle(1152 + 166, 256 + 44, 46, 83),
                    new Rectangle(1440 + 169, 256 + 62, 41, 65),
                    new Rectangle(1728 + 169, 256 + 70, 41, 57),
                    new Rectangle(2016 + 168, 256 + 113, 39, 14),
                    new Rectangle(2304 + 127, 256 + 105, 13, 22),
                    new Rectangle(2592 + 100, 256 + 107, 37, 18),
                } },
                // Add more animations as needed
            };
        }
    }

}
