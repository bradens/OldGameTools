using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace GLEED2D
{
    public partial class Level
    {
        /// <summary>
        /// The name of the level.
        /// </summary>
        [XmlAttribute()]
        public String Name;

        [XmlAttribute()]
        public bool Visible;

        /// <summary>
        /// A Level contains several Layers. Each Layer contains several MapObjects.
        /// </summary>
        public List<Layer> Layers;

        /// <summary>
        /// A Dictionary containing any user-defined Properties.
        /// </summary>
        public SerializableDictionary CustomProperties;

        public Rectangle bounds;

        public Level()
        {
            Visible = true;
            Layers = new List<Layer>();
            CustomProperties = new SerializableDictionary();
        }

        public static Level FromFile(string filename, ContentManager cm)
        {
            FileStream stream = File.Open(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Level));
            Level level = (Level)serializer.Deserialize(stream);
            stream.Close();

            foreach (Layer layer in level.Layers)
            {
                foreach (MapObject item in layer.MapObjects)
                {
                    item.CustomProperties.RestoreItemAssociations(level);
                    item.load(cm);
                }
            }

            return level;
        }

        public static Level FromFile(string filename)
        {
            FileStream stream = File.Open(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Level));
            Level level = (Level)serializer.Deserialize(stream);
            stream.Close();

           

            return level;
        }


        public MapObject getItemByName(string name)
        {
            foreach (Layer layer in Layers)
            {
                foreach (MapObject item in layer.MapObjects)
                {
                    if (item.Name == name) return item;
                }
            }
            return null;
        }

        public Layer getLayerByName(string name)
        {
            foreach (Layer layer in Layers)
            {
                if (layer.Name == name) return layer;
            }
            return null;
        }

        public void draw(SpriteBatch sb)
        {
            foreach (Layer layer in Layers) layer.draw(sb);
        }


    }


    public partial class Layer
    {
        /// <summary>
        /// The name of the layer.
        /// </summary>
        [XmlAttribute()]
        public String Name;

        /// <summary>
        /// Should this layer be visible?
        /// </summary>
        [XmlAttribute()]
        public bool Visible;

        /// <summary>
        /// The list of the items in this layer.
        /// </summary>
        public List<MapObject> MapObjects;

        /// <summary>
        /// The Scroll Speed relative to the main camera. The X and Y components are 
        /// interpreted as factors, so (1;1) means the same scrolling speed as the main camera.
        /// Enables parallax scrolling.
        /// </summary>
        public Vector2 ScrollSpeed;

        /// <summary>
        /// A Dictionary containing any user-defined Properties.
        /// </summary>
        public SerializableDictionary CustomProperties;


        public Layer()
        {
            MapObjects = new List<MapObject>();
            ScrollSpeed = Vector2.One;
            CustomProperties = new SerializableDictionary();
        }

        public void draw(SpriteBatch sb)
        {
            if (!Visible) return;
            foreach (MapObject item in MapObjects) item.draw(sb);
        }

    }

    //DEFAULT ITEMS
    [XmlInclude(typeof(TileObject))]
    [XmlInclude(typeof(CollisionRectangle))]
    [XmlInclude(typeof(CircleItem))]
    [XmlInclude(typeof(PathItem))]
    [XmlInclude(typeof(Item))]
    //CUSTOM ITEMS
    [XmlInclude(typeof(PObjectTemplate))]
    [XmlInclude(typeof(RevoluteJointTemplate))]
    [XmlInclude(typeof(AngleJointTemplate))]
    [XmlInclude(typeof(SliderJointTemplate))]
    [XmlInclude(typeof(SpringTemplate))]
    [XmlInclude(typeof(PlayerStart))]
    [XmlInclude(typeof(NPCStart))]
    [XmlInclude(typeof(PointLightTemplate))]

    
    public partial class MapObject
    {
        /// <summary>
        /// The name of this item.
        /// </summary>
        [XmlAttribute()]
        public String Name;

        public int Id;

        /// <summary>
        /// Should this item be visible?
        /// </summary>
        [XmlAttribute()]
        public bool Visible;

        /// <summary>
        /// The item's position in world space.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// A Dictionary containing any user-defined Properties.
        /// </summary>
        public SerializableDictionary CustomProperties;


        public MapObject()
        {
            CustomProperties = new SerializableDictionary();
        }

        /// <summary>
        /// Called by Level.FromFile(filename) on each MapObject after the deserialization process.
        /// Should be overriden and can be used to load anything needed by the MapObject (e.g. a texture).
        /// </summary>
        public virtual void load(ContentManager cm)
        {
        }

        public virtual void draw(SpriteBatch sb)
        {
        }
    }

    public partial class TileObject : MapObject
    {
        /// <summary>
        /// The item's rotation in radians.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The item's scale vector.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The color to tint the item's texture with (use white for no tint).
        /// </summary>
        public Color TintColor;

        /// <summary>
        /// If true, the texture is flipped horizontally when drawn.
        /// </summary>
        public bool FlipHorizontally;

        /// <summary>
        /// If true, the texture is flipped vertically when drawn.
        /// </summary>
        public bool FlipVertically;

        /// <summary>
        /// The path to the texture's filename (including the extension) relative to ContentRootFolder.
        /// </summary>
        public String texture_filename;

        /// <summary>
        /// The texture_filename without extension. For using in Content.Load<Texture2D>().
        /// </summary>
        public String asset_name;

        /// <summary>
        /// The XNA texture to be drawn. Can be loaded either from file (using "texture_filename") 
        /// or via the Content Pipeline (using "asset_name") - then you must ensure that the texture
        /// exists as an asset in your project.
        /// Loading is done in the MapObject's load() method.
        /// </summary>
        [XmlIgnore()]
        public Texture2D texture;

        /// <summary>
        /// The item's origin relative to the upper left corner of the texture. Usually the middle of the texture.
        /// Used for placing and rotating the texture when drawn.
        /// </summary>
        public Vector2 Origin;

        [XmlIgnore()]
        public bool isTemplate;

        public TileObject()
        {
        }

        /// <summary>
        /// Called by Level.FromFile(filename) on each MapObject after the deserialization process.
        /// Loads all assets needed by the TileObject, especially the Texture2D.
        /// You must provide your own implementation. However, you can rely on all public fields being
        /// filled by the level deserialization process.
        /// </summary>
        public override void load(ContentManager cm)
        {
            base.load(cm);
            //throw new NotImplementedException();

            //TODO: provide your own implementation of how a TileObject loads its assets
            //for example:
            //this.texture = Texture2D.FromFile(<GraphicsDevice>, texture_filename);
            //or by using the Content Pipeline:
            //this.texture = cm.Load<Texture2D>(asset_name);

        }

        public override void draw(SpriteBatch sb)
        {
            if (!Visible) return;
            SpriteEffects effects = SpriteEffects.None;
            if (FlipHorizontally) effects |= SpriteEffects.FlipHorizontally;
            if (FlipVertically) effects |= SpriteEffects.FlipVertically;
            sb.Draw(texture, Position, null, TintColor, Rotation, Origin, Scale, effects, 0);
        }
    }

    public partial class Item : TileObject
    {
        public String iconPath;
        public String texturePath;
        public SerializableDictionary attributes;
        public String description;
        public String itemName;

        public Item() : base()
        {
            this.Scale = Vector2.One;
            this.TintColor = Color.White;
            FlipHorizontally = FlipVertically = false;
            polygon = new Vector2[4];
        }
        public Item(String textureFile, String iconFile, SerializableDictionary atts, String description, Vector2 position, String Name) 
            : base(fullpath: textureFile, position: position)
        {
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
            this.coldata = new Color[texture.Width * texture.Height];
            texture.GetData(coldata);
            polygon = new Vector2[4];
            OnTransformed();
        }

        public void init()
        {
            this.coldata = new Color[texture.Width * texture.Height];
            texture.GetData(coldata);
        }

        public override MapObject clone()
        {
            Item result = (Item)this.MemberwiseClone();
            result.CustomProperties = new SerializableDictionary(CustomProperties);
            result.attributes = new SerializableDictionary(attributes);
            result.polygon = (Vector2[])polygon.Clone();
            result.hovering = false;
            return result;
        }

        public Item cloneItem()
        {
            Item result = (Item)this.MemberwiseClone();
            result.CustomProperties = new SerializableDictionary(CustomProperties);
            result.attributes = new SerializableDictionary(attributes);
            result.polygon = (Vector2[])polygon.Clone();
            result.hovering = false;
            return result;
        }
    }


    public partial class CollisionRectangle : MapObject
    {
        public float Width;
        public float Height;
        public Color FillColor;

        public CollisionRectangle()
        {
        }
    }



    public partial class CircleItem : MapObject
    {
        public float Radius;
        public Color FillColor;

        public CircleItem()
        {
        }
    }


    public partial class PathItem : MapObject
    {
        public Vector2[] LocalPoints;
        public Vector2[] WorldPoints;
        public bool IsPolygon;
        public int LineWidth;
        public Color LineColor;

        public PathItem()
        {
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
    //
    //    NEEDED FOR SERIALIZATION. YOU SHOULDN'T CHANGE ANYTHING BELOW!
    //
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////


    public class CustomProperty
    {
        public string name;
        public object value;
        public Type type;
        public string description;

        public CustomProperty()
        {
        }

        public CustomProperty(string n, object v, Type t, string d)
        {
            name = n;
            value = v;
            type = t;
            description = d;
        }

        public CustomProperty clone()
        {
            CustomProperty result = new CustomProperty(name, value, type, description);
            return result;
        }
    }


    public class SerializableDictionary : Dictionary<String, CustomProperty>, IXmlSerializable
    {

        public SerializableDictionary()
            : base()
        {

        }

        public SerializableDictionary(SerializableDictionary copyfrom)
            : base(copyfrom)
        {
            string[] keyscopy = new string[Keys.Count];
            Keys.CopyTo(keyscopy, 0);
            foreach (string key in keyscopy)
            {
                this[key] = this[key].clone();
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty) return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                CustomProperty cp = new CustomProperty();
                cp.name = reader.GetAttribute("Name");
                cp.description = reader.GetAttribute("Description");

                string type = reader.GetAttribute("Type");
               
                if (type == "string") cp.type = typeof(string);
                if (type == "bool") cp.type = typeof(bool);
                if (type == "Vector2") cp.type = typeof(Vector2);
                if (type == "Color") cp.type = typeof(Color);
                if (type == "MapObject") cp.type = typeof(MapObject);

                if (cp.type == typeof(MapObject))
                {
                    cp.value = reader.ReadInnerXml();
                    this.Add(cp.name, cp);
                }
                else
                {
                    reader.ReadStartElement("Property");
                    XmlSerializer valueSerializer = new XmlSerializer(cp.type);
                    object obj = valueSerializer.Deserialize(reader);
                    cp.value = Convert.ChangeType(obj, cp.type);
                    this.Add(cp.name, cp);
                    reader.ReadEndElement();
                }

                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (String key in this.Keys)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", this[key].name);
                if (this[key].type == typeof(string)) writer.WriteAttributeString("Type", "string");
                if (this[key].type == typeof(bool)) writer.WriteAttributeString("Type", "bool");
                if (this[key].type == typeof(Vector2)) writer.WriteAttributeString("Type", "Vector2");
                if (this[key].type == typeof(Color)) writer.WriteAttributeString("Type", "Color");
                if (this[key].type == typeof(MapObject)) writer.WriteAttributeString("Type", "MapObject");
             
                writer.WriteAttributeString("Description", this[key].description);

                if (this[key].type == typeof(MapObject))
                {
                    MapObject item = (MapObject)this[key].value;
                    if (item != null) writer.WriteString(item.Name);
                    else writer.WriteString("$null$");
                }
                else
                {
                    XmlSerializer valueSerializer = new XmlSerializer(this[key].type);
                    valueSerializer.Serialize(writer, this[key].value);
                }
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Must be called after all MapObjects have been deserialized. 
        /// Restores the MapObject references in CustomProperties of type MapObject.
        /// </summary>
        public void RestoreItemAssociations(Level level)
        {
            foreach (CustomProperty cp in Values)
            {
                if (cp.type == typeof(MapObject)) cp.value = level.getItemByName((string)cp.value);
            }
        }
    }

    [XmlRoot("dictionary")]
    public class SerializableGenericDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}