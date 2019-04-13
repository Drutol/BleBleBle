using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android;
using AoLibs.Adapters.Android.Interfaces;
using BleBleBle.Android.Utils.ActivityLifecycle;
using BleBleBle.Interfaces;

namespace BleBleBle.Android.Adapters
{
    public class PermissionsManager : IPermissionsManager
    {
        private readonly IContextProvider _contextProvider;
        private readonly IRequestPermissionsResultProvider _permissionsResultProvider;

        private readonly List<string> _permissions = new List<string>
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
        };

        public bool AreAllPermissionsGranted => _permissions.All(permission =>
            ContextCompat.CheckSelfPermission(_contextProvider.CurrentContext, permission) == Permission.Granted);


        public PermissionsManager(IContextProvider contextProvider,
            IRequestPermissionsResultProvider permissionsResultProvider)
        {
            _contextProvider = contextProvider;
            _permissionsResultProvider = permissionsResultProvider;
        }

        public async Task<bool> AskForPermissionGrants()
        {
            _contextProvider.CurrentContext.RequestPermissions(_permissions.ToArray(), 123);

            var result = await _permissionsResultProvider.Await();

            return result.GrantResults.All(permission => permission == Permission.Granted);
        }
    }
}