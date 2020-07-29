﻿// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
using static System.Math;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Enumerations;

namespace RobotComponents.Definitions
{
    public class ExternalRotationalAxis : ExternalAxis
    {
        #region fields
        private string _name; // The name of the external axis
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
            _name = "";
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();
        }

        /// <summary>
        /// Defines an external rotational axis with empty meshes. 
        /// </summary>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        public ExternalRotationalAxis(Plane axisPlane, Interval axisLimits)
        {
            _name = "";
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            _attachmentPlane = new Plane(_axisPlane); //TODO: for now they are always equal

            Initialize();
        }

        /// <summary>
        /// Defines an external rotational axis with a mesh geometry. 
        /// </summary>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external rotational axis. </param>
        /// <param name="linkMesh"> The link mesh of the external rotational axis posed for external axis value 0. </param>
        public ExternalRotationalAxis(Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = "";
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; // Todo
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            _attachmentPlane = new Plane(_axisPlane); //TODO: for now they are always equal

            Initialize();
        }

        /// <summary>
        /// Defines an external rotational axis with a mesh geometry. 
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external rotational axis. </param>
        /// <param name="linkMesh"> The link mesh of the external rotational axis posed for external axis value 0. </param>
        public ExternalRotationalAxis(string name, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = name;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            _attachmentPlane = new Plane(_axisPlane); //TODO: for now they are always equal

            Initialize();
        }

        /// <summary>
        /// Defines an external rotational axis with a mesh geometry. 
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="axisPlane"> The axis plane. The z-axis of the plane defines the rotation vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMeshes"> The base mesh of the external rotational axis as list with Meshes. </param>
        /// <param name="linkMeshes"> The link mesh of the external rotational axis posed for external axis value 0 as list with Meshes. </param>
        public ExternalRotationalAxis(string name, Plane axisPlane, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes)
        {
            _name = name;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            _attachmentPlane = new Plane(_axisPlane); //TODO: for now they are always equal

            Initialize();
        }

        /// <summary>
        /// Creates a new external rotational axis by duplicating an existing axis.
        /// This creates a deep copy of the existing axis.
        /// </summary>
        /// <param name="externalRotationalAxis"> The external rotational axis that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the mesh should be duplicated. </param>
        public ExternalRotationalAxis(ExternalRotationalAxis externalRotationalAxis, bool duplicateMesh = true)
        {
            _name = externalRotationalAxis.Name;
            _axisPlane = new Plane(externalRotationalAxis.AxisPlane);
            _attachmentPlane = new Plane(externalRotationalAxis.AttachmentPlane);
            _axisLimits = new Interval(externalRotationalAxis.AxisLimits);
            _axisNumber = externalRotationalAxis.AxisNumber;

            if (duplicateMesh == true)
            {
                _baseMesh = externalRotationalAxis.BaseMesh.DuplicateMesh();
                _linkMesh = externalRotationalAxis.LinkMesh.DuplicateMesh();
                _posedMeshes = externalRotationalAxis.PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh());
            }
            else
            {
                _baseMesh = new Mesh();
                _linkMesh = new Mesh();
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// A method to duplicate the ExternalRotationalAxis object. 
        /// </summary>
        /// <returns> Returns a deep copy for the ExternalRotationalAxis object. </returns>
        public ExternalRotationalAxis Duplicate()
        {
            return new ExternalRotationalAxis(this);
        }

        /// <summary>
        /// A method to duplicate the ExternalRotationalAxis object without duplicating the mesh. It will set an empty mesh. 
        /// </summary>
        /// <returns> Returns a deep copy for the ExternalRotationalObject without a mesh. </returns>
        public ExternalRotationalAxis DuplicateWithoutMesh()
        {
            return new ExternalRotationalAxis(this, false);
        }

        /// <summary>
        /// A method to duplicate the ExternalRotationalAxis object to an ExternalAxis object. 
        /// </summary>
        /// <returns> Returns a deep copy of the ExternalRotationalAxis object as an ExternalAxis object. </returns>
        public override ExternalAxis DuplicateExternalAxis()
        {
            return new ExternalRotationalAxis(this) as ExternalAxis;
        }

        /// <summary>
        /// A method to duplicate the ExternalRotationalAxis object to an ExternalAxis object without the mesh. 
        /// </summary>
        /// <returns> Returns a deep copy of the ExternalRotationalAxis object as an ExternalAxis object with empty meshes. </returns>
        public override ExternalAxis DuplicateExternalAxisWithoutMesh()
        {
            return new ExternalRotationalAxis(this, false) as ExternalAxis;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid External Rotational Axis";
            }
            else
            {
                return "External Rotational Axis (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for in degrees. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePosition(double axisValue, out bool inLimits)
        {
            Transform orientNow = CalculateTransformationMatrix(axisValue, out bool isInLimits);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(orientNow);

            inLimits = isInLimits;
            return positionPlane;
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the transformation matrix for in degrees. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The transformation matrix </returns>
        public override Transform CalculateTransformationMatrix(double axisValue, out bool inLimits)
        {
            bool isInLimits;

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

            double radians = axisValue / 180 * PI;
            Transform transform = Rhino.Geometry.Transform.Rotation(radians, _axisPlane.ZAxis, _axisPlane.Origin);

            inLimits = isInLimits;
            return transform;
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for in degrees. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePositionSave(double axisValue)
        {
            Transform orientNow = CalculateTransformationMatrixSave(axisValue);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(orientNow);

            return positionPlane;
        }

        /// <summary>
        /// Calculates the transformation matrix for a defined external axis value.
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the transformation matrix for in degrees. </param>
        /// <returns> The transformation matrix </returns>
        public override Transform CalculateTransformationMatrixSave(double axisValue)
        {
            double value;

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

            double radians = value / 180 * PI;
            Transform transform = Rhino.Geometry.Transform.Rotation(radians, _axisPlane.ZAxis, _axisPlane.Origin);

            return transform;
        }

        /// <summary>
        /// Calculates the position of the external axis mesh for a defined external axis value.
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the meshes for in degrees. </param>
        public override void PoseMeshes(double axisValue)
        {
            _posedMeshes.Clear();
            _posedMeshes.Add(_baseMesh.DuplicateMesh());
            _posedMeshes.Add(_linkMesh.DuplicateMesh());

            Transform rotateNow = CalculateTransformationMatrix(axisValue, out bool inLimits);
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

        /// <summary>
        /// Transforms the external rotational axis spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> Spatial deform. </param>
        public override void Transform(Transform xform)
        {
            _attachmentPlane.Transform(xform);
            _axisPlane.Transform(xform);
            _baseMesh.Transform(xform);
            _linkMesh.Transform(xform);

            for (int i = 0; i < _posedMeshes.Count; i++)
            {
                _posedMeshes[i].Transform(xform);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the External Rotational Axis object is valid. 
        /// </summary>
        public override bool IsValid
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
        /// The name of the external axis
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
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
                _axisPlane = new Plane(_attachmentPlane); //TODO: for now they are always equal
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
                _attachmentPlane = new Plane(_axisPlane); //TODO: for now they are always equal
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