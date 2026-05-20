namespace SAW.Portal.Models;

public record AppDefinition(string Id,string Name,string Url,string[] RequiredGroups);
public record DomainConfig(string SourceDomain,string TargetDomain,string TargetUpnSuffix,string[] SupportedControls,string[] UnsupportedControls);
public class SecurityPolicy { public Dictionary<string,bool> Flags {get;set;}=new(); public int SessionTimeoutMinutes {get;set;}=30; public bool RequireKerberos {get;set;}=true; public bool AllowNtlmFallback {get;set;}=false; }
public class PolicyConfig { public SecurityPolicy GlobalDefaults {get;set;}=new(); public Dictionary<string,SecurityPolicy> AppPolicies {get;set;}=new(); public Dictionary<string,SecurityPolicy> GroupPolicies {get;set;}=new(); }
public class IdentityMappingConfig { public bool SameUsernameMode {get;set;}=true; public Dictionary<string,string> ExplicitMappings {get;set;}=new(); }
public class SawOptions { public bool SandboxMode {get;set;}=true; public string MockUser {get;set;}="SOURCE\\user"; }
public record SessionInfo(string SessionId,string SourceUser,string TargetUser,string AppId,DateTimeOffset CreatedUtc,DateTimeOffset ExpiresUtc,string ContainerName,string Url,string Watermark);
