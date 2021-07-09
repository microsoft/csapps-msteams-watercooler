// <copyright file="Constants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Common
{
    /// <summary>
    /// Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// get the group read all scope.
        /// </summary>
        public const string RoomDataCreateOrUpdateSucessful = "Room data create or update successful.";

        /// <summary>
        /// get the bearer.
        /// </summary>
        public const string Schema = "Bearer";

        /// <summary>
        /// get the tenant replace string.
        /// </summary>
        public const string TenantReplaceString = "{tenant}";

        /// <summary>
        /// get the tenant id replace string.
        /// </summary>
        public const string TenantIdReplaceString = "TENANT_ID";

        /// <summary>
        /// get the No audience defined in token.
        /// </summary>
        public const string NoAudienceDefinedInToken = "No audience defined in token!";

        /// <summary>
        /// get the No valid audiences defined in validationParameters.
        /// </summary>
        public const string NoValidAudiencesDefinedInValidationParameters = "No valid audiences defined in validationParameters!";

        /// <summary>
        /// get the graph default scope.
        /// </summary>
        public const string GraphDefaultScope = "https://graph.microsoft.com/.default";

        /// <summary>
        /// get the Graph Web Client.
        /// </summary>
        public const string GraphWebClient = "GraphWebClient";

        /// <summary>
        /// get the valid issuers configuration settings exception.
        /// </summary>
        public const string ValidIssuersConfigurationSettingsException = " does not contain a valid value in the configuration file.";

        /// <summary>
        /// get the application/json.
        /// </summary>
        public const string ApplicationJsonMediaTypeHeader = "application/json";

        /// <summary>
        /// get the application/x-www-form-urlencoded.
        /// </summary>
        public const string ApplicationXWwwFormUrlencodedMediaTypeHeader = "application/x-www-form-urlencoded";

        /// <summary>
        /// get the OAuth v2 token link.
        /// </summary>
        public const string OAuthV2TokenLink = "https://login.microsoftonline.com/{tenant}";

        /// <summary>
        /// get the graph resource.
        /// </summary>
        public const string GraphResource = "https://graph.microsoft.com";

        /// <summary>
        /// get the common.
        /// </summary>
        public const string Common = "common";

        /// <summary>
        /// get the generating OAuth Token.
        /// </summary>
        public const string GeneratingOAuthToken = "AuthenticationProvider: Generating OAuth token.";

        /// <summary>
        /// get the failed to generate token for client.
        /// </summary>
        public const string FailedToGenerateTokenForClient = "Failed to generate token for client:";

        /// <summary>
        /// get the failed to generate token for client.
        /// </summary>
        public const string FailedToValidateTokenForClient = "Failed to validate token for client:";

        /// <summary>
        /// get the generated OAuth token. Expires in.
        /// </summary>
        public const string GeneratedOAuthTokenExpiresIn = "AuthenticationProvider: Generated OAuth token. Expires in";

        /// <summary>
        /// get the minutes.
        /// </summary>
        public const string Minutes = "minutes";

        /// <summary>
        /// get the authDomain.
        /// </summary>
        public const string AuthDomain = "https://api.aps.skype.com/v1/.well-known/OpenIdConfiguration";

        /// <summary>
        /// get the updating OpenID configuration.
        /// </summary>
        public const string UpdatingOpenIDConfiguration = "Updating OpenID configuration";

        /// <summary>
        /// get the graph Issuer.
        /// </summary>
        public const string GraphIssuer = "https://graph.microsoft.com";

        /// <summary>
        /// get the bot framework issuer.
        /// </summary>
        public const string BotFrameworkIssuer = "https://api.botframework.com";

        /// <summary>
        /// The name of web instance ID in query string.
        /// </summary>
        public const string WebInstanceIdName = "webInstanceId";

        /// <summary>
        /// get the environment web instance Id.
        /// </summary>
        public const string EnvWebInstanceId = "WEBSITE_INSTANCE_ID";

        /// <summary>
        /// get the local.
        /// </summary>
        public const string Local = "local";

        /// <summary>
        /// get the Authorization.
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// get the upn.
        /// </summary>
        public const string UpnKey = "upn";

        /// <summary>
        /// get the oid.
        /// </summary>
        public const string OidKey = "oid";

        /// <summary>
        /// Azure Client Id.
        /// </summary>
        public const string ClientIdConfigurationSettingsKey = "AzureAd:ClientId";

        /// <summary>
        /// Azure Tenant Id.
        /// </summary>
        public const string TenantIdConfigurationSettingsKey = "AzureAd:TenantId";

        /// <summary>
        /// Azure Application Id URI.
        /// </summary>
        public const string ApplicationIdURIConfigurationSettingsKey = "AzureAd:ApplicationIdURI";

        /// <summary>
        /// Azure Valid Issuers.
        /// </summary>
        public const string ValidIssuersConfigurationSettingsKey = "AzureAd:ValidIssuers";

        /// <summary>
        /// Azure ClientSecret .
        /// </summary>
        public const string ClientSecretConfigurationSettingsKey = "AzureAd:ClientSecret";

        /// <summary>
        /// Azure Url .
        /// </summary>
        public const string AzureInstanceConfigurationSettingsKey = "AzureAd:Instance";

        /// <summary>
        /// Azure Authorization Url .
        /// </summary>
        public const string AzureAuthUtrlConfigurationSettingsKey = "AzureAd:AuthUrl";

        /// <summary>
        /// get the meeting url regex.
        /// </summary>
        public const string MeetingUrlRegex = "https://teams\\.microsoft\\.com.*/(?<thread>[^/]+)/(?<message>[^/]+)\\?context=(?<context>{.*})";

        /// <summary>
        /// Join URL cannot be parsed.
        /// </summary>
        public const string JoinURLCannotBeParsedException = "Join URL cannot be parsed:";

        /// <summary>
        /// get the context.
        /// </summary>
        public const string Context = "context";

        /// <summary>
        /// get the thread.
        /// </summary>
        public const string Thread = "thread";

        /// <summary>
        /// get the message.
        /// </summary>
        public const string Message = "message";

        /// <summary>
        /// get the Storage account connection string.
        /// </summary>
        public const string StorageAccountConnectionStringConfigurationSettingsKey = "StorageAccountConnectionString";

        /// <summary>
        /// get the BLOB connection name.
        /// </summary>
        public const string BlobContainerNameConfigurationSettingsKey = "BlobContainerName";

        /// <summary>
        /// get the TermsofUseText.
        /// </summary>
        public const string TermsofUseTextConfigurationSettingsKey = "TermsofUseText";

        /// <summary>
        /// get the TermsofUseText.
        /// </summary>
        public const string TermsofUseUrlConfigurationSettingsKey = "TermsofUseUrl";

        /// <summary>
        /// get the Azure Ad key.
        /// </summary>
        public const string AzureAdConfigurationSettingsKey = "AzureAd";

        /// <summary>
        /// get the Bot key.
        /// </summary>
        public const string BotConfigurationSettingsKey = "Bot";

        /// <summary>
        /// get the Bot key.
        /// </summary>
        public const string CORSAllowAllPolicy = "AllowAll";

        /// <summary>
        /// get the user id key.
        /// </summary>
        public const string UserIdConfigurationSettingsKey = "UserId";

        /// <summary>
        /// get the Active Room Data cache key.
        /// </summary>
        public const string ActiveRoomData = "ActiveRoomData";

        /// <summary>
        /// get the started playing transfering prompt.
        /// </summary>
        public const string StartedPlayingTransferingPrompt = "Started playing transfering prompt";

        /// <summary>
        /// get the failed to play transfering prompt.
        /// </summary>
        public const string FailedToPlayTransferingPrompt = "Failed to play transfering prompt";

        /// <summary>
        /// get the failed to transfer to incident meeting.
        /// </summary>
        public const string FailedToTransferToIncidentMeeting = "Failed to transfer to incident meeting";
    }
}
