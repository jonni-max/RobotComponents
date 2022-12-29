﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Generators;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents a Wait for Digital Input instruction.
    /// This action is used to wait until a digital input is set.
    /// </summary>
    [Serializable()]
    public class WaitDI : Action, IInstruction, ISerializable
    {
        #region fields
        private string _name; // The name of the digital input signal
        private bool _value; // The desired state / value of the digital input signal
        private double _maxTime;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitDI(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _value = (bool)info.GetValue("Value", typeof(bool));
            _maxTime = version > 103000 ? (double)info.GetValue("Max Time", typeof(double)) : -1;
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Value", _value, typeof(bool));
            info.AddValue("Max Time", _maxTime, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Wait DI class.
        /// </summary>
        public WaitDI()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Wait DI class.
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="value"> Specifies whether the Digital Input is enabled.</param>
        /// <param name="maxTime"> The maximum time to wait in seconds. </param>
        public WaitDI(string name, bool value, double maxTime = -1)
        {
            _name = name;
            _value = value;
            _maxTime = maxTime;
        }

        /// <summary>
        /// Initializes a new instance of the Wait DI class by duplicating an existing Wait DI instance. 
        /// </summary>
        /// <param name="waitDI"> The Wait DI instance to duplicate. </param>
        public WaitDI(WaitDI waitDI)
        {
            _name = waitDI.Name;
            _value = waitDI.Value;
            _maxTime = waitDI.MaxTime;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance.
        /// </summary>
        /// <returns> A deep copy of the Wait DI instance. </returns>
        public WaitDI Duplicate()
        {
            return new WaitDI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Wait DI instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitDI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Wait Di instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new WaitDI(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (_name == null)
            {
                return "Empty Wait for Digital Input";
            }
            if (!IsValid)
            {
                return "Invalid Wait for Digital Input";
            }
            else
            {
                return $"Wait for Digital Input ({_name}\\{_value})";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return $"WaitDI {_name}, {(_value ? 1 : 0)}" +
                $"{(_maxTime > 0 ? $"\\MaxTime:={_maxTime:0.###}" : "")};";
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot)); 
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets or sets the desired state of the digital input signal.
        /// </summary>
        public bool Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the name of the digital input signal.
        /// </summary>
        public string Name 
        { 
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets te max. time to wait in seconds. Set a negative value to wait for ever (default is -1).
        /// </summary>
        public double MaxTime
        {
            get { return _maxTime; }
            set { _maxTime = value; }
        }
        #endregion
    }

}
