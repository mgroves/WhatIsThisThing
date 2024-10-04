using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using WhatIsThisThing.Server.Auth;

namespace WhatIsThisThing.Server.Controllers;

public class AuthController : Controller
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("/api/verify-token")]
    public async Task<IActionResult> VerifyToken([FromBody] TokenRequest tokenRequest)
    {
        try
        {
            // Validate the token using Google's API
            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenRequest.Credential);

            // Token is valid, proceed with your logic (e.g., create a JWT for session management)

            // look up user in database to get permissions (and confirm that user exists)
            var isAllowed = CheckCouchbaseEmail(payload.Email);
            if (!isAllowed)
                return Unauthorized(new { Message = "You do not have admin access. You must have a Couchbase email address." });

            var jwtToken = _tokenService.GenerateJwtToken(payload.Name);

            return Ok(new
            {
                Message = "Token is valid",
                payload.Email,
                payload.Name,
                JwtToken = jwtToken
            });
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { Message = "Invalid Token" });
        }
        catch (Exception)
        {
            return Problem("There was a problem validating that token.");
        }
    }

    private bool CheckCouchbaseEmail(string email)
    {
        try
        {
            // Parse the email address using MimeKit
            var mailboxAddress = MailboxAddress.Parse(email);

            // Check if the domain is couchbase.com
            return mailboxAddress.Domain.Equals("couchbase.com", StringComparison.OrdinalIgnoreCase);
        }
        catch (ParseException)
        {
            // If parsing fails, it's not a valid email address
            return false;
        }
    }
}
