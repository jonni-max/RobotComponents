﻿// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;

namespace RobotComponents.Gh.Utils
{
    /// <summary>
    /// The Object Manager keeps track of different variables to enable global funcionalities inside Grasshopper
    /// </summary>

    public class ObjectManager
    {
        #region fields
        private readonly string _id;
        private readonly Dictionary<Guid, GH_Component> _components;
        private readonly List<string> _names;
        #endregion

        #region constructors
        /// <summary>
        /// Creates an empty object manager. 
        /// </summary>
        public ObjectManager(string id)
        {
            _id = id;
            _components = new Dictionary<Guid, GH_Component>();
            _names = new List<string>() { "tool0", "wobj0", "load0" };

            _names.AddRange(SpeedData.ValidPredefinedNames);
            _names.AddRange(ZoneData.ValidPredefinedNames);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "Object Manager (" + _id + ")";
        }

        /// <summary>
        /// Checks the variable names of the declaration that are generated by the given component.
        /// </summary>
        /// <param name="component"> The component that generates the variable names. </param>
        public void CheckVariableNames(GH_Component component)
        {
            if (component is IObjectManager managedComponent)
            {
                // Adds component to collection
                if (!_components.ContainsKey(component.InstanceGuid))
                {
                    _components.Add(component.InstanceGuid, component);
                }

                // Remove registered variable names
                for (int i = 0; i < managedComponent.Registered.Count; i++)
                {
                    _names.Remove(managedComponent.Registered[i]);
                }

                managedComponent.Registered.Clear();
                _names.Remove(managedComponent.LastName);
                managedComponent.IsUnique = true;

                // Run SolveInstance on other components with no unique name to check if their name is now available
                this.UpdateComponents();

                for (int i = 0; i < managedComponent.ToRegister.Count; i++)
                {
                    // Duplicate varialble name
                    if (_names.Contains(managedComponent.ToRegister[i]))
                    {
                        component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The variable name \"" + managedComponent.ToRegister[i] + "\" is aleady in use.");
                        managedComponent.IsUnique = false;
                        managedComponent.LastName = "";
                        break;
                    }

                    // Empty variable name
                    else if (managedComponent.ToRegister[i] == string.Empty)
                    {
                        managedComponent.LastName = "";
                        break;
                    }

                    // Register unique variable names
                    else
                    {
                        managedComponent.Registered.Add(managedComponent.ToRegister[i]);
                        _names.Add(managedComponent.ToRegister[i]);

                        managedComponent.LastName = managedComponent.ToRegister[i];
                    }

                    // Checks if variable name exceeds max character limit for RAPID Code
                    if (Utils.HelperMethods.VariableExeedsCharacterLimit32(managedComponent.ToRegister[i]))
                    {
                        component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Variable name exceeds character limit of 32 characters.");
                        break;
                    }

                    // Checks if variable name starts with a number
                    if (Utils.HelperMethods.VariableStartsWithNumber(managedComponent.ToRegister[i]))
                    {
                        component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Variable name starts with a number which is not allowed in RAPID Code.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes data from the object manager that was registered by the given component. 
        /// </summary>
        /// <param name="component"> The component that registered data. </param>
        public void DeleteManagedData(GH_Component component)
        {
            if (component is IObjectManager managedComponent)
            {
                if (managedComponent.IsUnique == true)
                {
                    for (int i = 0; i < managedComponent.Registered.Count; i++)
                    {
                        _names.Remove(managedComponent.Registered[i]);
                    }
                }

                _components.Remove(component.InstanceGuid);
                this.UpdateComponents();
            }
        }

        /// <summary>
        /// Runs Solve Instance on all other components to check if the variable names are unique.
        /// </summary>
        private void UpdateComponents()
        {
            foreach (KeyValuePair<Guid, GH_Component> entry in _components)
            {
                if (entry.Value is IObjectManager component)
                {
                    if (component.IsUnique == false)
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list with the registered variable names. 
        /// </summary>
        /// <returns> List with registered variable names. </returns>
        public List<string> GetRegisteredVariableNames()
        {
            List<string> result = new List<string>() { };

            foreach (KeyValuePair<Guid, GH_Component> entry in _components)
            {
                if (entry.Value is IObjectManager component)
                {
                    result.AddRange(component.Registered);
                }   
            }

            return result;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the Robot Components document ID
        /// </summary>
        public string ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the dictionary with all the components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, GH_Component> Components
        {
            get { return _components; }
        }

        /// <summary>
        /// Gets a list with all the unique variable names in this object manager
        /// </summary>
        public List<string> Names
        {
            get { return _names; }
        }
        #endregion
    }
}
