using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Text;
using System.ComponentModel;


namespace GLEED2D
{
    public partial class Layer
    {

        [XmlIgnore()]
        [DisplayName("ScrollSpeed"), Category(" General")]
        [Description("The Scroll Speed relative to the main camera. The X and Y components are interpreted as factors, " +
        "so Vector2.One means same scrolling speed as the main camera. To be used for parallax scrolling.")]
        public Vector2 pScrollSpeed
        {
            get
            {
                return ScrollSpeed;
            }
            set
            {
                ScrollSpeed = value;
            }
        }


        [XmlIgnore]
        public Level level;

        public Layer(String name) : this()
        {
            this.Name = name;
            this.Visible = true;
        }

        public Layer clone()
        {
            Layer result = (Layer)this.MemberwiseClone();
            result.MapObjects = new List<MapObject>(MapObjects);
            for (int i = 0; i < result.MapObjects.Count; i++)
            {
                result.MapObjects[i] = result.MapObjects[i].clone();
                result.MapObjects[i].layer = result;
            }
            return result;
        }



        public MapObject getItemAtPos(Vector2 mouseworldpos)
        {
            for (int i = MapObjects.Count - 1; i >= 0; i--)
            {
                if (MapObjects[i].contains(mouseworldpos) && MapObjects[i].Visible) return MapObjects[i];
            }
            return null;
        }

        public void drawInEditor(SpriteBatch sb)
        {
            if (!Visible) return;
            foreach (MapObject item in MapObjects) item.drawInEditor(sb);
        }



    }
}
