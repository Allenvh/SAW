using SAW.Portal.Models;
using SAW.Portal.Services;

public class CoreTests {
    [Fact] public void Redaction_Works(){ var r=new AuditLogRedactor(); Assert.Contains("[REDACTED]", r.Redact("password=abc")); }
    [Fact] public void BrowserArgs_ContainKerberosFlags(){ var s=new SessionInfo("id","SOURCE\\user","user@TARGET.LOCAL","app1",DateTimeOffset.UtcNow,DateTimeOffset.UtcNow.AddMinutes(1),"c","https://app1.target.local","wm"); var p=new SecurityPolicy(); var args=SessionBroker.GenerateBrowserArgs(s,p); Assert.Contains(args,a=>a.StartsWith("--auth-server-whitelist")); }
    [Fact] public void DenyOverAllow_Behavior(){ var g=new SecurityPolicy{Flags=new(){["AllowFileDownload"]=true}}; var d=new SecurityPolicy{Flags=new(){["AllowFileDownload"]=false}}; var t=typeof(PolicyEngine).GetMethod("Merge",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!; t.Invoke(null,new object[]{g,d}); Assert.False(g.Flags["AllowFileDownload"]); }
}
