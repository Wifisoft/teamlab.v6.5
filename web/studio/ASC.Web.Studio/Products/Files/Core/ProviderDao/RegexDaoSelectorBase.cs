using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ASC.Files.Core.ProviderDao
{
    internal abstract class RegexDaoSelectorBase<T> : IDaoSelector
    {
        public Regex Selector { get; set; }
        public Func<object, IFileDao> FileDaoActivator { get; set; }
        public Func<object, IFolderDao> FolderDaoActivator { get; set; }
        public Func<object, T> IDConverter { get; set; }

        protected RegexDaoSelectorBase(Regex selector)
            : this(selector, null, null)
        {
        }

        protected RegexDaoSelectorBase(Regex selector, Func<object, IFileDao> fileDaoActivator, Func<object, IFolderDao> folderDaoActivator)
            : this(selector, fileDaoActivator, folderDaoActivator, null)
        {
        }

        protected RegexDaoSelectorBase(Regex selector, Func<object, IFileDao> fileDaoActivator, Func<object, IFolderDao> folderDaoActivator, Func<object, T> idConverter)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            Selector = selector;
            FileDaoActivator = fileDaoActivator;
            FolderDaoActivator = folderDaoActivator;
            IDConverter = idConverter;
        }

        public virtual object ConvertId(object id)
        {
            if (id == null) return null;

            return IDConverter != null ? IDConverter(id) : id;
        }

        public virtual object GetIdCode(object id)
        {
            return null;
        }

        public virtual bool IsMatch(object id)
        {
            return id != null && Selector.IsMatch(Convert.ToString(id, CultureInfo.InvariantCulture));
        }

        public virtual IFileDao GetFileDao(object id)
        {
            return FileDaoActivator != null ? FileDaoActivator(id) : null;
        }

        public virtual IFolderDao GetFolderDao(object id)
        {
            return FolderDaoActivator != null ? FolderDaoActivator(id) : null;
        }
    }
}