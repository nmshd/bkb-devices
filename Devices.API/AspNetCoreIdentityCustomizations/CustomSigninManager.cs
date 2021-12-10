using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Devices.API.AspNetCoreIdentityCustomizations;

public class CustomSigninManager : SignInManager<ApplicationUser>
{
    private readonly IDbContext _dbContext;

    public CustomSigninManager(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation,
        IDbContext dbContext) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _dbContext = dbContext;
    }

    public override async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
    {
        var result = await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

        if (result.Succeeded)
        {
            user.LoginOccurred();
            _dbContext.Set<ApplicationUser>().Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        return result;
    }
}
