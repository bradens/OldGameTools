﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GLEED2D
{
    class TextureLoader
    {

        private static TextureLoader instance;
        public static TextureLoader Instance
        {
            get
            {
                if (instance == null) instance = new TextureLoader();
                return instance;
            }
        }

        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();



        public Texture2D FromFile(GraphicsDevice gd, string filename)
        {
            if (!textures.ContainsKey(filename))
            {
                //TextureCreationParameters tcp = TextureCreationParameters.Default;
                //tcp.Format = SurfaceFormat.Color;
                //tcp.ColorKey = Constants.Instance.ColorTextureTransparent;
                FileStream stream = new FileStream(filename, FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
                textures[filename] = Texture2D.FromStream(gd, stream);
                stream.Close();
            }
            return textures[filename];
        }

        public void Clear()
        {
            textures.Clear();
        }

    }
}
