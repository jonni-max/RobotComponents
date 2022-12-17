﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick signal form class.
    /// </summary>
    public partial class PickSignalForm : Form
    {
        #region fields
        private int _index = 0;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs a pick signal form.
        /// </summary>
        /// <param name="items"> The items to fill the form with. </param>
        public PickSignalForm(List<string> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(items[i]);
            }
        }
        #endregion

        #region methods
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = "-";
            this.labelValueInfo.Text = "-";
            this.labelTypeInfo.Text = "-";
            this.labelMinValueInfo.Text = "-";
            this.labelMaxValueInfo.Text = "-";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _index = comboBox1.SelectedIndex;
            this.Close();
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the index of the picked signal. 
        /// </summary>
        public int Index
        {
            get { return _index; }
        }
        #endregion
    }
}
