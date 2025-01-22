using AppCore.Domain.AppCore.Enums;
using AppCore.Domain.AppCore.Models;

namespace AppCore.Persistence.ModelBuilders;

public static class SeedData
{
    public static class ApplicationDocuments
    {
        public static List<AppDocuments> GetApplicationDocuments()
        {
            return new List<AppDocuments>()
              {
                  new AppDocuments() { DocumentName = "Photo", DocumentDescription="Profile picture of the user.", DocumentAllowedFormats = new string[]
                  {
                      AppDocumentExtensionTypes.JPG.ToString().ToLower(),AppDocumentExtensionTypes.PNG.ToString().ToLower(),AppDocumentExtensionTypes.JPEG.ToString().ToLower()
                  }, MaxMbFileSize = 1.5M
                  }
              };
        }
    }

    public static List<UserRole> GetListOfUserRoles()
    {
        var defaultUserRolesList = Enum.GetNames<UserRolesEnum>();
        var objResp = new List<UserRole>();
        foreach (var item in defaultUserRolesList)
        {
            objResp.Add(new UserRole { RoleName = item.ToString() });
        }
        return objResp;
    }
}
