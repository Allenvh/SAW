using SAW.Portal.Models;
namespace SAW.Portal.Services;
public class IdentityMapper {
    private readonly ConfigLoader _cfg; public IdentityMapper(ConfigLoader cfg)=>_cfg=cfg;
    public string Map(string sourceUser){var map=_cfg.LoadIdentityMapping();var domain=_cfg.LoadDomain();if(map.ExplicitMappings.TryGetValue(sourceUser,out var e))return e;var u=sourceUser.Split('\\').Last();return $"{u}@{domain.TargetUpnSuffix}";}
}
