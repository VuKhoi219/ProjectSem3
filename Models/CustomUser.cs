using Microsoft.AspNetCore.Identity;

namespace Project_Sem3.Models;

public class CustomUser
{
  public string Id { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
  public bool EmailConfirmed { get; set; }
  public List<string> Roles { get; set; } = new List<string>();
}

public static class InMemoryStoreUser
{
  public static List<CustomUser> Users = new List<CustomUser>();
}

public class CustomUserManager
{
  public async Task<CustomUser> FindByEmailAsync(string email)
  {
    return InMemoryStoreUser.Users.FirstOrDefault(u => u.Email == email);
  }

  public async Task<IdentityResult> CreateAsync(CustomUser user, string password)
  {
    InMemoryStoreUser.Users.Add(user);
    return IdentityResult.Success;
  }

  public async Task<IdentityResult> AddToRoleAsync(CustomUser user, string role)
  {
    if (!user.Roles.Contains(role))
    {
      user.Roles.Add(role);
    }
    return IdentityResult.Success;
  }
}
