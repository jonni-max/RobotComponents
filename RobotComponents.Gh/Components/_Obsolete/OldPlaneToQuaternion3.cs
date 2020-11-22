﻿// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents convert plane orientation to quarternion component. An inherent from the GH_Component Class.
    /// </summary>
    public class PlaneToQuaternion3 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plane to Quarternion
        /// </summary>
        public PlaneToQuaternion3()
          : base("Plane to Quaternion", "PtoQ",
              "Calculates the four coefficient values in a quarternion. "
                + "The first value a is the real part, while the rest multiplies i, j and k, that are imaginary. "
                + System.Environment.NewLine + System.Environment.NewLine + "quarternion = a + bi + ci + dk"
                + System.Environment.NewLine + System.Environment.NewLine
                + "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Reference Plane", "RP", "The reference plane as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Coord X", "X", "The x-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Coord Y", "Y", "The y-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Coord Z", "Z", "The z-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Quaternion A", "A", "The real part of the quaternion.");
            pManager.Register_DoubleParam("Quaternion B", "B", "The first imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion C", "C", "The second imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion D", "D", "The third imaginary coefficient of the quaternion.");
            pManager.Register_StringParam("String", "STR", "The quaternion in string format that can be used in RAPID code.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            Plane plane = Plane.WorldXY;
            Plane refPlane = Plane.WorldXY;

            // Catch the input data
            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref refPlane)) { refPlane = Plane.WorldXY; }

            // Get the quarternion
            Quaternion quat = RobotComponents.Utils.HelperMethods.PlaneToQuaternion(refPlane, plane, out Point3d origin);

            // Write the quarternion value in the string format that is used in the RAPID and BASE code
            string text = "[" + quat.A.ToString("0.######") + ", " + quat.B.ToString("0.######") + ", "
                + quat.C.ToString("0.######") + ", " + quat.D.ToString("0.######") + "]";

            // Output
            DA.SetData(0, origin.X);
            DA.SetData(1, origin.Y);
            DA.SetData(2, origin.Z);
            DA.SetData(3, quat.A);
            DA.SetData(4, quat.B);
            DA.SetData(5, quat.C);
            DA.SetData(6, quat.D);
            DA.SetData(7, text);
        }

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.PlaneToQuat_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("13A3F345-1C9D-4E35-B50D-B11EF157EC72"); }
        }
    }
}