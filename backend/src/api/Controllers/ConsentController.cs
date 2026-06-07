using Microsoft.AspNetCore.Mvc;
using PlatformBanking.Api.Contracts;
using PlatformBanking.Services.Audit;
using PlatformBanking.Services.Consent;

namespace PlatformBanking.Api.Controllers;

[ApiController]
[Route("v1/consents")]
public sealed class ConsentController(
    IConsentService consentService,
    ITrustIndicatorService trustIndicatorService,
    IAuthConsentAuditPublisher auditPublisher) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ConsentProfileResponse>(StatusCodes.Status200OK)]
    public ActionResult<ConsentProfileResponse> GetProfile()
    {
        var userId = ResolveUserId();
        return Ok(consentService.GetProfile(userId));
    }

    [HttpPut("{scopeCategory}")]
    [ProducesResponseType<ConsentUpdateResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ConsentUpdateResult>> UpdateConsent(
        string scopeCategory,
        [FromBody] UpdateConsentRequest request)
    {
        var userId = ResolveUserId();
        var result = consentService.UpdateConsent(userId, scopeCategory, request.Status);

        var correlationId = HttpContext.Items["x-correlation-id"]?.ToString() ?? Guid.NewGuid().ToString("N");
        await auditPublisher.PublishConsentUpdateAsync(
            correlationId,
            userId,
            scopeCategory,
            request.Status,
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpGet("trust-indicator")]
    [ProducesResponseType<TrustIndicatorResponse>(StatusCodes.Status200OK)]
    public ActionResult<TrustIndicatorResponse> GetTrustIndicator()
    {
        var userId = ResolveUserId();
        var profile = consentService.GetProfile(userId);
        return Ok(trustIndicatorService.Build(userId, profile));
    }

    private string ResolveUserId() =>
        Request.Headers["x-user-id"].FirstOrDefault() ?? "demo-user";
}
