using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Entities.Weapons;
using StardenRPG.Entities.RPGsystem;
using StardenRPG.Entities.ItemDrop;
using StardenRPG.Entities.Bar;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Common.TextureTools;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using Microsoft.VisualBasic;
using tainicom.Aether.Physics2D.Collision;
using StardenRPG.Entities.Monster;
using StardenRPG.Entities.Bar;
using System.Formats.Asn1;

namespace StardenRPG.Entities
{
    public class Tag : Body
    {
        public string TagName { get; set; }

        public Tag(/*World world,*/ string tag)
        {
            TagName = tag;
        }
    }
}
