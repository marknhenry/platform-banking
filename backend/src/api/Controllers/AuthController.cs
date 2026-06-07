using Microsoft.AspNetCore.Mvc;
using PlatformBanking.Api.Contracts;
using PlatformBanking.Models;
using PlatformBanking.Services.Audit;

namespace PlatformBanking.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public sealed class AuthController(IAuthConsentAuditPublisher auditPublisher) : ControllerBase
{
    [HttpPost("session")]
    [ProducesResponseType<CustomerIdentitySessionResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CustomerIdentitySessionResponse>> CreateSession([FromBody] CreateSessionRequest request)
    {
        var now = DateTimeOffset.UtcNow;
        var response = new CustomerIdentitySessionResponse(
            Guid.NewGuid(),
            request.UserId,
            AssuranceLevel.Medium.ToString().ToLowerInvariant(),
            now,
            now.AddHours(1),
            SessionStatus.Active.ToString().ToLowerInvariant());

        var correlationId = HttpContext.Items["x-correlation-id"]?.ToString() ?? Guid.NewGuid().ToString("N");
        await auditPublisher.PublishSignInAsync(correlationId, request.UserId, HttpContext.RequestAborted);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
