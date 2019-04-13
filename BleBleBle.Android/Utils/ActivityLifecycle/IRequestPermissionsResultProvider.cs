using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Interfaces;

namespace BleBleBle.Android.Utils.ActivityLifecycle
{
    public interface
        IRequestPermissionsResultProvider : IOnActivityEvent<(int RequestCode, string[] Permissions, Permission[]
            GrantResults)>
    {

    }
}