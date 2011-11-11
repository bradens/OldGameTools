using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Drawing.Imaging;
using GLEED2D.Properties;

namespace GLEED2D
{

    class Brush
    {
        public String fullpath;
        public Texture2D texture;
        public enum Type
        {
            texture, item
        }
        public Type currentType;

        public Item itemObj;

        public Brush(Item itemObject)
        {
            this.itemObj = itemObject;
            this.currentType = Type.item;
            this.fullpath = this.itemObj.texturePath;
            this.itemObj.texture_fullpath = this.fullpath;
            this.texture = TextureLoader.Instance.FromFile(Game1.Instance.GraphicsDevice, this.fullpath);
            this.itemObj.texture = this.texture;
            this.itemObj.init();
        }

        public Brush(String fullpath)
        {
            this.fullpath = fullpath;
            this.currentType = Type.texture;
            this.texture = TextureLoader.Instance.FromFile(Game1.Instance.GraphicsDevice, this.fullpath);
        }
    }



}
