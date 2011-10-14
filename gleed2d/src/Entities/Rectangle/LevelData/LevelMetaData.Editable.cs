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

namespace Entities
{
    public partial class LevelMetaData:RectangleItem
    {

        public Vector2 gravity;

        [DisplayName("Gravity"), Category(" Level properties")]
        [XmlIgnore()]
       
        public Vector2 Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }
        public LevelMetaData()
        {
            gravity = new Vector2(0, 100);
        }
    }
}
