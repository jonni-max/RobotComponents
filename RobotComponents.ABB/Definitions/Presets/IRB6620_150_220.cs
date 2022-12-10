﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Definitions.Presets
{
    /// <summary>
    /// Represents a collection of methods to get the IRB6620-150/2.20 Robot instance.
    /// </summary>
    public static class IRB6620_150_220
    {
        /// <summary>
        /// Returns a new IRB6620-150/2.20 Robot instance.
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>>
        public static Robot GetRobot(Plane positionPlane, RobotTool tool, IList<ExternalAxis> externalAxes = null)
        {
            string name = "IRB6620-150/2.2";
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Make empty list with external axes if the value is null
            if (externalAxes == null)
            {
                externalAxes = new List<ExternalAxis>() { };
            }

            // Override the position plane when an external axis is coupled that moves the robot
            for (int i = 0; i < externalAxes.Count; i++)
            {
                if (externalAxes[i].MovesRobot == true)
                {
                    positionPlane = externalAxes[i].AttachmentPlane;
                    break;
                }
            }

            Robot robot = new Robot(name, meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, tool, externalAxes);
            Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
            robot.Transform(trans);

            return robot;
        }

        /// <summary>
        /// Defines the base and link meshes in robot coordinate space. 
        /// </summary>
        /// <returns> The list with robot meshes. </returns>
        public static List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>() { };
            string linkString;

            // Base
            linkString = Properties.Resources.IRB6620_150_2_20_link_0;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = Properties.Resources.IRB6620_150_2_20_link_1;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = Properties.Resources.IRB6620_150_2_20_link_2;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = Properties.Resources.IRB6620_150_2_20_link_3;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = Properties.Resources.IRB6620_150_2_20_link_4;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = Properties.Resources.IRB6620_150_2_20_link_5;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = Properties.Resources.IRB6620_150_2_20_link_6;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));

            return meshes;
        }

        /// <summary>
        /// Returns the list with the axis planes in robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            List<Plane> axisPlanes = new List<Plane>() { };

            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0, 0, 0),
                new Vector3d(0, 0, 1)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(320, 0, 680),
                new Vector3d(0, 1, 0)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(320, 0, 680.0 + 975),
                new Vector3d(0, 1, 0)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(502, 0, 680.0 + 975.0 + 200),
                new Vector3d(1, 0, 0)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(320.0 + 887.0, 0, 680.0 + 975.0 + 200),
                new Vector3d(0, 1, 0)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(320.0 + 887.0 + 200.0, 0, 680.0 + 975.0 + 200),
                new Vector3d(1, 0, 0)));

            return axisPlanes;
        }

        /// <summary>
        /// Returns the list with axis limits.  
        /// </summary>
        /// <returns> The list with axis limits. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval> { };

            axisLimits.Add(new Interval(-170, 170));
            axisLimits.Add(new Interval(-65, 140));
            axisLimits.Add(new Interval(-180, 70));
            axisLimits.Add(new Interval(-300, 300));
            axisLimits.Add(new Interval(-130, 130));
            axisLimits.Add(new Interval(-300, 300));

            return axisLimits;
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Plane mountingFrame = new Plane(
                new Point3d(320.0 + 887.0 + 200.0, 0, 680.0 + 975.0 + 200),
                new Vector3d(1, 0, 0));

            mountingFrame.Rotate(Math.PI* -0.5, mountingFrame.Normal);

            return mountingFrame;
        }
    }
}