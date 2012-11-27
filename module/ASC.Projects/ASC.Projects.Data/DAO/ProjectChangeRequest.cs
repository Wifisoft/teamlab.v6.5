#region Import

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;

#endregion

namespace ASC.Projects.Data.DAO
{
    using Common;
    using Core.Domain;
    using Core.DataInterfaces;

    internal class ProjectChangeRequestDao : AbstractNHibernateDao<ProjectChangeRequest, Int32>, IProjectChangeRequestDao
    {

        #region Constructor

        public ProjectChangeRequestDao(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath) { }

        #endregion

        #region Methods

        public List<ProjectChangeRequest> Get(int projectID)
        {

            ICriteria criteria = NHibernateSession.CreateCriteria(typeof(ProjectChangeRequest))
                                                  .Add(Expression.Eq("ProjectID", projectID));

            return criteria.List<ProjectChangeRequest>() as List<ProjectChangeRequest>;
            
        }

        #endregion
    }
}
