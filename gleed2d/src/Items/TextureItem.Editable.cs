﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Forms = System.Windows.Forms;
using CustomUITypeEditors;
using System.Windows.Forms;
using System.IO;

namespace GLEED2D
{

    public partial class TileObject
    {
        //for per-pixel-collision
        [XmlIgnore()]
        public Color[] coldata;

        Matrix transform;
        Rectangle boundingrectangle;    //bounding rectangle in world space, for collision broadphase

        [XmlIgnore()]
        public Vector2[] polygon;              //selection box: drawn when selected

        [XmlIgnore()]
        public string texture_fullpath;

        [XmlIgnore()]
        [DisplayName("Origin"), Category(" General")]
        [Description("The item's origin in texture space ([0,0] is upper left corner).")]
        public Vector2 pOrigin
        {
            get
            {
                return Origin;
            }
            set
            {
                Origin = value;
                OnTransformed();
            }
        }

        [XmlIgnore()]
        [DisplayName("Rotation"), Category(" General")]
        [Description("The item's rotation in radians.")]
        public float pRotation
        {
            get
            {
                return Rotation;
            }
            set
            {
                Rotation = value;
                OnTransformed();
            }
        }

        [XmlIgnore()]
        [DisplayName("Scale"), Category(" General")]
        [Description("The item's scale vector.")]
        public Vector2 pScale
        {
            get
            {
                return Scale;
            }
            set
            {
                Scale = value;
                OnTransformed();
            }
        }

