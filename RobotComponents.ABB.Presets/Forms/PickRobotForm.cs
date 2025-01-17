﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Robot Components Libs
using RobotComponents.ABB.Presets.Enumerations;

namespace RobotComponents.ABB.Presets.Forms
{
    /// <summary>
    /// Represents the pick robot preset form class.
    /// </summary>
    public partial class PickRobotForm : Form
    {
        #region fields
        private readonly List<RobotPreset> _robotPresets;
        private int _index = 0;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form from a given list with items.
        /// </summary>
        /// <param name="items"> Items to fill the form with. </param>
        public PickRobotForm(List<RobotPreset> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(GetRobotPresetName(items[i]));
            }

            _robotPresets = items;
        }
        #endregion

        #region methods
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = GetRobotPresetName(_robotPresets[comboBox1.SelectedIndex]);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _index = comboBox1.SelectedIndex;
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
                name = ReplaceFirst(name, "_", "-");
                name = ReplaceFirst(name, "_", "/");
                name = ReplaceFirst(name, "/0", "/0.");
                name = ReplaceFirst(name, "/1", "/1.");
                name = ReplaceFirst(name, "/2", "/2.");
                name = ReplaceFirst(name, "/3", "/3.");
                name = ReplaceFirst(name, "/4", "/4.");
                name = ReplaceFirst(name, "/5", "/5.");
                name = ReplaceFirst(name, "/6", "/6.");
                name = ReplaceFirst(name, "/7", "/7.");
                name = ReplaceFirst(name, "/8", "/8.");
                name = ReplaceFirst(name, "/9", "/9.");

                return name;
            }
        }

        /// <summary>
        /// Replaces the first occurence in a string with a new text. 
        /// </summary>
        /// <param name="text"> The text the search and replace in. </param>
        /// <param name="search"> The text to search for. </param>
        /// <param name="replace"> The new text. </param>
        /// <returns> Returns a new string with replaced text. </returns>
        private static string ReplaceFirst(string text, string search, string replace)
        {
            int position = text.IndexOf(search);

            if (position < 0)
            {
                return text;
            }
            return text.Substring(0, position) + replace + text.Substring(position + search.Length);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked index.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }
        #endregion
    }
}
