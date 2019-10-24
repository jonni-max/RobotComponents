﻿
namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Action class, main Class for all actions.
    /// </summary>
    public abstract class Action
    {
        #region fields

        #endregion

        #region constructors
        public Action()
        {

        }
        #endregion

        #region properties
        #endregion

        #region methods
        public abstract string InitRAPIDVar(RobotInfo robInfo, string RAPIDCode);
        public abstract string ToRAPIDFunction();
        #endregion
    }
}