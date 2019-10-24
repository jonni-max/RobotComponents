﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;

namespace RobotComponents.Components
{

    public class TargetVisualizerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public TargetVisualizerComponent()
          : base("Target Visualizer", "TV",
              "Displays Target Information."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Actions", "A", "Input Actions here, all Targets will be extracted automatically.", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Display Names", "DN", "Display Target Names if set to true", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Display Points", "DP", "Display Target Origins if set to true.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Display Direction", "DD", "Displays Target Plane Direction if set to true.", GH_ParamAccess.item, true);
            pManager.AddColourParameter("Color", "C", "Display Color", GH_ParamAccess.item, System.Drawing.Color.Black);
            pManager.AddIntegerParameter("Text Size", "TS", "Text size as int", GH_ParamAccess.item, 8);
            pManager.AddIntegerParameter("Point Size", "PS", "Point size as int", GH_ParamAccess.item, 3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        GH_Structure<TargetGoo> targetGoos = new GH_Structure<TargetGoo>();
        System.Drawing.Color color = new System.Drawing.Color();
        bool displayNames = true;
        bool displayPoints = true;
        bool displayDirections = false;
        int textSize = 7;
        int pointSize = 2;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> actions = new GH_Structure<IGH_Goo>();
            targetGoos.Clear();

            if (!DA.GetDataTree(0, out actions)) { return; }
            if (!DA.GetData(1, ref displayNames)) { return; }
            if (!DA.GetData(2, ref displayPoints)) { return; }
            if (!DA.GetData(3, ref displayDirections)) { return; }
            if (!DA.GetData(4, ref color)) { return; }
            if (!DA.GetData(5, ref textSize)) { return; }
            if (!DA.GetData(6, ref pointSize)) { return; }

            // Get paths
            var paths = actions.Paths;
            // Catch right datatype
            for (int i = 0; i < actions.Branches.Count; i++)
            {
                var branches = actions.Branches[i];
                GH_Path iPath = paths[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    if (actions.Branches[i][j] is MovementGoo)
                    {
                        MovementGoo movementGoo = actions.Branches[i][j] as MovementGoo;
                        TargetGoo targetGoo = new TargetGoo(movementGoo.Value.Target);
                        targetGoos.Append(targetGoo, iPath);
                    }
                    else if (actions.Branches[i][j] is TargetGoo)
                    {
                        TargetGoo targetGoo = actions.Branches[i][j] as TargetGoo;
                        targetGoos.Append(targetGoo, iPath);
                    }
                    else if (actions.Branches[i][j] is GH_Plane)
                    {
                        string targetName = "";
                        if (actions.Branches.Count == 1)
                        {
                            targetName = "plane" + "_" + j;
                        }
                        else
                        {
                            targetName = "plane" + "_" + i + "_" + j;
                        }

                        GH_Plane planeGoo = actions.Branches[i][j] as GH_Plane;
                        Target target = new Target(targetName, planeGoo.Value);
                        TargetGoo targetGoo = new TargetGoo(target);
                        targetGoos.Append(targetGoo, iPath);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A wrong datatype is used as input. " +
                            "Use a Action : Target, Action : Movement or a plane as input for the actions.");
                    }
                }
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.Target_Visualizer_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EDFDCE2D-65BC-4B99-8BCA-C171D42CB89B"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {

            for (int i = 0; i < targetGoos.Branches.Count; i++)
            {
                var branches = targetGoos.Branches[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    Target target = branches[j].Value;

                    // Display name
                    if (displayNames == true)
                    {
                        double pixelsPerUnit;
                        args.Viewport.GetWorldToScreenScale(target.Plane.Origin, out pixelsPerUnit);

                        Plane plane;
                        args.Viewport.GetCameraFrame(out plane);
                        plane.Origin = target.Plane.Origin + target.Plane.ZAxis * 2;

                        args.Display.Draw3dText(target.Name, color, plane, textSize / pixelsPerUnit, "Lucida Console");
                    }

                    // Display points
                    if (displayPoints == true)
                    {
                        args.Display.DrawPoint(target.Plane.Origin, Rhino.Display.PointStyle.Simple, pointSize, color);
                    }

                    // Display directions
                    if (displayDirections == true)
                    {
                        Plane planeVisual = target.Plane;
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.ZAxis, System.Drawing.Color.Blue);
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.XAxis, System.Drawing.Color.Red);
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.YAxis, System.Drawing.Color.Green);
                    }
                }
            }
        }
    }
}