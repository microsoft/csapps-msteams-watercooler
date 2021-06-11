Water Cooler App Template
Water Cooler is a custom Teams app that enables corporate teams to create, invite, and join casual conversations among teammates, like those that take place by the Water Cooler or break room. Use this template for multiple scenarios, such as new non-project related announcements, topics of interest, current events, or conversations about hobbies. 
The app provides an easy interface for anyone to find an existing conversation or start a new one. It's a foundation for building custom targeted communication capabilities, promoting interaction amongst coworkers who may otherwise not get a chance to socialize during breaks.
Key features
•	Water Cooler Home Page: Browse existing rooms where team members are interacting in existing conversations with certain people or topics of interest. Active conversations on the Home Page will show a room name, short description, call duration, and room image. 
![Homepage](Wiki/Images/homepage.png)
•	Join room: Active conversations will show a Join button to allow visitors to immediately enter an ongoing conversation.
![Join room](Wiki/Images/joinRoom.png)
•	Room creation: Easily create rooms by specifying the room name, short description, up to 5 colleagues as an initial group and selecting from the provided set of room images. Room creation will create a Teams call/chat for all attendees to interact.
![Create room](Wiki/Images/createRoom.png)
•	Find room: Use the find room feature to search keyword which will match the topic or short descriptions of ongoing conversations.
![Find room](Wiki/Images/findConversation.png)
•	Attendee invitation: Just as with any Teams call, additional users can be invited after room creation.
![Attendee invitation](Wiki/Images/attendeeInvitation.png)
•	App badge: Like other Teams apps, the Water Cooler icon on the left menu will show a badge with the number of active conversations visible from Teams while using any app. 
![App Badge](Wiki/Images/badge.png)
Architecture
![App Badge](Wiki/Images/architecture.png)
The Water Cooler app has the following main components:
•	App Service (API): The API app service will provide the API endpoints to get the Rooms Data, its participants, and to Add a New Room. 
•	App Service (UI): The UI app service will display the Rooms and the participants in the room. 
•	Azure Storage: Azure Storage tables will store the Room Data and the participants information in the tables.
•	Microsoft Graph API: The app leverages Microsoft graph APIs to List Participants, Get User Profile 
________________________________________
App Service
The app service implements two main concepts, Endpoints for displaying the calls and a scheduler job for updating the participant info.
API Endpoint
The end point will return all the active rooms by checking the azure storage tables. The azure storage tables will provide all active rooms. By utilizing the graph API, we will get the active participants in the call and return it data back to the UI. All these methods will be implemented using parallel async calls.
Scheduler
The scheduler will run for specified time and update database with the following things. It will loop through all the active calls and get the desired information from graph and based on the database records it will update or insert the records. E.g.: Currently there are 4 active users in the room. When the scheduler runs for the first time it will insert all 4 records. In next scheduler event we have a new participant in the call it will check for the change and as the new user is there it will insert the data. If any other user dropped off it will update the meeting end time for the user.
UI
The UI will fetch all the rooms and participants from the above-mentioned API and display the tiles information. The tile will have the Room name, description, participants list and a button to join the call. If the user is part of the call the join button will be displayed. Apart from that the first will be fixed and it will have the create new room button. On click on the button will open a dialog box with Room Name, Description and Participants list. After saving the Room a new call will be initiated. Bot will join the call and it will call other users who are invited initially. The UI application will continuously the poll the API to get the latest rooms information. 

________________________________________
Microsoft Graph API
Delegated Permissions
App service requires the following Delegated Permission:
1.	User.Read – In order to read the profile information of the logged in user
Application Permissions
App service requires the following Application Permissions:
1.	Calls.AccessMedia.All – In order to access media streams in a call as an app
2.	Calls.Initaite.All – Initiate 1 to 1 outgoing call from app
3.	Calls.InitiaateGroupCall.All – Initiate outgoing group calls from the app
4.	Calls.JoinGroupCall.All – Join group calls and meetings as an app.
5.	OnlineMeetings.Read.All – Read online meeting details
6.	OnlineMeetings.ReadWrite.All – Read and create online meetings
7.	People.Read.All – Read all users relevant peoples list
8.	User.Read.All – Read all users profile.

