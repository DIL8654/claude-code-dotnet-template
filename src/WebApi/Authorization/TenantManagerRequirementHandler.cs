using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

public sealed class TenantManagerRequirementHandler : AuthorizationHandler<TenantManagerRequirement>
{
    private const string PermissionsClaim = "permissions";
    private const string RolesClaim = "roles";
    private const string RequiredPermission = "create.tenant";
    private const string RequiredRole = "TenantManager";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantManagerRequirement requirement)
    {
        bool hasPermission = context.User.HasClaim(c =>
            c.Type == PermissionsClaim && c.Value == RequiredPermission);

        bool hasRole = context.User.HasClaim(c =>
            c.Type == RolesClaim && c.Value == RequiredRole);

        if (hasPermission && hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
