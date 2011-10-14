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
    public class LightTemplate : CircleItem
    {
        public LightTemplate(Vector2 startpos, float radius)
            : base(startpos, radius)
        {
            this.FillColor= Color.White;
            this.FillColor.A = 128;
            
        }
           public LightTemplate()
            : base()
        {
        }

        
    }

    public class PointLightTemplate : LightTemplate
    {
        public PointLightTemplate(Vector2 startpos, float radius)
            : base(startpos, radius)
        {
        }
        public PointLightTemplate()
            : base()
        {
        }

        public override string getNamePrefix()
        {
            return "Light_";
        }
    }
}
