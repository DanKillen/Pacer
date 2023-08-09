using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Pacer.Web;
public static class Extensions
{
    // -------------------------- VIEW Authorisation Helper -------------------------//
    // ClaimsPrincipal - HasOneOfRoles extension method to check if a user has any of the roles in a comma separated string
    public static bool HasOneOfRoles(this ClaimsPrincipal claims, string rolesString)
    {
        // split string into an array of roles
        var roles = rolesString.Split(",");

        // linq query to check that ClaimsPrincipal has one of these roles
        return roles.FirstOrDefault(role => claims.IsInRole(role)) != null;
    }

    // --------------------------- AUTHENTICATION Helper ----------------------------//
    // IServiceCollection extension method adding cookie authentication 
    public static void AddCookieAuthentication(this IServiceCollection services,
                                                    string notAuthorised = "/User/ErrorNotAuthorised",
                                                    string notAuthenticated = "/User/ErrorNotAuthenticated")
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = notAuthorised;
                    options.LoginPath = notAuthenticated;
                });
    }

    // --------------------------- AUTHORISATION Helper ----------------------------//
    public static void AddPolicyAuthorisation(this IServiceCollection services)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies

        services.AddAuthorization(options =>
        {
            // add policies here
            options.AddPolicy("RolePolicy", policy =>
                policy.RequireRole("admin", "manager")
            );

            options.AddPolicy("IsManagerRoleOrIsGuestEmail", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasOneOfRoles("manager") ||
                    context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value == "guest@mail.com"
                )
            );
            // for more sophisticated policies see resource based policies
            // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased  

        });
    }

    // --------------------------- AUTHENTICATION Helper ----------------------------//
    // ClaimsPrincipal extension method to extract user id (sid) from claims
    public static int GetSignedInUserId(this ClaimsPrincipal user)
    {
        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        {
            // id stored as a string in the Sid claim - convert to an int and return
            Claim sid = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid) ?? throw new KeyNotFoundException("Sid Claim is not found in the identity");
            try
            {
                return Int32.Parse(sid.Value);
            }
            catch (FormatException)
            {
                throw new KeyNotFoundException("Sid Claim value is invalid - not an integer");
            }
        }
        return 0;
    }

}

public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        string name = Enum.GetName(enumType, enumValue);
        if (name == null)
            return null;

        MemberInfo member = enumType.GetMember(name).FirstOrDefault();
        if (member == null)
            return null;

        DisplayAttribute displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? name;
    }
    }

public static class DateTimeExtensions
{
    public static int GetIso8601WeekOfYear(this DateTime date)
    {
        var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
        day = day >= 1 ? day : 7;

        var jan4 = new DateTime(date.Year, 1, 4);

        return (int)Math.Floor((date.Subtract(jan4).TotalDays + day - 1) / 7) + 1;
    }
}





