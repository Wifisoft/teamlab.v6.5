// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="FieldContainer.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.data
{

    #region usings

    #endregion

    /// <summary>
    ///   Bass class for all xdata classes that contain xData fields
    /// </summary>
    public abstract class FieldContainer : Element
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Field AddField()
        {
            var f = new Field();
            AddChild(f);
            return f;
        }

        /// <summary>
        /// </summary>
        /// <param name="field"> </param>
        public Field AddField(Field field)
        {
            AddChild(field);
            return field;
        }

        /// <summary>
        ///   Retrieve a field with the given "var"
        /// </summary>
        /// <param name="var"> </param>
        /// <returns> </returns>
        public Field GetField(string var)
        {
            ElementList nl = SelectElements(typeof (Field));
            foreach (Element e in nl)
            {
                var f = e as Field;
                if (f.Var == var)
                {
                    return f;
                }
            }

            return null;
        }

        /// <summary>
        ///   Gets a list of all form fields
        /// </summary>
        /// <returns> </returns>
        public Field[] GetFields()
        {
            ElementList nl = SelectElements(typeof (Field));
            var fields = new Field[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                fields[i] = (Field) e;
                i++;
            }

            return fields;
        }

        #endregion
    }
}