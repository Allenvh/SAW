using Microsoft.AspNetCore.Mvc.RazorPages;
using SAW.Portal.Models;
using SAW.Portal.Services;
public class StatusModel:PageModel{
    private readonly ConfigLoader _cfg; private readonly SessionBroker _broker;
    public int AppCount; public int SessionCount; public DomainConfig Domain=new("","","",Array.Empty<string>(),Array.Empty<string>());
    public StatusModel(ConfigLoader cfg,SessionBroker broker){_cfg=cfg;_broker=broker;}
    public void OnGet(){AppCount=_cfg.LoadApps().Count;SessionCount=_broker.List().Count;Domain=_cfg.LoadDomain();}
}
