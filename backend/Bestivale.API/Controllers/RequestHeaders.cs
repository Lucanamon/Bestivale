using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

internal static class RequestHeaders
{
    internal const string UsernameHeader = "X-Username";
    internal const string AdminUsernameHeader = "X-Admin-Username";

    internal static string? GetHeader(this ControllerBase controller, string name)
        => controller.Request.Headers[name].FirstOrDefault();

    internal static string? GetUsername(this ControllerBase controller)
        => controller.GetHeader(UsernameHeader);

    internal static string? GetAdminUsername(this ControllerBase controller)
        => controller.GetHeader(AdminUsernameHeader);

    internal static UnauthorizedObjectResult MissingUsername(this ControllerBase controller)
        => controller.Unauthorized($"Missing {UsernameHeader} header.");

    internal static UnauthorizedObjectResult MissingAdminUsername(this ControllerBase controller)
        => controller.Unauthorized($"Missing {AdminUsernameHeader} header.");
}

