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
    public partial class Spawner:RectangleItem
    {
        
        public Spawner()
        {

        }
    }

    public partial class PlayerStart : Spawner
    {
    
         //   this.FillColor = Color.GhostWhite;
        //    this.FillColor.A = 155;
     
      

         public PlayerStart()
        {

        }
    }

    public partial class NPCStart : Spawner
    {
      
        //    this.FillColor = Color.Chocolate;
      //      this.FillColor.A = 155;
      
        public NPCStart()
        {

        }
     
    }

    
}
