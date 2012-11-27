// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Privacy.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    public class Privacy : Element
    {
        public Privacy()
        {
            TagName = "query";
            Namespace = Uri.IQ_PRIVACY;
        }

        /// <summary>
        ///   The active list
        /// </summary>
        public Active Active
        {
            get { return SelectSingleElement(typeof (Active)) as Active; }
            set
            {
                if (HasTag(typeof (Active)))
                    RemoveTag(typeof (Active));

                if (value != null)
                    AddChild(value);
            }
        }

        /// <summary>
        ///   The default list
        /// </summary>
        public Default Default
        {
            get { return SelectSingleElement(typeof (Default)) as Default; }
            set
            {
                if (HasTag(typeof (Default)))
                    RemoveTag(typeof (Default));

                AddChild(value);
            }
        }

        /// <summary>
        ///   Add a provacy list
        /// </summary>
        /// <param name="list"> </param>
        public void AddList(List list)
        {
            AddChild(list);
        }

        /// <summary>
        ///   Get all Lists
        /// </summary>
        /// <returns> Array of all privacy lists </returns>
        public List[] GetList()
        {
            ElementList el = SelectElements(typeof (List));
            int i = 0;
            var result = new List[el.Count];
            foreach (List list in el)
            {
                result[i] = list;
                i++;
            }
            return result;
        }
    }
}