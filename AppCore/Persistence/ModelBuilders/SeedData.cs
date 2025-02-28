using AppCore.Domain.AppCore.Enums;
using AppCore.Domain.AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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


    #region HELPERS
    public static List<ResourceAccess> GetAllControllerMenu(string assemblyName)
    {
        List<ResourceAccess> objResp = new();

        // Get the WebApp assembly
        var webAppAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        if (webAppAssembly == null)
        {
            // If not loaded, try to load it
            try
            {
                webAppAssembly = Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load WebApp assembly: {ex.Message}");
                return objResp;
            }
        }
        var ctrResults = webAppAssembly
        .GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type))
        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
        .GroupBy(x => x.DeclaringType?.Name)
        .Select(x => new { ControllerBase = x.Key, Actions = x.Select(s => s.Name).ToList() })
        .ToList();

        var ctrResultsWithFilter = webAppAssembly
        .GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type))
        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                // Only include actions that have the AuthAttributeDynamicOnx filter attribute
                .Where(m => m.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "AuthAttributeDynamicOnx"))
        .GroupBy(x => new { Name = x.DeclaringType?.Name, Namespace = x.DeclaringType?.Namespace })
        .Select(x => new { Namespace = x.Key.Namespace, ControllerBase = x.Key.Name, Actions = x.Select(s => s.Name).ToList() })
        .ToList();

        int counter = 1;
        foreach (var ctr in ctrResultsWithFilter)
        {
            var ctrName = $"{ctr.Namespace?.ToString()}#{ctr.ControllerBase?.ToString()}";
            var displayName = !string.IsNullOrWhiteSpace(ctrName) ? ctrName.Replace("Controller", "") : throw new Exception("Invalid Controller name");
            string CtrCode = $"MDC{counter++}{DateTime.Now:yyyyMMddhhMMss}";


            #region For Inspection purposes only
            Console.WriteLine($"========{ctrName}========");
            ctr.Actions.ForEach(a =>
            {
                objResp.Add(new ResourceAccess { ResourceControllerName = ctrName.Trim(), ResourceFunctionName = a.Trim(), ResourceFullName = $"{ctrName.Trim()}#{a.Trim()}", AllowedRoles = UserRolesEnum.Admin.ToString(), ResourceId = Guid.NewGuid() });
                Console.WriteLine($"Action name is::: {a.Trim()}\n");
            });
            #endregion
        }
        //}
        return objResp;
    }
    #endregion




}
