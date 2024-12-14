using Hangfire.Dashboard;
using System.Text;

namespace ShopSolution.BackendApi
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _user;
        private readonly string _password;

        public HangfireDashboardAuthorizationFilter(string user, string password)
        {
            _user = user;
            _password = password;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            string header = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic "))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var authValues = GetAuthValues(header);
            if (!ValidateCredentials(authValues.user, authValues.password))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            return true;
        }

        private (string user, string password) GetAuthValues(string header)
        {
            var encodedCredentials = header.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var separatorIndex = decodedCredentials.IndexOf(':');

            return (
                decodedCredentials.Substring(0, separatorIndex),
                decodedCredentials.Substring(separatorIndex + 1)
            );
        }

        private bool ValidateCredentials(string user, string password)
        {
            return user == _user && password == _password;
        }

        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
        }
    }
}
