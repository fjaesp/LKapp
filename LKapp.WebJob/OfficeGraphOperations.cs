using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp;

namespace LKapp.WebJob
{
    class OfficeGraphOperations
    {
        public static void GetUsersFromAD(string GraphUrl, string ClientId, string Authority, string Thumbprint)
        {
            var certificate = GetCertificate(Thumbprint);
            if (certificate != null)
            {
                // Get an access token
                var token = GetAccessToken(certificate, GraphUrl, ClientId, Authority);
                if (!string.IsNullOrEmpty(token.Result))
                {
                    // Fetch the latest events
                    var client = new RestClient(GraphUrl);
                    var request = new RestRequest("/v1.0/users/{UserId or UserPrincipleName}/Events", Method.GET);
                    request.AddHeader("Authorization", "Bearer " + token.Result);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Accept", "application/json");

                    var response = client.Execute(request);
                    var content = response.Content;

                    Console.WriteLine(content);
                }
            }
        }

        private static X509Certificate2 GetCertificate(string Thumbprint)
        {
            X509Certificate2 certificate = null;
            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                certificate = certCollection[0];
            }
            certStore.Close();
            return certificate;
        }

        private static async Task<string> GetAccessToken(X509Certificate2 certificate, string GraphUrl, string ClientId, string Authority)
        {
            var authenticationContext = new AuthenticationContext(Authority, false);
            var cac = new ClientAssertionCertificate(ClientId, certificate);
            var authenticationResult = await authenticationContext.AcquireTokenAsync(GraphUrl, cac);
            return authenticationResult.AccessToken;
        }
    }
}