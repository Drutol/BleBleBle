using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BleBleBle.Interfaces
{
    public interface IPermissionsManager
    {
       bool AreAllPermissionsGranted { get; }
       Task<bool> AskForPermissionGrants();
    }
}
