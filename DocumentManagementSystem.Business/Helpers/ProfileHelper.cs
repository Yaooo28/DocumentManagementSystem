using AutoMapper;
using DocumentManagementSystem.Business.Mappings.AutoMapper;
using System.Collections.Generic;

namespace DocumentManagementSystem.Business.Helpers
{
    public class ProfileHelper
    {
        public static List<Profile> GetProfiles()
        {
            return new List<Profile>
            {
                new DocumentProfile(),
                new AppUserProfile(),
                new DepartmentProfile(),
                new AppRoleProfile(),
            };

        }
    }
}
