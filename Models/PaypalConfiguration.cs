using PayPal.Api;

namespace TrustCare.Models
{
    public static class PaypalConfiguration
    {
        //Variables for storing the clientID and clientSecret key  
        private static string clientId = "Abkae8HOQJjKF9bbDJa_rcdPlASAF3yva6UFpEUWV2m7TYb_14ZnaPqJmFNxMDyfhL1TPONzNe1jI5GW";
        private static string clientSecret = "EKdbFIYJobUhX-MojB9CmNtYJKMo_k7LWU8PkT3T_AIP-pyN7a0nbQCfp5f921qHzPb1SeTvzRwkAqkK";
        //Constructor  

        static PaypalConfiguration()
        {

        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig(string mode)
        {
            return new Dictionary<string, string>()
            {
                {"mode",mode}
            };
        }
        private static string GetAccessToken(string ClientId, string ClientSecret, string mode)
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, new Dictionary<string, string>()
            {
                {"mode",mode}
            }).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext(string clientId, string clientSecret, string mode)
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken(clientId, clientSecret, mode));
            apiContext.Config = GetConfig(mode);
            return apiContext;
        }
    }
}
