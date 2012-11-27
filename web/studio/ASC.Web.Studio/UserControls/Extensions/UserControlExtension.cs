using System.Collections.Generic;
using System.Reflection;

namespace System.Web.UI
{
    /// <summary>
    /// Class for the LoadControl extension method(s)
    /// </summary>
    public static class UserControlExtension
    {
        /// <summary>
        /// Loads a user control with a constructor with a signature matching the supplied params
        /// Control must implement a blank default constructor as well as the custom one or we will error
        /// </summary>
        /// <param name="templateControl">Template control base object</param>
        /// <param name="controlPath">Path to the user control</param>
        /// <param name="constructorParams">Parameters for the constructor</param>
        /// <returns></returns>
        public static UserControl LoadControl(this TemplateControl templateControl, string controlPath, params object[] constructorParams)
        {
            List<Type> constParamTypes = new List<Type>();
            foreach (object constParam in constructorParams)
            {
                constParamTypes.Add(constParam.GetType());
            }

            UserControl ctl = templateControl.LoadControl(controlPath) as UserControl;

            // Find the relevant constructor
            ConstructorInfo constructor = ctl.GetType().BaseType.GetConstructor(constParamTypes.ToArray());

            //And then call the relevant constructor
            if (constructor == null)
            {
                throw new MemberAccessException("The requested constructor was not found on : " + ctl.GetType().BaseType.ToString());
            }
            constructor.Invoke(ctl, constructorParams);

            // Finally return the fully initialized UC
            return ctl;
        }
    }
}