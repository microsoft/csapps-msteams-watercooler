page_type     | languages     | products      | description
------------- | ------------- | ------------- | -------------
sample        | csharp  | office-teams | Water Cooler is a custom Teams app that enables corporate teams to create, invite, and join casual conversations among teammates, like those that take place by the Water Cooler or break room.

# Water Cooler App Template

Water Cooler is a custom Teams app that enables corporate teams to create, invite, and join casual conversations among teammates, like those that take place by the Water Cooler or break room. Use this template for multiple scenarios, such as new non-project related announcements, topics of interest, current events, or conversations about hobbies.

The app provides an easy interface for anyone to find an existing conversation or start a new one. It's a foundation for building custom targeted communication capabilities, promoting interaction amongst coworkers who may otherwise not get a chance to socialize during breaks.

![App Screens](Wiki/Images/appScreens.gif)

## Key features

* __Water Cooler Welcome Tour__: Welcome tour gives a brief introduction about the Water cooler app and helps the users on how to create the rooms or join existing rooms. 

  ![WelcomeTour](Wiki/Images/WelcomeTour.gif)
* __Water Cooler Home Page__: Browse existing rooms where team members are interacting in existing conversations with certain people or topics of interest. Active conversations on the Home Page will show a room name, short description, call duration, and room image. 

  ![Homepage](Wiki/Images/homepage.png)
* __Join room__: Active conversations will show a Join button to allow visitors to immediately enter an ongoing conversation.

  ![Join room](Wiki/Images/joinRoom.png)
* __Room creation__: Easily create rooms by specifying the room name, short description, up to 5 colleagues as an initial group and selecting from the provided set of room images. Room creation will create a Teams call/chat for all attendees to interact.

  ![Create room](Wiki/Images/createRoom.png)
* __Find room__: Use the find room feature to search keyword which will match the topic or short descriptions of ongoing conversations.

  ![Find room](Wiki/Images/findConversation.png)
* __Attendee invitation__: Just as with any Teams call, additional users can be invited after room creation.
  ![Attendee invitation](Wiki/Images/attendeeInvitation.png)
* __App badge__: Like other Teams apps, the Water Cooler icon on the left menu will show a badge with the number of active conversations visible from Teams while using any app.

  ![App Badge](Wiki/Images/badge.png)

## Architecture
![Architecture](Wiki/Images/architecture.png)

