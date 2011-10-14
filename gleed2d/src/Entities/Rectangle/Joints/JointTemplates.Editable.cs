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
    public interface IJointTemplate
    {
    }

    public partial class RevoluteAngleJointTemplate :CircleItem
    {

     
      //      this.CustomProperties.Add("Body1", new CustomProperty("Body1", null, typeof(Item), "first body"));
    //        this.CustomProperties.Add("Body2", new CustomProperty("Body2", null, typeof(Item), "second body"));
      
        public RevoluteAngleJointTemplate()
         
        {
        }

    }

    public partial class SliderJointTemplate :PathItem
    {
       
            //this.LineColor = Color.PaleGreen;
            //this.LineColor.A = 128;
            //this.CustomProperties.Add("Body1", new CustomProperty("Body1", null, typeof(Item), "first body"));
            //this.CustomProperties.Add("Body2", new CustomProperty("Body2", null, typeof(Item), "second body"));
            //this.CustomProperties.Add("SliderMode", new CustomProperty("SliderMode", false, typeof(bool), "SliderMode"));
            //this.CustomProperties.Add("MinDistance", new CustomProperty("MinDistance", "0", typeof(string), "MinDistance"));
            //this.CustomProperties.Add("MaxDistance", new CustomProperty("MaxDistance", "0", typeof(string), "MinDistance"));

        

      
        public SliderJointTemplate()
        {
        }
    }



    public partial class AngleJointTemplate : RevoluteAngleJointTemplate
    {

        //    this.FillColor = Color.Yellow;
        //    this.FillColor.A = 128;
        //    //dont forget to convert to radians
        //    this.CustomProperties.Add("LimitJoint", new CustomProperty("LimitJoint", false, typeof(bool), "Get/set limit joint mode."));

        //    this.CustomProperties.Add("TargetAngle", new CustomProperty("TargetAngle", "0", typeof(string), "Get/set the angle in degrees that the body/bodies will attempt to achieve."));
        //    //this.CustomProperties.Add("MaxImpulse", new CustomProperty("MaxImpulse", null, typeof(float), "Get/set the maximum torque the body/bodies will use in attempting to achieve the TargetAngle. Defaults to the highest floating point number possible."));
        //    this.CustomProperties.Add("LowerLimit", new CustomProperty("LowerLimit", "0", typeof(string), "Get/set the minimum angle, in degrees, of the body."));
        //    this.CustomProperties.Add("UpperLimit", new CustomProperty("UpperLimit", "0", typeof(string), "Get/set the maximum angle, in degrees, of the body."));
        //}

        public AngleJointTemplate()
          
        {
        }



    }

    public partial class RevoluteJointTemplate : RevoluteAngleJointTemplate
    {
        public RevoluteJointTemplate()
           
        {
           
        }
       
            //this.FillColor = Color.Green;
            //this.FillColor.A = 128;
       
     
    }

    public partial class SpringTemplate
    {
    
            //this.LineColor = Color.Orange;
            //this.LineColor.A = 128;
            //this.CustomProperties.Add("Body1", new CustomProperty("Body1", null, typeof(Item), "first body"));
            //this.CustomProperties.Add("Body2", new CustomProperty("Body2", null, typeof(Item), "second body"));
            //this.CustomProperties.Add("AngleMode", new CustomProperty("AngleMode", false, typeof(bool), "AngleMode"));
            //this.CustomProperties.Add("SpringConstant", new CustomProperty("SpringConstant", "0", typeof(string), "SpringConstant"));
            //this.CustomProperties.Add("DampingConstant", new CustomProperty("DampingConstant", "0", typeof(string), "DampingConstant"));


     

        public SpringTemplate()
          
        {
        }
    }

    public partial class ChainTrackTemplate 
    {
        
           
        //    this.CustomProperties.Add("LinkType", new CustomProperty("LinkType", "0", typeof(string), "LinkType")); //0 revolute-joint
        //    this.CustomProperties.Add("LinkWidth", new CustomProperty("LinkWidth", "0", typeof(string), "LinkWidth"));
        //    this.CustomProperties.Add("LinkHeight", new CustomProperty("LinkHeight", "0", typeof(string), "LinkHeight"));
        //    this.CustomProperties.Add("LinkMass", new CustomProperty("LinkMass", "0", typeof(string), "LinkMass"));
        //    //  _track = ComplexFactory.Instance.CreateTrack(PhysicsSimulator, _controlPoints, 20.0f, 10.0f, 3.0f, true, 2, LinkType.RevoluteJoint);
        //    //_chain = ComplexFactory.Instance.CreateChain(PhysicsSimulator, new Vector2(150, 100), new Vector2(200, 300), 20.0f, 10.0f, 1, true, false, LinkType.RevoluteJoint);
          
        //}

        public ChainTrackTemplate()
           
        { }

    }

    public partial class ChainTemplate : ChainTrackTemplate
    {
      
            //this.LineColor = Color.White;
            //this.LineColor.A = 128;
            //this.CustomProperties.Add("PinStart", new CustomProperty("PinStart", false, typeof(bool), "PinStart"));
            //this.CustomProperties.Add("PinEnd", new CustomProperty("PinEnd", false, typeof(bool), "PinEnd"));

        
        
        public ChainTemplate()
          
        { }
     

    }

    public partial class TrackTemplate : ChainTrackTemplate
    {
      
            //this.LineColor = Color.White;
            //this.LineColor.A = 128;
          //  this.CustomProperties.Add("PinStart", new CustomProperty("PinStart", false, typeof(bool), "PinStart"));
      //      this.CustomProperties.Add("PinEnd", new CustomProperty("PinEnd", false, typeof(bool), "PinEnd"));

        

        public TrackTemplate()
           
        { }
     
    }

}