Deployment Guide
1. Register Azure AD application
Register an Azure AD application in your tenant's directory for Water Cooler app.
1.	Log in to the Azure Portal for your subscription, and go to the App registrations blade.
2.	Click New registration to create an Azure AD application.
o	Name: Name of your Teams App - if you are following the template for a default deployment, we recommend "Water Cooler".
o	Supported account types: Select "Accounts in any organizational directory" (refer image below).
o	Leave the "Redirect URI" field blank for now.
![App Badge](Wiki/Images/appRegistration.png)
3.	Click Register to complete the registration.
4.	When the app is registered, you'll be taken to the app's "Overview" page. Copy the Application (client) ID; we will need it later. Verify that the "Supported account types" is set to Multiple organizations.
![App Badge](Wiki/Images/clientId.png)
 
5.	On the side rail in the Manage section, navigate to the "Certificates & secrets" section. In the Client secrets section, click on "+ New client secret". Add a description for the secret and choose when the secret will expire. Click "Add".
![App Badge](Wiki/Images/clientSecret.png)
6.	Once the client secret is created, copy its Value; we will need it later.
At this point you should have the following 3 values:
o	Application (client) ID for the Water Cooler bot.
o	Client secret for the Water Cooler bot.
o	Directory (tenant) ID.
We recommend that you copy the values, we will need them later.
![App Badge](Wiki/Images/appData.png)

2. Set-up Bot
1.	Open bot channels registration
2.	Fill details to create the bot and click on auto create App Id and password.
![App Badge](Wiki/Images/botRegistration.png)
 
3.	Fill your app id or password or auto generate.
![App Badge](Wiki/Images/botAppId.png)
4.	Go to channels from left tray.
![App Badge](Wiki/Images/channels.png)

5.	Setup Microsoft teams in channels
![App Badge](Wiki/Images/connectChannels.png)

6.	Setup Calling in channel.
As per Saikrishna the endpoint should be callback/calling – update below screenshot
 

3. Deploy to your Azure subscription
1.	Click on the Deploy to Azure button below.
 
2.	When prompted, log in to your Azure subscription.
3.	Azure will create a "Custom deployment" based on the Water Cooler ARM template and ask you to fill in the template parameters.
Note: Please ensure that you don't use underscore (_) or space in any of the field values otherwise the deployment may fail.
4.	Select a subscription and a resource group.
o	We recommend creating a new resource group.
o	The resource group location MUST be in a datacenter that supports all the following:
	Storage Accounts
	Application Insights (??same as Azure Monitor?? Please check and confirm)
	App Service
	Azure Bot Services
For an up-to-date list of datacenters that support the above, click here
5.	Enter a Base Resource Name, which the template uses to generate names for the other resources.
o	The [Base Resource Name] must be available. For example, if you select contosoWaterCooler as the base name, the name contosoWaterCooler must be available (not taken); otherwise, the deployment will fail with a Conflict error.
o	Remember the base resource name that you selected. We will need it later.
6.	Update the following fields in the template:
o	Client ID: The application (client) ID of the Microsoft Teams Water Cooler bot app. (from Step 1)
o	Client Secret: The client secret of the Microsoft Teams Water Cooler bot app. (from Step 1)
o	Tenant Id: The tenant ID. (from Step 1). Basically, it will take it from the logged in tenant. You can also update manually.
o	Bot Id: The Bot Id that you have created. If you create the clientId as Bot Id keep the BotId and ClientId as same values
o	Bot Secret: The created Bot Secret. If you create the ClientId as Bot Id then you can also keep the client secret as Bot Secret.
	Check in Arm Template if Bot Id and Bot secret are needed apart from Client ID and Client Secret – add any additional resources as per ARM template