        [XmlIgnore()]
        [DisplayName("TintColor"), Category(" General")]
        [Editor(typeof(XNAColorUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The Color to tint the texture with. Use white for no tint.")]
        public Color pTintColor
        {
            get
            {
                return TintColor;
            }
            set
            {
                TintColor = value;
            }
        }

        [XmlIgnore()]
        [DisplayName("FlipHorizontally"), Category(" General")]
        [Description("If true, the texture is flipped horizontally when drawn.")]
        public bool pFlipHorizontally
        {
            get { return FlipHorizontally; }
            set { FlipHorizontally = value; }
        }

        [XmlIgnore()]
        [DisplayName("FlipVertically"), Category(" General")]
        [Description("If true, the texture is flipped vertically when drawn.")]
        public bool pFlipVertically
        {
            get { return FlipVertically; }
            set { FlipVertically = value; }
        }
        [XmlIgnore()]
        [DisplayName("IsTemplate"), Category(" General")]
        [Description("Can be used as template for various objects (chains, tracks, spawners), will not be drawn in this exact location")]
        public bool IsTemplate
        {
            get
            {
                return isTemplate;
            }
            set
            {
                isTemplate = value;
            }
        }

        public TileObject(String fullpath, Vector2 position)
            : base()
        {
            this.texture_fullpath = fullpath;
            this.Position = position;
            this.Rotation = 0;
            this.Scale = Vector2.One;
            this.TintColor = Color.White;
            FlipHorizontally = FlipVertically = false;
            loadIntoEditor();
            this.Origin = getTextureOrigin(texture);

            //compensate for origins that are not at the center of the texture
            Vector2 center = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Position -= (center - Origin);

            this.isTemplate = false;

            OnTransformed();
        }

        public override bool loadIntoEditor()
        {
            if (layer != null) this.texture_fullpath = System.IO.Path.Combine(layer.level.ContentRootFolder + "\\", texture_filename);

            if (!File.Exists(texture_fullpath))
            {
                DialogResult dr = Forms.MessageBox.Show("The file \"" + texture_fullpath + "\" doesn't exist!\n"
                    + "The texture path is a combination of the Level's ContentRootFolder and the TileObject's relative path.\n"
                    + "Please adjust the XML file before trying to load this level again.\n"
                    + "For now, a dummy texture will be used. Continue loading the level?", "Error loading texture file",
                    MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error);
                if (dr == DialogResult.No) return false;
                texture = Editor.Instance.dummytexture;
            }
            else
            {
                texture = TextureLoader.Instance.FromFile(Game1.Instance.GraphicsDevice, texture_fullpath);
            }

            //for per-pixel-collision
            coldata = new Color[texture.Width * texture.Height];
            texture.GetData(coldata);

            polygon = new Vector2[4];

            OnTransformed();
            return true;
        }

        public override MapObject clone()
        {
            TileObject result = (TileObject)this.MemberwiseClone();
            result.CustomProperties = new SerializableDictionary(CustomProperties);
            result.polygon = (Vector2[])polygon.Clone();
            result.hovering = false;
            return result;
        }

        public override string getNamePrefix()
        {
            return "Texture_";
        }

        public override void OnTransformed()
        {
            transform =
                Matrix.CreateTranslation(new Vector3(-Origin.X, -Origin.Y, 0.0f)) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            Vector2 leftTop = new Vector2(0, 0);
            Vector2 leftBottom;// = new Vector2(0, width);

            Vector2 rightTop;// = new Vector2(width, 0);

            Vector2 rightBottom;
            if (this.CustomProperties.ContainsKey("Animated"))
            {
                Vector2 dimensions = (Vector2)this.CustomProperties["FrameDimensions"].value;
                leftBottom = new Vector2(0, dimensions.X);

                rightTop = new Vector2(dimensions.X, 0);

                rightBottom = new Vector2(dimensions.X, dimensions.Y);
                column = 0;
                row = 0;
                frameIndex = 0;
            }
            else
            {

                leftBottom = new Vector2(0, texture.Height);

                rightTop = new Vector2(texture.Width, 0);

                rightBottom = new Vector2(texture.Width, texture.Height);
            }
            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            polygon[0] = leftTop;
            polygon[1] = rightTop;
            polygon[3] = leftBottom;
            polygon[2] = rightBottom;

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return as a rectangle
            boundingrectangle = new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));

        }


        public override void onMouseButtonDown(Vector2 mouseworldpos)
        {
            hovering = false;
            MainForm.Instance.pictureBox1.Cursor = Cursors.SizeAll;
            base.onMouseButtonDown(mouseworldpos);
        }


        public override bool CanRotate()
        {
            return true;
        }

        public override float getRotation()
        {
            return pRotation;
        }

        public override void setRotation(float rotation)
        {
            pRotation = rotation;
        }


        public override bool CanScale()
        {
            return true;
        }

        public override Vector2 getScale()
        {
            return pScale;
        }

        public override void setScale(Vector2 scale)
        {
            pScale = scale;
        }



        //for anim support
        int frameIndex = 0;
        int row = 0;
        int column = 0;
        int rows = 0;
        int columns = 0;
        int frameCount = 0;
        float time = 0;
        float frameTime = 0;
        bool reversed = false;
       

        private void ReversedFrameIndex()
        {
            time -= frameTime;
            frameIndex--;

            column--;
           

            if (column <= 0)
            {
                column = columns - 1;
                row--;

                if (row < 0)
                    row = 0;
            }
            if (frameIndex <= 0)
            {
                
                frameIndex = frameCount;
                column = columns - 1;
                row = rows - 1;
            }
        }

        //Compute frame index in forward animations
        private void ComputeFrameIndex()
        {
            time -= frameTime;

            frameIndex++;

            column++;


            if (column >= columns)
            {
                column = 0;
                row++;

            }



            if (frameIndex >= frameCount)
            {
               
                frameIndex=0;    
                    row=0;
                    column = 0;
            }
        }

        public override void drawInEditor(SpriteBatch sb)
        {
            if (!Visible) return;

            SpriteEffects se = SpriteEffects.None;
            if (pFlipHorizontally) se |= SpriteEffects.FlipHorizontally;
            if (pFlipVertically) se |= SpriteEffects.FlipVertically;
            Color c = TintColor;
            if (hovering && Constants.Instance.EnableHighlightOnMouseOver) c = Constants.Instance.ColorHighlight;
            if (this.CustomProperties.ContainsKey("Animated"))
            {
                
                Vector2 dimensions = (Vector2)this.CustomProperties["FrameDimensions"].value;

                this.Origin = new Vector2(dimensions.X / 2, dimensions.Y / 2);
                frameCount = int.Parse((string)this.CustomProperties["FrameCount"].value);
                frameTime = (float)1 / float.Parse((string)this.CustomProperties["FrameRate"].value);
                columns = (int)(texture.Width / dimensions.X);
                rows = (int)(texture.Height / dimensions.Y);
                reversed=(bool)this.CustomProperties["Reversed"].value;

                //  CollisionRectangle temprectangle = new CollisionRectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);
                if (column < 0)
                    column = 0;
                if (row < 0)
                    row = 0;
                time += (float)Game1.Instance.GameTime.ElapsedGameTime.TotalSeconds;
               
                if (time > frameTime)
                {
                    if (!reversed)
                        ComputeFrameIndex();
                    else
                        ReversedFrameIndex();

                }

                Rectangle source = new Rectangle((int)dimensions.X * column, (int)dimensions.Y * row, (int)dimensions.X, (int)dimensions.Y);

                sb.Draw(texture, Position, source, c, Rotation, Origin, Scale, se, 0);

            }
            else
            {
                sb.Draw(texture, Position, null, c, Rotation, Origin, Scale, se, 0);
            }
        }

        public override void drawSelectionFrame(SpriteBatch sb, Matrix matrix, Color color)
        {
            Vector2[] poly = new Vector2[4];
            Vector2.Transform(polygon, ref matrix, poly);

            Primitives.Instance.drawPolygon(sb, poly, color, 2);
            foreach (Vector2 p in poly)
            {
                Primitives.Instance.drawCircleFilled(sb, p, 4, color);
            }
            Vector2 origin = Vector2.Transform(pPosition, matrix);
            Primitives.Instance.drawBoxFilled(sb, origin.X - 5, origin.Y - 5, 10, 10, color);
        }

        public override bool contains(Vector2 worldpos)
        {
            if (boundingrectangle.Contains((int)worldpos.X, (int)worldpos.Y))
            {
                return intersectpixels(worldpos);
            }
            return false;
        }

        public bool intersectpixels(Vector2 worldpos)
        {
            Vector2 positionInB = Vector2.Transform(worldpos, Matrix.Invert(transform));
            int xB = (int)Math.Round(positionInB.X);
            int yB = (int)Math.Round(positionInB.Y);

            if (FlipHorizontally) xB = texture.Width - xB;
            if (FlipVertically) yB = texture.Height - yB;

            // If the pixel lies within the bounds of B
            if (0 <= xB && xB < texture.Width && 0 <= yB && yB < texture.Height)
            {
                Color colorB = coldata[xB + yB * texture.Width];
                if (colorB.A != 0)
                {
                    return true;
                }
            }
            return false;
        }



        public Vector2 getTextureOrigin(Texture2D texture)
        {
            switch (Constants.Instance.DefaultTextureOriginMethod)
            {
                case TextureOriginMethodEnum.TextureCenter:
                    if (this.CustomProperties.ContainsKey("Animated"))
                    {
                        Vector2 dimensions = (Vector2)this.CustomProperties["FrameDimensions"].value;

                        return new Vector2(dimensions.X / 2, dimensions.Y / 2);
                    }
                    return new Vector2(texture.Width / 2, texture.Height / 2);
                case TextureOriginMethodEnum.Centroid:
                    uint[] data = new uint[texture.Width * texture.Height];
                    texture.GetData(data);
                    Vertices verts = Vertices.CreatePolygon(data, texture.Width, texture.Height);
                    return verts.GetCentroid();
                case TextureOriginMethodEnum.TopLeft:
                    return new Vector2(0, 0);
                case TextureOriginMethodEnum.TopRight:
                    return new Vector2(texture.Width, 0);
                case TextureOriginMethodEnum.BottomLeft:
                    return new Vector2(0, texture.Height);
                case TextureOriginMethodEnum.BottomRight:
                    return new Vector2(texture.Width, texture.Height);
            }
            return Vector2.Zero;
        }






    }










}
