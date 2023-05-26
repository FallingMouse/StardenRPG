using Microsoft.Xna.Framework;
using StardenRPG.Entities.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.TextureTools;
using tainicom.Aether.Physics2D.Dynamics;

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
        //old code, maybe can be used later
        /*public override void findSwordVertices(Body swordBody, Vector2 position)
         {
             //Vertices to define each point of sword hitbox
             {
                 Vertices vertices = new Vertices(4);

                 //not the real position, just try if it actually work
                 vertices.Add(new Vector2(position.X, position.Y - 0.5f));
                 vertices.Add(new Vector2(position.X, position.Y  + 1.0f));
                 vertices.Add(new Vector2(position.X - 3f, position.Y -0.5f));
                 vertices.Add(new Vector2(position.X - 3f, position.Y + 1.0f));

                 PolygonShape chassis = new PolygonShape(vertices, 2);

                 swordBody.CreateFixture(chassis);
                 swordBody.BodyType = BodyType.Dynamic;
             }
         }*/

        public override Vertices findSwordVertices(Vector2 position)
        {
            //Vertices to define each point of sword hitbox
            vertices = new Vertices(4);
            {
                //not the real position, just try if it actually work
                vertices.Add(new Vector2(position.X, position.Y - 0.5f));
                vertices.Add(new Vector2(position.X, position.Y + 1.0f));
                vertices.Add(new Vector2(position.X - 3f, position.Y - 0.5f));
                vertices.Add(new Vector2(position.X - 3f, position.Y + 1.0f));

                //PolygonShape chassis = new PolygonShape(vertices, 2);

                //swordBody.CreateFixture(chassis);
                //wordBody.BodyType = BodyType.Dynamic;   
            }
            return vertices;
        }

        //old code, maybe can be used later
        /*public override void findSwordVertices(Body swordBody)
        {
            //Vertices to define each point of sword hitbox
            {
                Vertices vertices = new Vertices(4);

                //not the real position, just try if it actually work
                vertices.Add(new Vector2(-2.5f, 0f));
                vertices.Add(new Vector2(-2.5f, 3f));
                vertices.Add(new Vector2(-1.5f, 3f));
                vertices.Add(new Vector2(-1.5f, 0f));

                PolygonShape chassis = new PolygonShape(vertices, 2);

                swordBody.CreateFixture(chassis);
                swordBody.BodyType = BodyType.Dynamic;
            }
        }*/
    }
}
