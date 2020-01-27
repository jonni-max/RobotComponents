﻿using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GH_IO.Serialization;

using RobotComponents.BaseClasses.Actions;
using RobotComponents.BaseClasses.Definitions;

using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Movement component. An inherent from the GH_Component Class.
    /// </summary>
    public class JointMovementComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public JointMovementComponent()
          : base("Action: JointMovement", "JM",
              "Defines a robot movement instruction for simulation and code generation."
                + System.Environment.NewLine +
                "RobotComponents: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")

        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as string", GH_ParamAccess.list, new List<string> { "default" });
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "Internal Axis Values as List", GH_ParamAccess.tree, new List<double> { 0, 0, 0, 0, 0, 0 });
            pManager.AddNumberParameter("External Axis Values", "EAV", "External Axis Values as List", GH_ParamAccess.tree, new List<double> { 0, 0, 0, 0, 0, 0 });
            pManager.AddGenericParameter("Speed Data", "SD", "Speed Data as Custom Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Precision", "P", "Precision as int. If value is smaller than 0, precision will be set to fine.", GH_ParamAccess.list, 0);
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 6;

        // Create an array with the variable input parameters
        readonly IGH_Param[] variableInputParameters = new IGH_Param[1]
        {
            new RobotToolParameter() { Name = "Robot Tool", NickName = "RT", Description = "Robot Tool as as list", Access = GH_ParamAccess.list, Optional = true}
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new JointMovementParameter(), "JointMovement", "JM", "Resulting Movement", GH_ParamAccess.list);  //Todo: beef this up to be more informative.
        }

        // Fields
        //private bool _expire = false;
        private bool _overrideRobotTool = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Input variables
            List<string> names = new List<string>();
            GH_Structure<GH_Number> internalAxisValuesTree = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> externalAxisValuesTree = new GH_Structure<GH_Number>();
            List<SpeedDataGoo> speedDataGoos = new List<SpeedDataGoo>();
            List<int> precisions = new List<int> { 0 };
            List<RobotToolGoo> robotToolGoos = new List<RobotToolGoo>();

            // Create an empty Robot Tool
            RobotTool emptyRobotTool = new RobotTool();
            emptyRobotTool.Clear();

            // Catch the input data from the fixed parameters
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataTree(1, out internalAxisValuesTree)) { return; }
            if (!DA.GetDataTree(2, out externalAxisValuesTree)) { return; }
            if (!DA.GetDataList(3, speedDataGoos)) { return; }
            if (!DA.GetDataList(4, precisions)) { return; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetDataList(variableInputParameters[0].Name, robotToolGoos))
                {
                    robotToolGoos = new List<RobotToolGoo>() { new RobotToolGoo(emptyRobotTool) };
                }
            }

            // Make sure variable input parameters have a default value
            if (robotToolGoos.Count == 0)
            {
                robotToolGoos.Add(new RobotToolGoo(emptyRobotTool)); // Empty Robot Tool
            }

            // Get longest Input List
            int[] sizeValues = new int[6];
            sizeValues[0] = names.Count;
            sizeValues[1] = internalAxisValuesTree.PathCount;
            sizeValues[2] = externalAxisValuesTree.PathCount;
            sizeValues[3] = speedDataGoos.Count;
            sizeValues[4] = precisions.Count;
            sizeValues[5] = robotToolGoos.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int namesCounter = -1;
            int internalValueCounter = -1;
            int externalValueCounter = -1;
            int speedDataGooCounter = -1;
            int precisionCounter = -1;
            int robotToolGooCounter = -1;
            int digitalOutputGooCounter = -1;

            //// Creates movements
            List<JointMovement> jointMovements = new List<JointMovement>();


            for (int i = 0; i < biggestSize; i++)
            {
                string name;
                List<double> internalAxisValues = new List<double>();
                List<double> externalAxisValues = new List<double>();

                SpeedDataGoo speedDataGoo;
                int precision;
                RobotToolGoo robotToolGoo;
                DigitalOutputGoo digitalOutputGoo;

                // Target counter
                if (i < sizeValues[0])
                {
                    name = names[i];
                    namesCounter++;
                }
                else
                {
                    name = names[namesCounter] + "_" + (i - namesCounter);
                }

                // internal axis values counter
                if (i < sizeValues[1])
                {
                    internalAxisValues = internalAxisValuesTree[i].ConvertAll(x => (double)x.Value);
                    internalValueCounter++;
                }
                else
                {
                    internalAxisValues = internalAxisValuesTree[internalValueCounter].ConvertAll(x => (double)x.Value);
                }

                // external axis values counter
                if (i < sizeValues[2]) //instead of calling names.Count again
                {
                    externalAxisValues = externalAxisValuesTree[i].ConvertAll(x => (double)x.Value);
                    externalValueCounter++;
                }
                else
                {
                    externalAxisValues = externalAxisValuesTree[externalValueCounter].ConvertAll(x => (double)x.Value);
                }

                // Workobject counter
                if (i < sizeValues[3])
                {
                    speedDataGoo = speedDataGoos[i];
                    speedDataGooCounter++;
                }
                else
                {
                    speedDataGoo = speedDataGoos[speedDataGooCounter];
                }

                // Precision counter
                if (i < sizeValues[4])
                {
                    precision = precisions[i];
                    precisionCounter++;
                }
                else
                {
                    precision = precisions[precisionCounter];
                }

                // Robot tool counter
                if (i < sizeValues[5])
                {
                    robotToolGoo = robotToolGoos[i];
                    robotToolGooCounter++;
                }
                else
                {
                    robotToolGoo = robotToolGoos[robotToolGooCounter];
                }

                // JointMovement constructor
                JointMovement jointMovement = new JointMovement(name, internalAxisValues, externalAxisValues, speedDataGoo.Value, precision, robotToolGoo.Value);
                jointMovements.Add(jointMovement);
            }

            // Check if a right value is used for the input of the precision
            for (int i = 0; i < precisions.Count; i++)
            {
                if (HelperMethods.PrecisionValueIsValid(precisions[i]) == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Precision value <" + i + "> is invalid. " +
                        "In can only be set to -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                        "A value of -1 will be interpreted as fine movement in RAPID Code.");
                    break;
                }
            }

            // Check if a right predefined speeddata value is used
            for (int i = 0; i < speedDataGoos.Count; i++)
            {
                if (speedDataGoos[i].Value.PreDefinied == true)
                {
                    if (HelperMethods.PredefinedSpeedValueIsValid(speedDataGoos[i].Value.V_TCP) == false)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Pre-defined speed data <" + i +
                            "> is invalid. Use the speed data component to create custom speed data or use of one of the valid pre-defined speed datas. " +
                            "Pre-defined speed data can be set to 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300, " +
                            "400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000 or 7000.");
                        break;
                    }
                }
            }

            // Output
            DA.SetDataList(0, jointMovements);
        }

        // Methods and properties for creating custom menu items and event handlers when the custom menu items are clicked
        #region menu items
        /// <summary>
        /// Boolean that indicates if the custom menu item for overriding the Robot Tool is checked
        /// </summary>
        public bool OverrideRobotTool
        {
            get { return _overrideRobotTool; }
            set { _overrideRobotTool = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            // Add our own fields
            writer.SetBoolean("Override Robot Tool", OverrideRobotTool);

            // Call the base class implementation.
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            // Read our own fields
            OverrideRobotTool = reader.GetBoolean("Override Robot Tool");

            // Call the base class implementation.
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Add custom menu items
            Menu_AppendItem(menu, "Override Robot Tool", MenuItemClickRobotTool, true, OverrideRobotTool);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Robot Tool" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickRobotTool(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Override Robot Tool");
            OverrideRobotTool = !OverrideRobotTool;

            // Add or remove the robot tool input parameter
            AddParameter(0);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        public void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = variableInputParameters[index];

            // Parameter name
            string name = variableInputParameters[index].Name;

            // If the parameter already exist: remove it
            if (Params.Input.Any(x => x.Name == name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == name), true);
            }

            // Else remove the variable input parameter
            else
            {
                // The index where the parameter should be added
                int insertIndex = fixedParamNumInput;

                // Check if other parameters are already added and correct the insert index
                for (int i = 0; i < index; i++)
                {
                    if (Params.Input.Any(x => x.Name == variableInputParameters[i].Name))
                    {
                        insertIndex += 1;
                    }
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, insertIndex);
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);

        }
        #endregion

        // Methods of variable parameter interface which handles (de)serialization of the variable input parameters
        #region variable input parameters
        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location. </returns>
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted. 
        /// You must provide a valid parameter or insertion will be skipped. 
        /// You do not, repeat not, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> A valid IGH_Param instance to be inserted. In our case a null value. </returns>
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed. 
        /// You do not need to do anything, but this would be a good time to remove 
        /// any event handlers that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if the parameter in question can indeed be removed. Note, this is only in emergencies, 
        /// typically the CanRemoveParameter function should return false if the parameter in question is not removable. </returns>
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter operations completes. 
        /// This would be a good time to ensure all Nicknames and parameter properties are correct. 
        /// This method will also be called upon IO operations such as Open, Paste, Undo and Redo.
        /// </summary>
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponentsABB.Properties.Resources.Movement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("962E09EC-D371-4B81-BE27-E786BEE86481"); }
        }

    }

}