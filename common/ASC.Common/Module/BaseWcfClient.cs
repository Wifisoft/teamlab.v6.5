﻿using System;
using System.ServiceModel;

namespace ASC.Common.Module
{
    public class BaseWcfClient<TService> : ClientBase<TService>, IDisposable where TService : class
    {
        void IDisposable.Dispose()
        {
            // msdn recommendation to close wcf client
            try
            {
                Close();
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }
    }
}
