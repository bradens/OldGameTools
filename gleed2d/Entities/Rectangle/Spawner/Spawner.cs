﻿using System;
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
    public class Spawner:CollisionRectangle
    {
        public Spawner(Rectangle rect)
            : base(rect)
        {

        }
        public Spawner():base()
        {

        }
    }

    public class PlayerStart : Spawner
    {
         public PlayerStart(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.GhostWhite;
            this.FillColor.A = 155;
        }
         public override string getNamePrefix()
         {
             return "PlayerStart_";// base.getNamePrefix();
         }

         public PlayerStart():base()
        {

        }
    }
    public class NPCStart : Spawner
    {
          public NPCStart(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Chartreuse;
            this.FillColor.A = 155;
        }

          public override string getNamePrefix()
          {
              return "NPCStart_";// base.getNamePrefix();
          }


          public NPCStart()
              : base()
        {

        }
    }

    
}
