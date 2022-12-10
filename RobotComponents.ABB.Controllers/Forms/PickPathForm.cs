﻿// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// ABB Libs
using ABB.Robotics.Controllers.ConfigurationDomain;

namespace RobotComponents.ABB.Controllers.Forms
{
    public partial class PickPathForm : Form
    {
        public string Domain = "";
        public string Type = "";
        public string Instance = "";
        public string Attribute = "";

        private DomainCollection _domains = new DomainCollection();
        private TypeCollection _types = new TypeCollection();
        private Instance[] _instances = new Instance[0];
        private AttributeCollection _attributes = new AttributeCollection();

        private static Controller _controller;

        public PickPathForm()
        {
            InitializeComponent();
        }

        public PickPathForm(Controller controller)
        {
            _controller = controller;
            InitializeComponent();
            this.PopulateDomains();
            Domain = _domains[0].Name;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ComboBoxDomainSelectedIndexChanged(object sender, EventArgs e)
        {
            Domain = _domains[comboBoxDomain.SelectedIndex].Name;

            if (PopulateTypes() == false)
            {
                comboBoxType.Items.Clear();
                comboBoxInstance.Items.Clear();
                comboBoxAttribute.Items.Clear();
                comboBoxType.DataSource = null;
                comboBoxInstance.DataSource = null;
                comboBoxAttribute.DataSource = null;
                comboBoxType.SelectedIndex = -1;
                comboBoxInstance.SelectedIndex = -1;
                comboBoxAttribute.SelectedIndex = -1;
                _types = new TypeCollection();
                _instances = new Instance[0];
                _attributes = new AttributeCollection();
                Type = "";
                Instance = "";
                Attribute = "";
                labelValueInfo.Text = "-";
            }
        }

        private void ComboBoxTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            Type = _types[comboBoxType.SelectedIndex].Name;

            if (PopulateInstances() == false)
            {
                comboBoxInstance.Items.Clear();
                comboBoxAttribute.Items.Clear();
                comboBoxInstance.DataSource = null;
                comboBoxAttribute.DataSource = null;
                comboBoxInstance.SelectedIndex = -1;
                comboBoxAttribute.SelectedIndex = -1;
                _instances = new Instance[0];
                _attributes = new AttributeCollection();
                Instance = "";
                Attribute = "";
                labelValueInfo.Text = "-";
            }
        }

        private void ComboBoxInstanceSelectedIndexChanged(object sender, EventArgs e)
        {
            Instance = _instances[comboBoxInstance.SelectedIndex].Name;

            if (PopulateAttributes() == false)
            {
                comboBoxAttribute.Items.Clear();
                comboBoxAttribute.DataSource = null;
                comboBoxAttribute.SelectedIndex = -1;
                _attributes = new AttributeCollection();
                Attribute = "";
                labelValueInfo.Text = "-";
            }
        }

        private void ComboBoxAttributeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAttribute.SelectedIndex != -1 && _attributes.Count != 0)
            {
                Attribute = _attributes[comboBoxAttribute.SelectedIndex].Name;

                try
                {
                    labelValueInfo.Text = _controller.ReadConfigurationDatabase(Domain, Type, Instance, Attribute);

                }
                catch
                {
                    labelValueInfo.Text = "Path does not exist.";
                }
            }
            else
            {
                Attribute = "";
            }
        }

        private bool PopulateDomains()
        {
            comboBoxDomain.Items.Clear();
            _domains = _controller.ControllerInstanceABB.Configuration.Domains;

            if (_domains.Count != 0)
            {
                for (int i = 0; i < _domains.Count; i++)
                {
                    comboBoxDomain.Items.Add(_domains[i].Name);
                }

                comboBoxDomain.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateTypes()
        {
            comboBoxType.Items.Clear();

            if (comboBoxDomain.SelectedIndex != -1)
            {
                _types = _domains[comboBoxDomain.SelectedIndex].Types;
            }
            else
            {
                return false;
            }

            if (_types.Count != 0)
            {
                for (int i = 0; i < _types.Count; i++)
                {
                    comboBoxType.Items.Add(_types[i].Name);
                }

                comboBoxType.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateInstances()
        {
            comboBoxInstance.Items.Clear();
            if (comboBoxType.SelectedIndex != -1)
            {
                _instances = _types[comboBoxType.SelectedIndex].GetInstances();
            }
            else
            {
                return false;
            }

            if (_instances.Length != 0)
            {
                for (int i = 0; i < _instances.Length; i++)
                {
                    comboBoxInstance.Items.Add(_instances[i].Name);
                }

                comboBoxInstance.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateAttributes()
        {
            comboBoxAttribute.Items.Clear();

            if (comboBoxType.SelectedIndex != -1)
            {
                _attributes = _types[comboBoxType.SelectedIndex].Attributes;
            }
            else
            {
                return false;
            }

            if (_attributes.Count != 0)
            {
                for (int i = 0; i < _attributes.Count; i++)
                {
                    comboBoxAttribute.Items.Add(_attributes[i].Name);
                }

                comboBoxAttribute.SelectedIndex = 0;

                return true;
            }

            return false;
        }
    }
}