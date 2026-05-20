using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SAW.Portal.Models;
using SAW.Portal.Services;
public class IndexModel:PageModel{
    private readonly ConfigLoader _cfg; private readonly IUserGroupResolver _groups; private readonly PolicyEngine _policy; private readonly SessionBroker _broker;
    public string SourceUser="SOURCE\\user"; public List<AppDefinition> VisibleApps=new(); public IReadOnlyCollection<SessionInfo> Sessions=Array.Empty<SessionInfo>();
    public IndexModel(ConfigLoader cfg,IUserGroupResolver groups,PolicyEngine policy,SessionBroker broker){_cfg=cfg;_groups=groups;_policy=policy;_broker=broker;}
    public void OnGet(){Load();}
    public IActionResult OnPostLaunch(string appId){Load(); var app=_cfg.LoadApps().First(a=>a.Id==appId); var g=_groups.ResolveGroups(User,SourceUser); if(!_policy.IsAppVisible(app,g)) return Forbid(); _broker.Launch(SourceUser,app,g); return RedirectToPage(); }
    public IActionResult OnPostStop(string id){_broker.Stop(id); return RedirectToPage();}
    private void Load(){SourceUser=Request.Headers["X-Mock-User"].FirstOrDefault()??"SOURCE\\user"; var g=_groups.ResolveGroups(User,SourceUser); VisibleApps=_cfg.LoadApps().Where(a=>_policy.IsAppVisible(a,g)).ToList(); Sessions=_broker.List();}
}
