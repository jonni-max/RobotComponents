﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Robot Components Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Gh.Forms
{
    public partial class PickRobotForm : Form
    {
        public int RobotIndex = 0;
        private readonly List<RobotPreset> _robotPresets;

        public PickRobotForm()
        {
            InitializeComponent();
        }

        public PickRobotForm(List<RobotPreset> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(PickRobotForm.GetRobotPresetName(items[i]));
            }

            _robotPresets = items;
        }

        private void PickRobot_Load(object sender, EventArgs e)
        {

        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = PickRobotForm.GetRobotPresetName(_robotPresets[comboBox1.SelectedIndex]);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RobotIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        /// <summary>
        /// Returns the Robot preset name.
        /// </summary>
        /// <param name="preset"> The Robot preset. </param>
        /// <returns> The name of the Robot preset. </returns>
        private static string GetRobotPresetName(RobotPreset preset)
        {
            if (preset == RobotPreset.IRB1010_1_5_037)
            {
                return "IRB1010-1.5/0.37";
            }
            else
            {
                string name = Enum.GetName(typeof(RobotPreset), preset);
                name = name.ReplaceFirst("_", "-");
                name = name.ReplaceFirst("_", "/");
                name = name.ReplaceFirst("/0", "/0.");
                name = name.ReplaceFirst("/1", "/1.");
                name = name.ReplaceFirst("/2", "/2.");
                name = name.ReplaceFirst("/3", "/3.");
                name = name.ReplaceFirst("/4", "/4.");
                name = name.ReplaceFirst("/5", "/5.");
                name = name.ReplaceFirst("/6", "/6.");
                name = name.ReplaceFirst("/7", "/7.");
                name = name.ReplaceFirst("/8", "/8.");
                name = name.ReplaceFirst("/9", "/9.");

                return name;
            }
        }
    }
}