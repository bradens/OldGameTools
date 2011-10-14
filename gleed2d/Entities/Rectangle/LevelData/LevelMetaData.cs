using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GLEED2D
{
    public class LevelMetaData:CircleItem
    {

        public Vector2 gravity;

        [DisplayName("Gravity"), Category(" Level properties")]
        [XmlIgnore()]
       
        public Vector2 Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }
        public LevelMetaData(Vector2 startpos, float radius)
            : base(startpos, radius)
        {
            gravity = new Vector2(0, 100);
        }
    }
}
