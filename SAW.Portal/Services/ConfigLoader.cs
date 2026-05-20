using System.Text.Json;
using SAW.Portal.Models;
namespace SAW.Portal.Services;
public class ConfigLoader {
    private readonly IWebHostEnvironment _env;
    public ConfigLoader(IWebHostEnvironment env){_env=env;}
    private T Load<T>(string path){var p=Path.Combine(_env.ContentRootPath,"..","config",path);return JsonSerializer.Deserialize<T>(File.ReadAllText(p), new JsonSerializerOptions{PropertyNameCaseInsensitive=true})!;}
    public IReadOnlyList<AppDefinition> LoadApps()=>Load<List<AppDefinition>>("apps.json");
    public PolicyConfig LoadPolicies()=>Load<PolicyConfig>("policies.json");
    public IdentityMappingConfig LoadIdentityMapping()=>Load<IdentityMappingConfig>("identity-mapping.json");
    public DomainConfig LoadDomain()=>Load<DomainConfig>("domain.json");
}
