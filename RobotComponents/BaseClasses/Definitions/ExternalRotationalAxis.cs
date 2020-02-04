﻿using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses.Definitions
{
    public class ExternalRotationalAxis : ExternalAxis
    {
        #region fields
        private Plane _attachmentPlane; // The plane where the robot or the work object is attached
        private Plane _axisPlane; // Todo: now only the attachment plane is copied
        private Interval _axisLimits; // The movement limits
        private int? _axisNumber; // TODO: The axis logic number
        private Mesh _baseMesh; // The base mesh (fixed)
        private Mesh _linkMesh; // The link mesh posed for axis value 0
        private List<Mesh> _posedMeshes; // The mesh posed for a certain axis value
        #endregion

        #region constructors
        /// <summary>
        /// An empty constuctor that creates an empty external rotational axis.
        /// </summary>
        public ExternalRotationalAxis()
        {
            _linkMesh = new Mesh();
            _baseMesh = new Mesh();
            _posedMeshes = new List<Mesh>();
        }

        /// <summary>
        /// Defines an external rotational axis with empty meshes. 
        /// </summary>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector.  </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        public ExternalRotationalAxis(Plane attachmentPlane, Plane axisPlane, Interval axisLimits)
        {
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; // Todo
            _linkMesh = new Mesh();
            _baseMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external rotational axis with a mesh geometry. 
        /// </summary>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector.  </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external rotational axis. </param>
        /// <param name="linkMesh"> The link mesh of the external rotational axis posed for external axis value 0. </param>
        public ExternalRotationalAxis(Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; // Todo
            _linkMesh = baseMesh;
            _baseMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// A method to duplicate the ExternalRotationalAxis object. 
        /// </summary>
        /// <returns> Returns a deep copy for the ExternalRotationalAxis object. </returns>
        public ExternalRotationalAxis Duplicate()
        {
            ExternalRotationalAxis dup = new ExternalRotationalAxis(AttachmentPlane, AxisPlane, AxisLimits, BaseMesh, LinkMesh);
            return dup;
        }
        #endregion

        #region methods
        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePosition(double axisValue, out bool inLimits)
        {
            // Bool that indicates if the axis value is within the limits
            bool isInLimits;

            // Check if value is within axis limits
            if (axisValue < _axisLimits.Min)
            {
                isInLimits = false;
            }
            else if (axisValue > _axisLimits.Max)
            {
                isInLimits = false;
            }
            else
            {
                isInLimits = true;
            }

            // Transform
            double radians = Rhino.RhinoMath.ToRadians(axisValue);
            Transform orientNow = Transform.Rotation(radians, _axisPlane.ZAxis, _axisPlane.Origin);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(orientNow);

            inLimits = isInLimits;
            return positionPlane;
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePositionSave(double axisValue)
        {
            // Double that will be used to calculate the real pose within the axis limits
            double value;

            // Check if value is within axis limits
            if (axisValue < _axisLimits.Min)
            {
                value = _axisLimits.Min;
            }
            else if (axisValue > _axisLimits.Max)
            {
                value = _axisLimits.Max;
            }
            else
            {
                value = axisValue;
            }

            // Transform
            double radians = Rhino.RhinoMath.ToRadians(value);
            Transform orientNow = Transform.Rotation(radians, _axisPlane.ZAxis, _axisPlane.Origin);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(orientNow);

            return positionPlane;
        }

        /// <summary>
        /// Calculates the position of the external axis mesh for a defined external axis value.
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the meshes for. </param>
        public override void PoseMeshes(double axisValue)
        {
            _posedMeshes.Clear();
            double radians = Rhino.RhinoMath.ToRadians(axisValue);
            Transform rotateNow = Transform.Rotation(radians, _axisPlane.ZAxis, _axisPlane.Origin);
            _posedMeshes.Add(_baseMesh.DuplicateMesh());
            _posedMeshes.Add(_linkMesh.DuplicateMesh());
            _posedMeshes[1].Transform(rotateNow);
        }

        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid external rotational axis object. 
        /// </summary>
        private void Initialize()
        {
            // Nothing to do here at the moment
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid external rotational axis. 
        /// </summary>
        public override void ReInitialize()
        {
            Initialize();
            _posedMeshes.Clear();
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the External Rotational Axis object is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (AttachmentPlane == null) { return false; }
                if (AttachmentPlane == Plane.Unset) { return false; }
                if (AxisPlane == null) { return false;  }
                if (AxisPlane == Plane.Unset) { return false; }
                if (AxisLimits == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The attachment plane of the axis. 
        /// </summary>
        public override Plane AttachmentPlane
        {
            get 
            { 
                return _attachmentPlane; 
            }
            set 
            { 
                _attachmentPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The axis plane. The z-axis of the place rotation center of the axis. 
        /// </summary>
        public override Plane AxisPlane
        {
            get 
            { 
                return _axisPlane; 
            }
            set 
            { 
                _axisPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The rotation limits of the linear axis in degrees. 
        /// </summary>
        public override Interval AxisLimits 
        { 
            get 
            { 
                return _axisLimits; 
            }
            set 
            { 
                _axisLimits = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The logic number of the external axis. 
        /// </summary>
        public override int? AxisNumber
        {
            get { return _axisNumber; }
            set { _axisNumber = value; }
        }

        /// <summary>
        /// The axis movement type.
        /// </summary>
        public override AxisType AxisType 
        { 
            get { return AxisType.ROTATIONAL; }
        }

        /// <summary>
        /// The fixed base mesh of the external axis. 
        /// </summary>
        public override Mesh BaseMesh
        {
            get
            {
                return _baseMesh;
            }
            set
            {
                _baseMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// The movable link mesh of the external axis posed for external axis value 0. 
        /// </summary>
        public override Mesh LinkMesh
        {
            get
            {
                return _linkMesh;
            }
            set
            {
                _linkMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// The external axis mesh posed for a certain external axis value.
        /// </summary>
        public override List<Mesh> PosedMeshes 
        { 
            get { return _posedMeshes; }
        }
        #endregion
    }
}