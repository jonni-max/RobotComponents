﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Actions
{
    /// <summary>
    /// Represents the SyncMoveOff instruction to end a sequence of synchronized movements.
    /// </summary>
    [Serializable()]
    public class SyncMoveOff : Action, IInstruction, ISyncident, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type for sync identification
        private string _syncident; // the sync identification name
        private double _timeOut;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected SyncMoveOff(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _syncident = (string)info.GetValue("Sync ID", typeof(string));
            _timeOut = version > 103000 ? (double)info.GetValue("Time Out", typeof(double)) : -1;
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
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("Sync ID", _syncident, typeof(string));
            info.AddValue("Time Out", _timeOut, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the SyncMoveOff class.
        /// </summary>
        public SyncMoveOff()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyncMoveOff class.
        /// </summary>
        /// <param name="name"> The name of the synchronization point. </param>
        /// <param name="timeOut"> The max. time to wait for the other program tasks to reach the synchronization point. </param>
        public SyncMoveOff(string name, double timeOut = -1)
        {
            _referenceType = ReferenceType.VAR;
            _syncident = name;
            _timeOut = timeOut;
        }

        /// <summary>
        /// Initializes a new instance of the SyncMoveOff class by duplicating an existing SyncMoveOff instance. 
        /// </summary>
        /// <param name="SyncMoveOff"> The SyncMoveOff instance to duplicate. </param>
        public SyncMoveOff(SyncMoveOff SyncMoveOff)
        {
            _referenceType = SyncMoveOff.ReferenceType;
            _syncident = SyncMoveOff.SyncID;
            _timeOut = SyncMoveOff.TimeOut;
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOff instance.
        /// </summary>
        /// <returns> A deep copy of the SyncMoveOff instance. </returns>
        public SyncMoveOff Duplicate()
        {
            return new SyncMoveOff(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOff instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the SyncMoveOff instance as an IDeclaration. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new SyncMoveOff(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOff instance as ISyncident.
        /// </summary>
        /// <returns> A deep copy of the SyncMoveOff instance as an ISyncident. </returns>
        public ISyncident DuplicateSyncident()
        {
            return new SyncMoveOff(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Tasks instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new SyncMoveOff(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Sync Move Off";
            }
            else
            {
                return "Sync Move Off (" + _syncident + ")";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return $"{Enum.GetName(typeof(ReferenceType), _referenceType)} syncident {_syncident};";
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return $"SyncMoveOff {_syncident}{(_timeOut > 0 ? $"\\TimeOut:={_timeOut:0.###}" : "")};";
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (!RAPIDGenerator.Syncidents.ContainsKey(_syncident))
            {
                RAPIDGenerator.Syncidents.Add(_syncident, this);
                RAPIDGenerator.ProgramDeclarationsMultiMove.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));
            RAPIDGenerator.SynchronizedMovements = false;
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
                if (_syncident == null) { return false; }
                if (_syncident == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the reference type of the syncident.
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the synchronization (meeting) point (syncident).
        /// </summary>
        public string SyncID
        {
            get { return _syncident; }
            set { _syncident = value; }
        }

        /// <summary>
        /// Gets or sets te max. time to wait for the other program tasks to reach the synchronization point.
        /// Set a negative value to wait for ever (default is -1).
        /// </summary>
        public double TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        #endregion
    }
}