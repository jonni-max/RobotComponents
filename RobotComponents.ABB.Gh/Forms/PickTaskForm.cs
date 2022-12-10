﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Components.ControllerUtility;

namespace RobotComponents.ABB.Gh.Forms
{
    public partial class PickTaskForm : Form
    {
        public static int TaskIndex = 0;

        public PickTaskForm()
        {
            InitializeComponent();
        }

        public PickTaskForm(List<string> items)
        {
            InitializeComponent();
            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(items[i]);
            }
        }

        private void PickTask_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            TaskIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        private void ComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = RemoteConnectionComponent.Tasks[comboBox1.SelectedIndex].Name.ToString();
            this.labelTaskTypeInfo.Text = RemoteConnectionComponent.Tasks[comboBox1.SelectedIndex].TaskType.ToString();
            this.labelEnabledInfo.Text = RemoteConnectionComponent.Tasks[comboBox1.SelectedIndex].Enabled.ToString();
        }
    }
}