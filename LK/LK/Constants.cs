namespace LK
{
    class Constants
    {
        //B2C AD
        public static string ClientID = "79364928-572a-44d8-8455-ee32e9d0cd95";
        public static string[] Scopes = { ClientID };
        public static string SignUpSignInpolicy = "B2C_1_SiUpIn";
        public static string ResetPasswordpolicy = "B2C_1_SSPR";
        public static string EditProfilepolicy = "B2C_1_SiPe";
        public static string Authority = "https://login.microsoftonline.com/LKapp.onmicrosoft.com/";

        //MobileApp
        public static string ApplicationURL = @"https://lkmobileapp.azurewebsites.net";

		// Azure app-specific connection string and hub path
		public const string ConnectionString = "Endpoint=sb://lknamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=urN2s7121zAbvWS/nb4vTTCSxJyZ9zPZHVWnXc7twWA=";
		public const string NotificationHubPath = "<Azure hub path>";

    }
}