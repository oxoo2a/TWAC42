using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;

using RestSharp.Authenticators;
using RestSharp.Contrib;
using RestSharp;

namespace twac42
{
    class Program
    {
        static void Main(string[] args)
        {
            AppID id = new AppID(args[0]);

            // Ask for the access token
            RestClient client = new RestClient("http://api.twitter.com");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(id.Key, id.Secret);
            RestRequest request = new RestRequest("oauth/request_token", Method.POST);
            RestResponse response = (RestResponse) client.Execute(request);

            Debug.Assert(response != null);
            Debug.Assert(response.StatusCode == HttpStatusCode.OK);

            var qs = HttpUtility.ParseQueryString(response.Content);
            string oauth_token = qs["oauth_token"];
            string oauth_token_secret = qs["oauth_token_secret"];

            Debug.Assert(oauth_token != null);
            Debug.Assert(oauth_token_secret != null);

            Console.WriteLine("OAuth request token is <{0}>", oauth_token);
            Console.WriteLine("OAuth request token secret is <{0}>", oauth_token_secret);

            request = new RestRequest("oauth/authorize");
            request.AddParameter("oauth_token", oauth_token);

            // Delegate to the Twitter authorization page
            // Callback URL has been defined as None on dev.twitter.com for the application. This yields the verifier PIN code
            // string url = client.BuildUri(request).ToString();
            // Process.Start(url);
            // Console.WriteLine("Awaiting Authorization ...");

            string verifier = "4870366";
            request = new RestRequest("oauth/access_token", Method.POST);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(id.Key, id.Secret, oauth_token, oauth_token_secret, verifier);
            response = (RestResponse) client.Execute(request);

            Debug.Assert(response != null);
            Debug.Assert(response.StatusCode == HttpStatusCode.OK);
            
            qs = HttpUtility.ParseQueryString(response.Content);
            oauth_token = qs["oauth_token"];
            oauth_token_secret = qs["oauth_token_secret"];

            Debug.Assert(oauth_token != null);
            Debug.Assert(oauth_token_secret != null);

            Console.WriteLine("OAuth access token is <{0}>", oauth_token);
            Console.WriteLine("OAuth access token secret is <{0}>", oauth_token_secret);

            request = new RestRequest("account/verify_credentials.xml");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(id.Key, id.Secret, oauth_token, oauth_token_secret);
            response = (RestResponse) client.Execute(request);

            Debug.Assert(response != null);
            Debug.Assert(response.StatusCode == HttpStatusCode.OK);

            request = new RestRequest("statuses/update.json", Method.POST);
            request.AddParameter("status", "TWAC42 says " + DateTime.Now.Ticks.ToString());
            response = (RestResponse) client.Execute(request);

            Debug.Assert(oauth_token != null);
            Debug.Assert(oauth_token_secret != null);
        }
    }
}
