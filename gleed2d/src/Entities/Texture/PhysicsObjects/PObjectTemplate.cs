using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
//using FarseerGames.FarseerPhysics.Collisions;

namespace GLEED2D
{
    public class PObjectTemplate : TextureItem
    {
        [DisplayName("Mass"), Category(" Physical properties")]
        [XmlIgnore()]
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }


        //Physical properties
        public float mass;
        public float friction;
        public float restitution;
        public float torque;
        public float angularImpulse;
        public Vector2 linearVelocity;
        public float angularVelocity;
        public Vector2 force;
        public Vector2 impulse;
        public Boolean hires;

        [DisplayName("Hi-res"), Category(" Performance properties")]
        [XmlIgnore()]
        public Boolean Hires
        {
            get { return hires; }
            set { hires = value; }
        }
        [DisplayName("AngularVelocity"), Category(" Physical properties")]
        [XmlIgnore()]
        public float AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }
        [DisplayName("LinearVelocity"), Category(" Physical properties")]
        [XmlIgnore()]
        public Vector2 LinearVelocity
        {
            get { return linearVelocity; }
            set { linearVelocity = value; }
        }



        [DisplayName("AngularImpulse"), Category(" Physical properties")]
        [XmlIgnore()]
        public float AngularImpulse
        {
            get { return angularImpulse; }
            set { angularImpulse = value; }
        }

        [DisplayName("Impulse"), Category(" Physical properties")]
        [XmlIgnore()]
        public Vector2 Impulse
        {
            get { return impulse; }
            set { impulse = value; }
        }

        [DisplayName("Force"), Category(" Physical properties")]
        [XmlIgnore()]

        public Vector2 Force
        {
            get { return force; }
            set { force = value; }
        }

        public int objectType; //1-rectangle,2-circle, anything else - from texture
        public bool isStatic;

        [DisplayName("Static"), Category(" Physical properties")]
        [XmlIgnore()]
        public bool IsStatic
        {
            get { return isStatic; }
            set { isStatic = value; }
        }
        public int collisionGroup;

        [DisplayName("Collision group"), Category(" Physical properties")]
        [XmlIgnore()]
        public int CollisionGroup
        {
            get { return collisionGroup; }
            set { collisionGroup = value; }
        }
        //bool isSensor,



        [DisplayName("Friction"), Category(" Physical properties")]
        [XmlIgnore()]
        public float Friction
        {
            get { return friction; }
            set { friction = value; }
        }

        [DisplayName("Restitution"), Category(" Physical properties")]
        [XmlIgnore()]
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        [DisplayName("Torque"), Category(" Physical properties")]
        [XmlIgnore()]
        public float Torque
        {
            get { return torque; }
            set { torque = value; }
        }

        [DisplayName("Object Type"), Category(" Physical properties")]
        [XmlIgnore()]
        public int ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }


        public PObjectTemplate()
            : base()
        {

        }

        public override string getNamePrefix()
        {
            if (isStatic)
                return "SO_";
            else
                return "DO_";
        }

        public PObjectTemplate(string fullpath, Vector2 position)
            : base(fullpath, position)
        {
            //default settings
            mass = 10;
            friction = 2f;
            restitution = 0f;
            torque = 0;
            angularImpulse = 0;
            force = Vector2.Zero;
            linearVelocity = Vector2.Zero;
            angularVelocity = 0;// Vector2.Zero;
            impulse = Vector2.Zero;
            objectType = 0; //1-rectangle,2-circle, anything else - from texture
            isStatic = false;
            collisionGroup = 0;
            hires = false;





        }


    }
}

