using System;
using System.Collections.Generic;
using System.Text;

namespace InstallTemplateApp
{
    public static class Constants
    {
        public const string RedirectUrlFormat = "{0}/Redirect?action=InstallApp&appId={1}&packageKey={2}&ownerId={3}";

        // Configurable Parameters
        public const string AppIdConfigurationKey = "TemplateAppInstall:Application:AppId";
        public const string PackageKeyConfigurationKey = "TemplateAppInstall:Application:PackageKey";
        public const string OwnerIdConfigurationKey = "TemplateAppInstall:Application:OwnerId";
        public const string ClientIdConfigurationKey = "TemplateAppInstall:ServicePrincipal:ClientId";
        public const string ClientSecretConfigurationKey = "TemplateAppInstall:ServicePrincipal:ClientSecret";

        // Power BI API Constants
        public const string PowerBIApiResourceUrl = "https://analysis.windows.net/powerbi/api";
        public const string PowerBIApiUrl = "https://api.powerbi.com";
        public const string PowerBIApiBaseUrl = "https://app.powerbi.com";
        public const string PowerBIApiAuthorityUrlFormat = "https://login.microsoftonline.com/{0}/";
    }
}
