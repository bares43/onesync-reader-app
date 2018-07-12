using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace EbookReader.Helpers {
    public static class PermissionHelper {

        public static async Task<PermissionStatus> CheckAndRequestPermission(Permission permission) {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted) {

                var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                if (results.ContainsKey(permission))
                    status = results[permission];
            }

            return status;
        }
    }
}