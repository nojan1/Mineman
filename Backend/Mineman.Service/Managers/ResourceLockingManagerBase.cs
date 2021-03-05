using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.Managers
{
    public class ResourceInUseException : Exception { }

    public abstract class ResourceLockingManagerBase
    {
        private static List<int> resourcesInUse = new List<int>();

        public void TryToClaimResource(int resourceId)
        {
            if (resourcesInUse.Contains(resourceId))
            {
                throw new ResourceInUseException();
            }

            resourcesInUse.Add(resourceId);
        }

        public void ReleaseResource(int resourceId)
        {
            resourcesInUse.Remove(resourceId);
        }

        public ResourceUsageWrapper ClaimResource(int resourceId)
        {
            return new ResourceUsageWrapper(resourceId, this);
        }
    }

    public class ResourceUsageWrapper : IDisposable
    {
        private int _resourceId;
        private ResourceLockingManagerBase _resourceLockingManager;

        public ResourceUsageWrapper(int resourceId, ResourceLockingManagerBase resourceLockingManager)
        {
            _resourceId = resourceId;
            _resourceLockingManager = resourceLockingManager;

            _resourceLockingManager.TryToClaimResource(resourceId);
        }

        public void Dispose()
        {
            _resourceLockingManager.ReleaseResource(_resourceId);
        }
    }
}
