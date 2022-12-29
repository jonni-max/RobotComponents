﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Controllers.Forms;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that gets and sets analog inputs on a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class SetAnalogInputComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        private Signal _signal = new Signal();
        #endregion

        /// <summary>
        /// Initializes a new instance of the SetAnalogIntputComponent class.
        /// </summary>
        public SetAnalogInputComponent()
          : base("Set Analog Input", "SetAI",
              "Changes the state of a defined analog input from an ABB controller in realtime."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Controller", "C", "Controller to connected to as Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the Analog Intput Signal as text", GH_ParamAccess.item);
            pManager.AddNumberParameter("Value", "V", "State of the Analog Input as a number", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Update", "U", "Updates the Analog Input as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Signal(), "Signal", "S", "Analog Input Signal", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables      
            string name = "";
            double value = 0.0;
            bool update = false;

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref name)) { return; }
            if (!DA.GetData(2, ref value)) { return; }
            if (!DA.GetData(3, ref update)) { return; }

            // Get the signal
            if (name != _signal.Name)
            {
                try
                {
                    _signal = _controller.GetAnalogInput(name, out _);
                }
                catch (Exception e)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                }
            }

            // Update the signal
            if (update == true)
            {
                bool success = _signal.SetValue(Convert.ToSingle(value), out string msg);

                if (success == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, msg);
                }
            }

            // Output
            DA.SetData(0, _signal);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            if (this.Params.Output[0].VolatileData.DataCount > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component only functions correctly with item inputs. " +
                    "Use multiple components if you want to set multiple signals.");
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.SetAnalogOutput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C799F319-75D6-4F0D-BCAD-8E2ECACEB0B8"); }
        }
        #endregion

        #region menu-items
        /// <summary>
        /// Adds the additional item "Pick Signal" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Signal", MenuItemClick);
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

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            if (this.GetSignal() == true)
            {
                this.Params.Input[1].RemoveAllSources();
                HelperMethods.CreatePanel(this, _signal.Name, 1);
                this.ExpireSolution(true);
            }
        }
        #endregion

        #region addtional methods
        /// <summary>
        /// Get the signal
        /// </summary>
        /// <returns> Indicates whether or not the signal was picked successfully. </returns>
        private bool GetSignal()
        {
            List<Signal> signals = _controller.AnalogInputs;

            if (signals.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No analog input signals found!");
                return false;
            }

            else if (signals.Count == 1)
            {
                _signal = signals[0];
                return true;
            }

            else if (signals.Count > 1)
            {
                PickSignalForm frm = new PickSignalForm(signals);
                Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
                frm.ShowDialog();
                int index = frm.Index;

                if (index < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal picked from the menu!");
                    _signal = new Signal();
                    return false;
                }
                else
                {
                    _signal = signals[index];
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}