The __Water Cooler__ app has the following main components:
* __App Service (API)__: The API app service will provide the API endpoints to get the Rooms Data, its participants, and to Add a New Room. 
* __App Service (UI)__: The UI app service will display the Rooms and the participants in the room. 
* __Azure Storage__: Azure Storage tables will store the Room Data and the participants information in the tables.
* __Microsoft Graph API__: The app leverages Microsoft graph APIs to [List Participants](https://docs.microsoft.com/en-us/graph/api/call-list-participants?view=graph-rest-1.0&tabs=http), [Get User Profile.](https://docs.microsoft.com/en-us/graph/api/profile-get?view=graph-rest-beta&tabs=http)

## App Service
The app service implements two main concepts, Endpoints for displaying the calls and a scheduler job for updating the participant info.

#### API Endpoint
The end point will return all the active rooms by checking the azure storage tables. The azure storage tables will provide all active rooms. By utilizing the graph API, we will get the active participants in the call and return it data back to the UI. All these methods will be implemented using parallel async calls.

#### Scheduler
The scheduler will run for specified time and update database with the following things. It will loop through all the active calls and get the desired information from graph and based on the database records it will update or insert the records. E.g.: Currently there are 4 active users in the room. When the scheduler runs for the first time it will insert all 4 records. In next scheduler event we have a new participant in the call it will check for the change and as the new user is there it will insert the data. If any other user dropped off it will update the meeting end time for the user.

#### UI
The UI will fetch all the rooms and participants from the above-mentioned API and display the tiles information. The tile will have the Room name, description, participants list and a button to join the call. If the user is part of the call the join button will be displayed. Apart from that the first will be fixed and it will have the create new room button. Click on the button to open the dialog box with Room Name, Description and Participants list. After saving the Room a new call will be initiated. Bot will join the call and it will call other users who are invited initially. The UI application will continuously the poll the API to get the latest rooms information.

## Microsoft Graph API
#### Delegated Permissions
App service requires the following Delegated Permission:

Delegated Permission | Use Case|
------------- | ------------- |
User.Read | In order to read the profile information of the logged in user.

#### Application Permissions
App service requires the following Application Permissions:

Application Permission | Use Case|
------------- | ------------- |
Calls.AccessMedia.All | In order to access media streams in a call as an app.
Calls.Initaite.All | Initiate 1 to 1 outgoing call from app.
Calls.InitiaateGroupCall.All | Initiate outgoing group calls from the app.
Calls.JoinGroupCall.All | Join group calls and meetings as an app.
OnlineMeetings.Read.All | Read online meeting details.
OnlineMeetings.ReadWrite.All | Read and create online meetings.
People.Read.All | Read all users relevant peoples list.
User.Read.All | Read all users profile.


## Deployment Guide

Deployment of the Water Cooler app will take approx. 90 minutes, assuming all pre-requisites are in place. 

### Pre-requisites

Please make sure you are ready with the following list:

1. __Access to [ https://portal.azure.com]( https://portal.azure.com)__
   1. You will need an active subscription with credit available
   2. Note: You will need to create a new Resource Group and be able to deploy Azure Services into it
2. __Access to run PowerShell on your local machine (as Administrator) connecting to a Microsoft Teams Tenant. This is needed to create the Policy allowing the Water Cooler app to make calls inside your tenant. Or you can use Azure cloud shell instead of PowerShell on local machine.__
   1. When running the PowerShell your user account will need to have the Teams Administrator Role – or – be Global Administrator.
   2. Learn more: [ Use Microsoft Teams administrator roles to manage Teams](https://docs.microsoft.com/en-us/MicrosoftTeams/using-admin-roles)
3. __Permissions to submit/approve a Teams App for use in your tenant via the Teams Admin Centre__
   1. This allows users to find and access the app

### Deployment steps
1. __Register Azure AD application__: Register an Azure AD application in your tenant's directory for Water Cooler app.
    1. Log in to the Azure Portal for your subscription, and go to the [App registrations](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps) blade.
    2. Click __New registration__ to create an Azure AD application.
       1. __Name__: Name of your Teams App - if you are following the template for a default deployment, we 
          recommend "Water Cooler".
       2. __Supported account types__: Select "Accounts in any organizational directory" (refer to image 
          below).
       3. Leave the `"Redirect URI"` field blank for now.

          ![App Registration](Wiki/Images/appRegistration.png)
    3. Click __Register__ to complete the registration.
    4. When the app is registered, you'll be taken to the app's "Overview" page.
       1. Copy the __Application (client) ID__; we will need it later.
       2. Verify that the "Supported account types" is set to __Multiple organizations.__

          ![Client ID](Wiki/Images/clientId.png)
    5. On the side rail in the Manage section, navigate to the "Certificates & secrets" section. 
       1. In the Client secrets section, click on "+ New client secret".
       2. Add a description for the secret and choose when the secret will expire.
       3. Click "Add".

          ![Client Secret](Wiki/Images/clientSecret.png)
    6. Once the client secret is created, copy its __Value__; we will need it later. At this point you 
       should have the following 3 values. 
       1. Application (client) ID for the Water Cooler app.
       2. Client secret for the Water Cooler app.
       3. Directory (tenant) ID. (Get from the app overview page or Azure AD) 

       We recommend that you copy the values, we will need them later. 
       ![App Data](Wiki/Images/appData.png)


2. __Deploy your Azure subscription__
    1. Click on the __Deploy to Azure__ button below.

       [![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmicrosoft%2Fcsapps-msteams-watercooler%2Fmain%2FDeployment%2Fazuredeploy.json)
    2. When prompted, log in to your Azure subscription. 
    3. Azure will create a "Custom deployment" based on the Water Cooler ARM template and ask you to fill 
       in the template parameters.

       __IMPORTANT NOTE__: Please ensure that you **don't use underscore (_) or space** in any of the field values otherwise the deployment may fail.
    4. Select a subscription and a resource group.
       1. We recommend creating a new resource group.
       2. The resource group location __MUST__ be in a datacenter that supports all the following:
          1. Storage Accounts
          2. Application Insights
          3. App Service
          4. Azure Bot Services

            For an up-to-date list of datacenters that support the above, click [here](https://azure.microsoft.com/en-us/global-infrastructure/services/?products=monitor,bot-services,storage,app-service)
    5. Update the following fields in the template:
       1. __Client ID__: The application (client) ID of the Microsoft Teams Water Cooler bot app. (from Step 1)
       2. __Client Secret__: The client secret of the Microsoft Teams Water Cooler bot app. (from Step 1)
       3. __Bot Name__: – Give the name to the Bot, as the calls will be initiated by bot in the Watercooler teams app. (Bot name must be available `not taken` otherwise, the deployment will fail with a Conflict error)
       4. __User Id__: This is the Azure AD GUID for the user account you want to run the application as. It just needs to be a regular AD account, no special licencing or permissions
       5. __Storage Account Name__: you need to think of a short name here for the storage table, keep it under 12 characters and all lower-case.(The storage name must be available `not taken` otherwise, the deployment will fail with a Conflict error) 
       6. __App Service Name__: Think of a name for your app service (e.g. watercoolerappservice, must be available `not taken` otherwise, the deployment will fail with a Conflict error) - write this down as you'll need it later. 
       7. __Tenant Id__: The tenant ID. (from Step 1). Basically, it will take it from the logged in tenant. You can also update manually.
       8. __SKU__: The app service plan. P1v2, P2v2, P3v2 (Premium V2 service plans), etc., You can check all the plans and its costs here. e.g. P3v2. (We recommend Premium plan P3v2).
       9. __Terms Of Use Text__: The text that should be shown in welcome card link.
       10. __Terms Of Use Url__: The link to be redirected when user click on the TermsOfUseText..

       __Note__: Make sure that the values are copied as-is, with no extra spaces. The template checks that GUIDs are exactly 36 
       characters.

       __Note__: If your Azure subscription is in a different tenant than the tenant where you want to install the Teams App, please 
       update the Tenant Id field with the tenant where you want to install the Teams App.
    6. If you wish to change the app name, description, and icon from the defaults, modify the corresponding template parameters.
    7. Agree to the Azure terms and conditions by clicking on the check box "I agree to the terms and conditions stated above" located at 
       the bottom of the page.
    8. Click on "Agree & Create" to validate the template. It will validate the template and will provide create button if everything is 
       good. Click on Create to start the deployment.
    9. Wait for the deployment to finish. You can check the progress of the deployment from the "Notifications" pane of the Azure Portal. It may take __up to an hour__ for the deployment to finish.
    10. If deployment fails, Go to the deployed __App sevice__ -> __Deployment Center__ -> __Logs__. Then __Sync__ to redeploy.

        ![Sync](Wiki/Images/sync.png)
    11. If Sync fails then We recommend to deploy on P1v2, P2v2, P3v2 (Premium V2 service plans) and after deploying successfully change the plan to S1. 
    12. Once successfully deployed, get the URL value from the Web API App Service that has been provisioned. Copy that value to clipboard for use next (and later).  
        1. Go to: Home -> Bot Services -> Your Bot -> Channels -> Microsoft Teams (edit) -> Calling tab -> Tick ‘Enable Calling’ -> create Webhook like example here: `https://yourappserviceurl.azurewebsites.net/callback/calling`
        2. Click Save

           ![Bot Calling](Wiki/Images/botCalling.png)

        3. Click on __Bot Profile__
        4. __Update Bot icon, name and description__
           
           ![Bot Profile](Wiki/Images/botProfile.png)
   

3. __Set-up Authentication__
   1. Go to App Registrations page [here](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps) and open the Water Cooler app you created (in Step 1) from the application list.
   2. Under __Manage__, click on __Authentication__ to bring up authentication settings.
      1. Click on __Add a Platform__:
         1. Click `Web`
         2. __Redirect URI__: Enter  `https://%AppServiceUrl%/`auth-end for the URL e.g. `https://yourappserviceurl/auth-end` (app service)
         3. You can find this URL by: Azure -> App Services -> your App Service -> copy URL
      2. Under __Implicit grant__, check __ID tokens__.
      3. Click __Configure__ to commit your changes.
   3. Back under __Manage__, click on __Expose an API__.
      1. Click on the __Set__ link next to __Application ID URI__, and change the value to `api://%AppServiceURL%/clientId`, this is the same URL you used in the previous step (but with api not https) see below: 
e.g. `api://youappserviceurl.azurewebsites.net/clientId`.

      2. Click __Save__ to commit your changes.
      3. Click on __Add a scope__, under __Scopes defined by this API__. In the flyout that appears, enter the following values:
         1. __Scope name__:  `access_as_user`
         2. __Who can consent?__:  Admins and users
         3. __Admin and user consent display name__:  Access the API as the current logged-in user
         4. __Admin and user consent description__:  Access the API as the current logged-in user
      4. Click Add scope to commit your changes.
      5. Click __Add a client application__, under __Authorized client applications__. In the flyout that appears, enter the following values:
         1. __Client ID__: `5e3ce6c0-2b1f-4285-8d4b-75ee78787346` (<- Has to be this)
         2. __Authorized scopes__: Select the scope that ends with `access_as_user`. (There should only be 1 scope in this list.)
      6. Click __Add application__ to commit your changes.
      7. __Repeat the previous two steps__, but with client ID = `1fec8e78-bce4-4aaf-ab1b-5451cc387264`. After this step you should have __two__ client applications (`5e3ce6c0-2b1f-4285-8d4b-75ee78787346` and `1fec8e78-bce4-4aaf-ab1b-5451cc387264`) listed under __Authorized client applications__.

4. __Add Permissions to your App__

   Continuing from the Azure AD app registration page where we ended Step 3.
   1. Select __API Permissions__ blade from the left-hand side.
   2. Click on __Add a permission__ button to add permission to your app.
   3. In Microsoft APIs under Select an API label, select the service and give the following permissions,
      1. Under Commonly used Microsoft APIs
      2. Select “Microsoft Graph”, then select Application permissions and check the following permissions,
         1. __Calls.AccessMedia.All__
         2. __Calls.Initiate.All__
         3. __Calls.InitiateGroupCall.All__
         4. __Calls.JoinGroupCall.All__
         5. __Calls.JoinGroupCallAsGuestAll__
         6. __OnlineMeetings.ReadWrite.All__
         7. __People.Read.All__
         8. __User.Read.All__
      3. then select __Delegated permissions__ and check the following permissions
         1. __Email__
         2. __Offline_access__
         3. __OnlineMeetings.ReadWrite__
         4. __Openid__
         5. __Profile__
         6. __User.Read__
      4. Click on __Add Permissions__ to commit your changes.

         ![Permissions](Wiki/Images/permissions.png)
   4. You should have __14 Configured Permissions__ in total. If not, you’ve missed some!
   5. If you are logged in as the Global Administrator, click on the “Grant admin consent for %tenant-name%” button to grant admin consent, else inform your Admin to do the same through the portal.
Alternatively you may follow the steps below:
         1. Prepare link - `https://login.microsoftonline.com/common/adminconsent?client_id=%appId%`. Replace the `%appId%` with the Application (client) ID of Microsoft Teams Water Cooler bot app (from above).
         2. Global Administrator can grant consent using the link above.




5. __Give policy access to admin user__
   
   1. Click on command shell icon on header

      ![Command shell](Wiki/Images/cmdshell.png)
   2. Run `Update-Module MicrosoftTeams` (To use updated module)
   2. Write below commands to run and provide appropriate details
      1. Import-Module MicrosoftTeams
      2. `$userCredential` = Get-Credential
      3. Connect-MicrosoftTeams -Credential `$userCredential`
      4. New-CsApplicationAccessPolicy -Identity OnlineMeeting-WaterCooler -AppIds `%clientId%` -Description "Water cooler app - Online meeting policy for admin -Tenant"
      5. Grant-CsApplicationAccessPolicy -PolicyName OnlineMeeting-WaterCooler -Identity `%adminUserId%`

         ![PowerShell](Wiki/Images/powershell.png)

6. __Create the Teams app package__
   
   Now everything is deployed in Azure, we need to package up the Teams App and add it into Teams!

   1. Make sure you have cloned the app repository locally.
   2. Open the  Manifest\manifest.json file in a text editor.
   3. Change the placeholder fields in the manifest to values appropriate for your organization in developer property.
      1. `MenifestVersion = 1.5`
      2. `Version = 1.0.0`
      3. `id` = `Client ID`
      3. `developer.name` = Microsoft ([What's this?](https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema#developer)) 
      4. `developer.websiteUrl`
      5. `developer.privacyUrl`
      6. `developer.termsOfUseUrl` (This should be the same Terms of use URL used in step 4 ->15-> 2)
   [Note: These 3 URLs should be different]
   4. Change the placeholder fields in the manifest to values appropriate to app name property
      1. short: “`Water Cooler`”,
      2. Full: “`Water Cooler`”
      3. Description.short = Must be less than 80 characters in length
   5. Change the placeholder fields in the manifest as an example below:
      1. "entityId": "`waterCooler`",
      2. "name": "`Water Cooler`",
      3. "contentUrl": `https://yourappserviceurl.azurewebsites.net/`,
      4. "websiteUrl": `"https://yourappserviceurl.azurewebsites.net/`",
   6. Change the `<<clientId>>` placeholder in the id setting of the webApplicationInfo section to be the `%clientId%` value. Change the `<<appDomain>>` placeholder in the resource setting of the webApplicationInfo section to be the `%appDomain%` value e.g. `api://appname.azurewebsites.net/clientId`.
   7. Create a ZIP package with the  manifest.json, color.png, and outline.png. The two image files are the icons for your app in Teams.
      1. Name this package  `manifest.zip`, so you know that this is the app for the Water Cooler.
      2. Make sure that the 3 files are the top level of the ZIP package, with no nested folders.

         ![Manifest](Wiki/Images/manifest.png)

7. __Install app in Microsoft Teams__

   1. Install the Water Cooler app (the  manifest.zip package)
      1. You can do this through the Teams Admin Centre -> Teams Apps -> Manage Apps -> Upload -> Select a File -> Choose Manifest.zip you just created
      2. If your tenant has sideloading apps enabled, you can install your app by following the instructions [here](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/apps/apps-upload#load-your-package-into-teams).

8. __Upload room icons to blob storage__
    1. Open the created storage in the Resource group.
    2. Click on Storage explorer -> Blob containers.
    3. Click on upload and upload the icons for room as shown below.

       ![Blob Upload](Wiki/Images/blobUpload.png)

#### IMPORTANT
   Proactive app installation will work only if you upload the app to your tenant's app catalog. Install the app (the manifest.zip package) to the users and teams that will be the target audience.

## __For Existing Customers__
1. __Update app service application settings.__
      1. Go to resource group provided for deployment. 
      2. Open deployed API app service. 
      3. Go to Configuration from left tray.
      4. Click on Advance edit.
 
         ![Configuration](Wiki/Images/config.png)
  
      5. Update following property values. 

         ![Advance Settings](Wiki/Images/advanceEdit.png)

      6. AzureAd:ApplicationIdURI : `api://yourapiappservice.azurewebsites.net/<<clientId>>` 
      7. Add properties mentioned below.
         1. `TermsOfUseText` (Terms of use Text to be shown in welcome card) 
         2. `TermsOfUseUrl` (Link to be redirected when clicked on terms of use text)
      8. __WEBSITE_NODE_DEFAULT_VERSION__ should be `14.16.0`
      9. Save the configuration settings.

2. __Sync the deployment.__
     1. Go to __Deployment Center__ from left tray. 
     2. Click on sync.

        ![Deployment Center](Wiki/Images/sync.png)

3. __Update app services link in registered App.__
     1. Go to app registration. 
     2. Open registered application.
     3. Go to Expose an API.
        1. Update __Application ID URI__

           ![Expose An API](Wiki/Images/updateAppIdURI.png)

        2. `api://yourapiappservice.azurewebsites.net/<<ClientId>>`
        3. Save the changes.

4. __Go to Authentication.__ 
     1. Update __redirect WEB URIs__

        ![Authentication](Wiki/Images/updateAuth.png)

     2. `https://yourapiappservice.azurewebsites.net/auth-end`
     3. Save the changes.

5. Delete your __UI App service__. We don’t need it anymore.
6. Create the Teams app package (Follow Deployment process  step 6 above)  

## __Troubleshooting Guidelines__

   1. If deployment fails, Go to the deployed App service -> Deployment centre -> Logs. Then Sync to redeploy.
     
      ![Sync](Wiki/Images/sync.png)

   2. If Sync fails then We recommend to deploy on P1v2, P2v2, P3v2 (Premium V2 service plans) and after deploying successfully change the plan to S1. 

## Feedback
Thoughts? Questions? Ideas? Share them with us on [Teams UserVoice](https://microsoftteams.uservoice.com/forums/555103-public)!

Please report bugs and other code issues here.


## Legal notice

This app template is provided under the [MIT License](https://github.com/microsoft/csapps-msteams-watercooler/blob/main/LICENSE) terms. In addition to these terms, by using this app template you agree to the following:

* You, not Microsoft, will license the use of your app to users or organization.
* This app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with respect to any applicable regulations, including but not limited to privacy, healthcare, employment, or financial regulations.
* You are responsible for complying with all applicable privacy and security regulations including those related to use, collection, and handling of any personal data by your app. This includes complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.
* Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository. Microsoft’s general trademark guidelines can be found [here](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general.aspx).
* If the app template enables access to any Microsoft Internet-based services (e.g., Office365), use of those services will be subject to the separately provided terms of use. In such cases, Microsoft may collect telemetry data related to app template usage and operation. Use and handling of telemetry data will be performed in accordance with such terms of use.
* Use of this template does not guarantee acceptance of your app to the Teams app store. To make this app available in the Teams app store, you will have to comply with the [submission and validation process](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/appsource/publish), and all associated requirements such as including your own privacy statement and terms of use for your app.

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit [https://cla.microsoft.com](https://cla.microsoft.com/).

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.