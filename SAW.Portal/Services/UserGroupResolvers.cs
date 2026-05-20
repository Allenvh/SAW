using System.Security.Claims;
namespace SAW.Portal.Services;
public interface IUserGroupResolver{IReadOnlyCollection<string> ResolveGroups(ClaimsPrincipal principal,string sourceUser);} 
public class MockGroupResolver: IUserGroupResolver {
    public IReadOnlyCollection<string> ResolveGroups(ClaimsPrincipal principal,string sourceUser)=>sourceUser.Contains("admin",StringComparison.OrdinalIgnoreCase)
        ? new[]{"SAW_Users","SAW_Admins","SAW_DevTools_Allowed"}
        : new[]{"SAW_Users"};
}
public class WindowsPrincipalGroupResolver: IUserGroupResolver { public IReadOnlyCollection<string> ResolveGroups(ClaimsPrincipal principal,string sourceUser)=>principal.Claims.Where(c=>c.Type==ClaimTypes.GroupSid||c.Type==ClaimTypes.Role).Select(c=>c.Value).ToArray(); }
