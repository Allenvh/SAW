using System.Collections.Concurrent;
using SAW.Portal.Models;
namespace SAW.Portal.Services;
public class SessionBroker {
    private readonly ConcurrentDictionary<string,SessionInfo> _sessions = new();
    private readonly PolicyEngine _policy; private readonly IdentityMapper _mapper; private readonly ILogger<SessionBroker> _log;
    public SessionBroker(PolicyEngine policy, IdentityMapper mapper, ILogger<SessionBroker> log){_policy=policy;_mapper=mapper;_log=log;}
    public IReadOnlyCollection<SessionInfo> List()=>_sessions.Values.ToArray();
    public SessionInfo Launch(string sourceUser, AppDefinition app, IReadOnlyCollection<string> groups){var pol=_policy.Evaluate(app.Id,groups);var id=$"saw-{Guid.NewGuid():N}";var target=_mapper.Map(sourceUser);var now=DateTimeOffset.UtcNow;var watermark=$"{sourceUser} | {target} | {app.Name} | {id} | {now:O}";var s=new SessionInfo(id,sourceUser,target,app.Id,now,now.AddMinutes(pol.SessionTimeoutMinutes),$"ctr-{id}",app.Url,watermark);_sessions[id]=s;_log.LogInformation("1100 Session created {SessionId}",id);return s;}
    public bool Stop(string id){if(_sessions.TryRemove(id,out _)){_log.LogInformation("1101 Session stopped {SessionId}",id);return true;} return false;}
    public int CleanupExpired(){var now=DateTimeOffset.UtcNow;var ids=_sessions.Values.Where(x=>x.ExpiresUtc<now).Select(x=>x.SessionId).ToArray();foreach(var id in ids){_sessions.TryRemove(id,out _);}return ids.Length;}
    public static string[] GenerateBrowserArgs(SessionInfo s, SecurityPolicy p)=>new[]{"--no-first-run","--disable-sync","--disable-dev-shm-usage",$"--user-data-dir=/profiles/{s.SessionId}","--auth-server-whitelist=*.target.local","--auth-negotiate-delegate-whitelist=*.target.local", p.Flags.GetValueOrDefault("AllowDevTools")?"":"--disable-dev-tools"}.Where(x=>!string.IsNullOrWhiteSpace(x)).ToArray();
}
public class SessionCleanupWorker: BackgroundService{private readonly SessionBroker _b; public SessionCleanupWorker(SessionBroker b)=>_b=b; protected override async Task ExecuteAsync(CancellationToken stoppingToken){while(!stoppingToken.IsCancellationRequested){_b.CleanupExpired();await Task.Delay(TimeSpan.FromSeconds(30),stoppingToken);}}}
public class AuditLogRedactor { public string Redact(string input)=>System.Text.RegularExpressions.Regex.Replace(input,"(?i)(password|authorization|cookie|token)\\s*[=:]\\s*[^;\\s]+","$1=[REDACTED]"); }
