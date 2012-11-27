using System;
using System.Threading;

namespace ASC.Threading
{
    public class SlimReadLock:IDisposable
    {
        private readonly ReaderWriterLockSlim _lockSlim;

        public SlimReadLock(ReaderWriterLockSlim lockSlim)
        {
            _lockSlim = lockSlim;
            _lockSlim.EnterReadLock();
        }

        public void Dispose()
        {
            _lockSlim.ExitReadLock();
        }
    }

    public class SlimWriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _lockSlim;

        public SlimWriteLock(ReaderWriterLockSlim lockSlim)
        {
            _lockSlim = lockSlim;
            _lockSlim.EnterWriteLock();
        }

        public void Dispose()
        {
            _lockSlim.ExitWriteLock();
        }
    }

    public class SlimUpgradeableReadLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _lockSlim;
        private bool _isWriting;

        public SlimUpgradeableReadLock(ReaderWriterLockSlim lockSlim)
        {
            _lockSlim = lockSlim;
            _lockSlim.EnterUpgradeableReadLock();
        }

        public void UpgradeToWrite()
        {
            _lockSlim.EnterWriteLock();
            _isWriting = true;
        }

        public void Dispose()
        {
            if (_isWriting)
                _lockSlim.ExitWriteLock();
            _lockSlim.ExitReadLock();
        }
    }  

    public static class ReaderWriterLockSlimExtensions
    {
        public static SlimReadLock BeginRead(this ReaderWriterLockSlim lockSlim)
        {
            return new SlimReadLock(lockSlim);
        }

        public static SlimWriteLock BeginWrite(this ReaderWriterLockSlim lockSlim)
        {
            return new SlimWriteLock(lockSlim);
        }

    }
}