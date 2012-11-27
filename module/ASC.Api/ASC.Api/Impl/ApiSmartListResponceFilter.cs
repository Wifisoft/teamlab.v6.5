#region usings

using System;
using System.Collections;
using ASC.Api.Collections;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    public class ApiSmartListResponceFilter : IApiResponceFilter
    {

        #region IApiResponceFilter Members

        public object FilterResponce(object responce, ApiContext context)
        {
            if (responce != null && !context.FromCache)
            {
                ISmartList smartList = null;
                var type = responce.GetType();
                if (responce is ISmartList)
                {
                    smartList = responce as ISmartList;
                }
                else if (Utils.Binder.IsCollection(type) && !typeof(IDictionary).IsAssignableFrom(type))
                {
                    try
                    {
                        var elementType = Utils.Binder.GetCollectionType(type);
                        var smartListType = SmartListFactory.GetSmartListType().MakeGenericType(elementType);
                        smartList = Activator.CreateInstance(smartListType, (IEnumerable)responce) as ISmartList;
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                if (smartList != null)
                {
                    return TransformList(context, smartList);
                }
            }
            return responce;
        }

        private static object TransformList(ApiContext context, ISmartList smartList)
        {
            if (context.Count<smartList.Count)
            {
                //We already get more than allowed, so data is not paged
                context.TotalCount = smartList.Count;
            }
            smartList.TakeCount = context.SpecifiedCount;
            smartList.StartIndex = context.StartIndex;
            smartList.IsDescending = context.SortDescending;
            smartList.SortBy = context.SortBy;
            smartList.FilterBy = context.FilterBy;
            smartList.FilterOp = context.FilterOp;
            smartList.FilterValue = context.FilterValues;
            smartList.UpdatedSince = context.UpdatedSince;
            smartList.FilterType = context.FilterToType;
            return smartList.Transform();
        }

        #endregion
    }
}