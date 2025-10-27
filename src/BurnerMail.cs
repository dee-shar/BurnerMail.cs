using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace BurnerMailApi
{
    public class BurnerMail
    {
        private string client;
        private string accessToken;
        private string applicationSession;
        private readonly HttpClient httpClient;
        private readonly string apiApplicationKey = "1234test";
        private readonly string apiUrl = "https://burnermail.io/api";
        public BurnerMail()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("api_application_key", apiApplicationKey);
        }

        public async Task<string> Login(string email, string password)
        {
            var data = new StringContent(
                $"email={email}&password={password}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync($"{apiUrl}/auth/sign_in", data);
            var responseContent = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("client", out var clientElement))
            {
                client = clientElement.GetString();
                httpClient.DefaultRequestHeaders.Add("client", client);
            }
            if (document.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                accessToken = tokenElement.GetString();
                httpClient.DefaultRequestHeaders.Add("uid", email);
                httpClient.DefaultRequestHeaders.Add("token-type", "Bearer");
                httpClient.DefaultRequestHeaders.Add("access-token", accessToken);
            }
            if (document.RootElement.TryGetProperty("_goninja-app_session", out var sessionElement))
            {
                applicationSession = sessionElement.GetString();
                httpClient.DefaultRequestHeaders.Add("cookie", applicationSession);
            }
            return responseContent;
        }

        public async Task<string> Register(string email, string password)
        {
            var data = new StringContent(
                $"email={email}&password={password}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync($"{apiUrl}/auth/", data);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAliases(string email, string password)
        {
            var response = await httpClient.GetAsync($"{apiUrl}/aliases?email={email}&password={password}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUsers()
        {
            var response = await httpClient.GetAsync($"{apiUrl}/users");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetDomains()
        {
            var response = await httpClient.GetAsync($"{apiUrl}/v1/domains");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetHash()
        {
            var response = await httpClient.GetAsync($"{apiUrl}/v1/hash");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetAccountInfo()
        {
            var response = await httpClient.GetAsync($"{apiUrl}/v1/users.json");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetVirtualEmails()
        {
            var response = await httpClient.GetAsync($"{apiUrl}/v1/virtual_emails.json");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> CreateVirtualEmail(string email)
        {
            string hash = await GetHash();
            var payload = new
            {
                virtual_emails = new
                {
                    hash = hash,
                    address = email
                }
            };
            var data = JsonContent.Create(payload);
            var response = await httpClient.PostAsync($"{apiUrl}/v1/virtual_emails", data);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> PreviewEmail(string email, int first = 0, int last = 10)
        {
            var payload = new
            {
                virtual_emails = new
                {
                    email = email
                },
                first = first,
                last = last
            };
            var data = JsonContent.Create(payload);
            var response = await httpClient.PostAsync($"{apiUrl}/v1/emails/preview", data);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> ReadMessage(string email, int messageId)
        {
            var payload = new
            {
                virtual_emails = new
                {
                    email = email,
                    email_id = messageId
                },
            };
            var data = JsonContent.Create(payload);
            var response = await httpClient.PostAsync($"{apiUrl}/v1/emails/open", data);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteEmail(string email)
        {
            var data = JsonContent.Create(new { email = email });
            var response = await httpClient.PostAsync($"{apiUrl}/v1/emails/open", data);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
