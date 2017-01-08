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

        //Graph
        public static string OfficeAppID = "ee833982-a11c-4c8a-b0cf-2fe36c117323";
        public static string[] OfficeAppScopes = { "User.Read" };
        public static string Username = string.Empty;
    }
}