o	Base Resource Name: It will give a prefix to your resources. So those duplicates and deployments failures will be eradicated.
o	User Id: You need to provide the Admin User Id, who will initiate the calls in the application on behalf.
o	Storage Account Name: The Azure storage table name.
o	Web API Endpoint name: The app service name for the backend deployment.
o	SKU: The app service plan. Basically the values are F1 (Free), D1 (Shared), B1, B2, B3 (Basic), S1, S2, S3 (Standard plans), P1v2, P2v2, P3v2 (Premium V2 service plans)etc. You can check all the plans and its costs here 
Note: Make sure that the values are copied as-is, with no extra spaces. The template checks that GUIDs are exactly 36 characters.
Note: If your Azure subscription is in a different tenant than the tenant where you want to install the Teams App, please update the Tenant Id field with the tenant where you want to install the Teams App.
7.	If you wish to change the app name, description, and icon from the defaults, modify the corresponding template parameters.
8.	Agree to the Azure terms and conditions by clicking on the check box "I agree to the terms and conditions stated above" located at the bottom of the page.
9.	Click on "Agree & Create" to validate the template. It will validate the template and will provide create button if everything is good. Click on Create to start the deployment.
10.	Wait for the deployment to finish. You can check the progress of the deployment from the "Notifications" pane of the Azure Portal. It may take up to an hour for the deployment to finish.
4. Create UI App service for client App
1.	Go to app services from homepage.
2.	Click on create, the below interface will appear. Fill the appropriate details and create.
![App Badge](Wiki/Images/uiAppService.png)
3.	After creating UI App service go to App service overview page.
4.	Copy UI App service URL and edit as below.
Ex. https://%uiAppserviceName.scm.domain.com
5.	Open this URL in browser and go to Tools > ZIP push deploy
![App Badge](Wiki/Images/zipDeploy.png)
6.	Make sure you have cloned the repository. Open cmd, go to waterCoolerClientApp directory. 
7.	Open file explorer and open development.ts file in any editor and update Deployed App service URL.
8.	Run npm install command
9.	Run npm run-script build
10.	Go to build folder in Zip the content and upload in the window opened in step 5.

 
5. Set-up Authentication
3.	Note that you have the %applicationId% from the previous step (Step 1).
If do not have this value, refer this section of the Troubleshooting guide for steps to get these values.
>>Saikrishna/Naidu to create a Wiki page for Watercooler else remove this statement
4.	Go to App Registrations page here and open the Water Cooler app you created (in Step 1) from the application list.
5.	Under Manage, click on Authentication to bring up authentication settings.
i.	Add a new entry to Redirect URIs:
	Type: Web
	Redirect URI: Enter  https://%clientAppServiceURl%/auth-end for the URL e.g. https://WaterCoolerdevui.azurewebsites.net/auth-end (client app service)
ii.	Under Implicit grant, check ID tokens.
iii.	Click Save to commit your changes.
6.	Back under Manage, click on Expose an API.
i.	Click on the Set link next to Application ID URI, and change the value to  api://%clientAppServiceURL%/clientId,
e.g. api://WaterCoolerdevui.azurewebsites.net/clientId.
ii.	Click Save to commit your changes.
iii.	Click on Add a scope, under Scopes defined by this API. In the flyout that appears, enter the following values:
	Scope name:  access_as_user
	Who can consent?:  Admins and users
	Admin and user consent display name:  Access the API as the current logged-in user
	Admin and user consent description:  Access the API as the current logged-in user
iv.	Click Add scope to commit your changes.
v.	Click Add a client application, under Authorized client applications. In the flyout that appears, enter the following values:
	Client ID: 5e3ce6c0-2b1f-4285-8d4b-75ee78787346
	Authorized scopes: Select the scope that ends with access_as_user. (There should only be 1 scope in this list.)
vi.	Click Add application to commit your changes.
vii.	Repeat the previous two steps, but with client ID = 1fec8e78-bce4-4aaf-ab1b-5451cc387264. After this step you should have two client applications (5e3ce6c0-2b1f-4285-8d4b-75ee78787346 and 1fec8e78-bce4-4aaf-ab1b-5451cc387264) listed under Authorized client applications.

6. Add Permissions to your app
Continuing from the Azure AD app registration page where we ended Step 3.
1.	Select API Permissions blade from the left-hand side.
2.	Click on Add a permission button to add permission to your app.
3.	In Microsoft APIs under Select an API label, select the service and give the following permissions,
o	Under Commonly used Microsoft APIs,
o	Select “Microsoft Graph”, then select Application permissions and check the following permissions,
a.	Calls.AccessMedia.All
b.	Calls.Initiate.All
c.	Calls.InitiateGroupCall.All
d.	Calls.JoinGroupCall.All
e.	Calls.JoinGroupCallAsGuestAll
f.	OnlineMeetings.ReadWriteAll
g.	People.Read.All
h.	User.Read.All
o	then select Delegated permissions and check the following permissions,
a.	Email
b.	Offline_access
c.	OnlineMeetings.ReadWrite
d.	Openid
e.	Profile
f.	User.Read
o	Click on Add Permissions to commit your changes.
![App Badge](Wiki/Images/permissions.png)
Please refer to Solution overview for more details about the above permissions.

