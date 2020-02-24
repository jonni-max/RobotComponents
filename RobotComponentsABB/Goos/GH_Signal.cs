﻿// Grasshopper Libs
using Grasshopper.Kernel.Types;
// ABB Robotic Libs
using ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// Signal Goo wrapper class, makes sure Signal can be used in Grasshopper.
    /// </summary>
    public class GH_Signal : GH_Goo<DigitalSignal>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Signal()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="signal"> Signal Value to store inside this Goo instance. </param>
        public GH_Signal(DigitalSignal signal)
        {
            this.Value = signal;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="signalGoo"> SignalGoo to store inside this Goo instance. </param>
        public GH_Signal(GH_Signal signalGoo)
        {
            this.Value = signalGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SignalGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return DuplicateSignalGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SignalGoo. </returns>
        public GH_Signal DuplicateSignalGoo()
        {
            return new GH_Signal(Value);
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
                return true;
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Signal";
            }
            else
            {
                return "SignalName : " + Value.Name + "\nValue : " + Value.Value;
            }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Signal"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get
            {
                return "Defines a ABB Signal";
            }
        }

        #endregion
    }
}