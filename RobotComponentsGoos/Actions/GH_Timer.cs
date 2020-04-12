﻿// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Timer Goo wrapper class, makes sure Timer can be used in Grasshopper.
    /// </summary>
    public class GH_Timer : GH_GeometricGoo<Timer>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Timer()
        {
            this.Value = new Timer();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="timer"> Timer Value to store inside this Goo instance. </param>
        public GH_Timer(Timer timer)
        {
            if (timer == null)
                timer = new Timer();
            this.Value = timer;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="timerGoo"> TimerGoo to store inside this Goo instance. </param>
        public GH_Timer(GH_Timer timerGoo)
        {
            if (timerGoo == null)
                timerGoo = new GH_Timer();
            this.Value = timerGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the TimerGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateTimerGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the TimerGoo. </returns>
        public GH_Timer DuplicateTimerGoo()
        {
            return new GH_Timer(Value == null ? new Timer() : Value.Duplicate());
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// ets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Timer instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Timer instance: Did you define a duration?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null Timer";
            else
                return "Timer";
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Timer"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a Timer."); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                return BoundingBox.Empty; //Note: beef this up if needed
            }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty; //Note: beef this up if needed
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to Timer
            if (typeof(Q).IsAssignableFrom(typeof(Timer)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to TimerGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Timer)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Timer(Value);
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to ActionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Action(Value);
                return true;
            }

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Number(Value.Duration);
                return true;
            }

            target = default(Q);
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to source of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Timer
            if (typeof(Timer).IsAssignableFrom(source.GetType()))
            {
                Value = source as Timer;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is Timer action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is Timer action)
                {
                    Value = action;
                    return true;
                }
            }

            // Cast from number
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                GH_Number ghNumber = (GH_Number)source;
                Value = new Timer(ghNumber.Value);
                return true;
            }
            return false;
        }
        #endregion

        #region transformation methods
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region drawing methods
        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }
        #endregion
    }
}
