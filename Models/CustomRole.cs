namespace Project_Sem3.Models;

public class CustomRole
{
  public string Name { get; set; }
}

public static class InMemoryStoreRole
{
  public static List<CustomRole> Roles = new List<CustomRole>();
}

public class CustomRoleManager
{
  public async Task<bool> RoleExistsAsync(string roleName)
  {
    return InMemoryStoreRole.Roles.Any(r => r.Name == roleName);
  }

  public async Task CreateAsync(CustomRole role)
  {
    InMemoryStoreRole.Roles.Add(role);
  }
}
