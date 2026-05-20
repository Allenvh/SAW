using SAW.Portal.Models;
namespace SAW.Portal.Services;
public class PolicyEngine {
    private readonly ConfigLoader _cfg; private readonly ILogger<PolicyEngine> _log;
    public PolicyEngine(ConfigLoader cfg,ILogger<PolicyEngine> log){_cfg=cfg;_log=log;}
    public SecurityPolicy Evaluate(string appId,IReadOnlyCollection<string> groups){var p=_cfg.LoadPolicies();var result=Clone(p.GlobalDefaults);if(p.AppPolicies.TryGetValue(appId,out var app))Merge(result,app);foreach(var g in groups){if(p.GroupPolicies.TryGetValue(g,out var gp))Merge(result,gp);}if(result.Flags.TryGetValue("PersistentProfile",out var pp)&&pp){result.Flags["EphemeralProfile"]=false;}
        _log.LogInformation("1600 Policy evaluated for {AppId}",appId);return result;}
    private static SecurityPolicy Clone(SecurityPolicy x)=>new(){Flags=x.Flags.ToDictionary(k=>k.Key,v=>v.Value),SessionTimeoutMinutes=x.SessionTimeoutMinutes,RequireKerberos=x.RequireKerberos,AllowNtlmFallback=x.AllowNtlmFallback};
    private static void Merge(SecurityPolicy target, SecurityPolicy src){foreach(var kv in src.Flags){ if(!kv.Value && target.Flags.ContainsKey(kv.Key)) {target.Flags[kv.Key]=false; continue;} target.Flags[kv.Key]=kv.Value;} if(src.SessionTimeoutMinutes>0)target.SessionTimeoutMinutes=src.SessionTimeoutMinutes; target.RequireKerberos = src.RequireKerberos && target.RequireKerberos; target.AllowNtlmFallback = src.AllowNtlmFallback || target.AllowNtlmFallback;}
    public bool IsAppVisible(AppDefinition app,IReadOnlyCollection<string> groups)=>app.RequiredGroups.Length==0||app.RequiredGroups.Any(groups.Contains);
}