4.	If you are logged in as the Global Administrator, click on the “Grant admin consent for %tenant-name%” button to grant admin consent, else inform your Admin to do the same through the portal.
Alternatively you may follow the steps below:
o	Prepare link - https://login.microsoftonline.com/common/adminconsent?client_id=%appId%. Replace the %appId% with the Application (client) ID of Microsoft Teams Water Cooler bot app (from above).
o	Global Administrator can grant consent using the link above.

7. Give policy access to admin user
1.	Open powershell as system administrator
2.	Write below commands to run and provide appropriate details
a.	Import-Module MicrosoftTeams
b.	$userCredential = Get-Credential
c.	Connect-MicrosoftTeams -Credential $userCredential 
d.	New-CsApplicationAccessPolicy -Identity OnlineMeeting-WaterCooler -AppIds "%clientId%" -Description "Water cooler app - Online meeting policy for admin -Tenant"
e.	Grant-CsApplicationAccessPolicy -PolicyName OnlineMeeting-WaterCooler -Identity "%adminUserId%"



8. Create the Teams app package
Create Teams app packages: To be installed to teams.
1.	Make sure you have cloned the app repository locally.
2.	Open the  Manifest\manifest.json file in a text editor.
3.	Change the placeholder fields in the manifest to values appropriate for your organization in developer property. 
o	developer.name (What's this?)
o	developer.websiteUrl
o	developer.privacyUrl
o	developer.termsOfUseUrl
4.	Change the placeholder fields in the manifest to values appropriate to app name property
o	short: “The Water Cooler”,
o	Full: “The Water Cooler”
	
5.	Change the placeholder fields in the manifest as an example below:
o	"entityId": "Water Cooler",
o	"name": "The Water Cooler",
o	"contentUrl": https://yourappdomain.azurewebsites.net/,
o	"websiteUrl": "https://yourappdomain.azurewebsites.net/",

6.	Change the <<clientId>> placeholder in the id setting of the webApplicationInfo section to be the %clientId% value. Change the <<appDomain>> placeholder in the resource setting of the webApplicationInfo section to be the %appDomain% value e.g. "api://appname.azurewebsites.net/clientId".
>>Confirm if same format for web app info section is present in the mainfest
7.	Create a ZIP package with the  manifest.json, color.png, and outline.png. The two image files are the icons for your app in Teams.
o	Name this package  manifest.zip, so you know that this is the app for the Water Cooler.
o	Make sure that the 3 files are the top level of the ZIP package, with no nested folders.
![App Badge](Wiki/Images/manifest.png)
9. Install the apps in Microsoft Teams
1.	Install the Water Cooler app (the  manifest.zip package) with the tenant scope.
o	If your tenant has sideloading apps enabled, you can install your app by following the instructions here.
IMPORTANT:  Proactive app installation will work only if you upload the app to your tenant's app catalog.

4.	Install the app (the manifest.zip package) to the users and teams that will be the target audience.
Feedback
Thoughts? Questions? Ideas? Share them with us on Teams UserVoice!
Please report bugs and other code issues here.
Legal notice
This app template is provided under the MIT License terms. In addition to these terms, by using this app template you agree to the following:
>>We do not have a license as of now – Add a similar license file to our repo and link it here – refer to https://github.com/OfficeDev/microsoft-teams-apps-company-communicator/blob/master/LICENSE 
•	You, not Microsoft, will license the use of your app to users or organization.
•	This app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with respect to any applicable regulations, including but not limited to privacy, healthcare, employment, or financial regulations.
•	You are responsible for complying with all applicable privacy and security regulations including those related to use, collection, and handling of any personal data by your app. This includes complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.
•	Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository. Microsoft’s general trademark guidelines can be found here.
•	If the app template enables access to any Microsoft Internet-based services (e.g., Office365), use of those services will be subject to the separately provided terms of use. In such cases, Microsoft may collect telemetry data related to app template usage and operation. Use and handling of telemetry data will be performed in accordance with such terms of use.
•	Use of this template does not guarantee acceptance of your app to the Teams app store. To make this app available in the Teams app store, you will have to comply with the submission and validation process, and all associated requirements such as including your own privacy statement and terms of use for your app.
Contributing
This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.
When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.
This project has adopted the Microsoft Open Source Code of Conduct. For more information see the Code of Conduct FAQ or contact opencode@microsoft.com with any additional questions or comments.
