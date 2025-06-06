---
page_type: sample
description: Add this sample app to enable anonymous user participation and interaction in Microsoft Teams meetings.
products:
- office-teams
- office
- office-365
languages:
- nodejs
extensions:
 contentType: samples
 createdDate: "22-02-2023 10:00:01"
urlFragment: officedev-microsoft-teams-samples-app-anonymous-users-nodejs

---

## Anonymous User Support in Microsoft Teams Apps

This sample demonstrates how to enable anonymous user support in Microsoft Teams meeting apps. It provides guidance on configuring and integrating various Microsoft services, like Azure Active Directory (AAD) and the Microsoft Bot Framework, to allow guest users to interact with meeting apps seamlessly.

**Interaction with app**
![appanonymoususersGif](Images/anonymoususersupport.gif)

## Prerequisites

- [Node.js](https://nodejs.org)

    ```bash
    # determine node version
    node --version
    ```
- [dev tunnel](https://learn.microsoft.com/en-us/azure/developer/dev-tunnels/get-started?tabs=windows) or [ngrok](https://ngrok.com/) latest version or equivalent tunnelling solution
- [Teams](https://teams.microsoft.com) Microsoft Teams is installed and you have an account
- [Microsoft 365 Agents Toolkit for VS Code](https://marketplace.visualstudio.com/items?itemName=TeamsDevApp.ms-teams-vscode-extension) or [TeamsFx CLI](https://learn.microsoft.com/microsoftteams/platform/toolkit/teamsfx-cli?pivots=version-one)

## Run the app (Using Microsoft 365 Agents Toolkit for Visual Studio Code)

The simplest way to run this sample in Teams is to use Teams Toolkit for Visual Studio Code.
1. Ensure you have downloaded and installed [Visual Studio Code](https://code.visualstudio.com/docs/setup/setup-overview)
1. Install the [Microsoft 365 Agents Toolkit extension](https://marketplace.visualstudio.com/items?itemName=TeamsDevApp.ms-teams-vscode-extension)
1. Select **File > Open Folder** in VS Code and choose this samples directory from the repo
1. Using the extension, sign in with your Microsoft 365 account where you have permissions to upload custom apps
1. Select **Debug > Start Debugging** or **F5** to run the app in a Teams web client.
1. In the browser that launches, select the **Add** button to install the app to Teams.
> If you do not have permission to upload custom apps (uploading), Microsoft 365 Agents Toolkit will recommend creating and using a Microsoft 365 Developer Program account - a free program to get your own dev environment sandbox that includes Teams.

## Setup

> Note these instructions are for running the sample on your local machine.

1. Run ngrok - point to port 3978

   ```bash
   ngrok http 3978 --host-header="localhost:3978"
   ```  

   Alternatively, you can also use the `dev tunnels`. Please follow [Create and host a dev tunnel](https://learn.microsoft.com/en-us/azure/developer/dev-tunnels/get-started?tabs=windows) and host the tunnel with anonymous user access command as shown below:

   ```bash
   devtunnel host -p 3978 --allow-anonymous
   ```

2. Setup

 ### Register you app with Azure AD.

  1. Register a new application in the [Microsoft Entra ID – App Registrations](https://go.microsoft.com/fwlink/?linkid=2083908) portal.
  2. Select **New Registration** and on the *register an application page*, set following values:
      * Set **name** to your app name.
      * Choose the **supported account types** (any account type will work)
      * Leave **Redirect URI** empty.
      * Choose **Register**.
  3. On the overview page, copy and save the **Application (client) ID, Directory (tenant) ID**. You’ll need those later when updating your Teams application manifest and in the .env.
  4. Under **Manage**, select **Expose an API**. 
  5. Select the **Set** link to generate the Application ID URI in the form of `api://{base-url}/botid-{AppID}`. Insert your fully qualified domain name (with a forward slash "/" appended to the end) between the double forward slashes and the GUID. The entire ID should have the form of: `api://fully-qualified-domain-name/botid-{AppID}`
      * ex: `api://%ngrokDomain%.ngrok-free.app/botid-00000000-0000-0000-0000-000000000000`.
  6. Select the **Add a scope** button. In the panel that opens, enter `access_as_user` as the **Scope name**.
  7. Set **Who can consent?** to `Admins and users`
  8. Fill in the fields for configuring the admin and user consent prompts with values that are appropriate for the `access_as_user` scope:
      * **Admin consent title:** Teams can access the user’s profile.
      * **Admin consent description**: Allows Teams to call the app’s web APIs as the current user.
      * **User consent title**: Teams can access the user profile and make requests on the user's behalf.
      * **User consent description:** Enable Teams to call this app’s APIs with the same rights as the user.
  9. Ensure that **State** is set to **Enabled**
  10. Select **Add scope**
      * The domain part of the **Scope name** displayed just below the text field should automatically match the **Application ID** URI set in the previous step, with `/access_as_user` appended to the end:
          * `api://[ngrokDomain].ngrok-free.app/botid-00000000-0000-0000-0000-000000000000/access_as_user.
  11. In the **Authorized client applications** section, identify the applications that you want to authorize for your app’s web application. Each of the following IDs needs to be entered:
      * `1fec8e78-bce4-4aaf-ab1b-5451cc387264` (Teams mobile/desktop application)
      * `5e3ce6c0-2b1f-4285-8d4b-75ee78787346` (Teams web application)
  12. Navigate to **API Permissions**, and make sure to add the follow permissions:
  -   Select Add a permission
  -   Select Microsoft Graph -\> Delegated permissions.
      - `User.Read` (enabled by default)
  -   Click on Add permissions. Please make sure to grant the admin consent for the required permissions.
  13. Navigate to **Authentication**
      If an app hasn't been granted IT admin consent, users will have to provide consent the first time they use an app.
  - Set a redirect URI:
      * Select **Add a platform**.
      * Select **Single-page application**.
      * Enter the **redirect URI** for the app in the following format: `https://{Base_Url}/auth-end` and `https://{Base_Url}/auth-start`.
  14.  Navigate to the **Certificates & secrets**. In the Client secrets section, click on "+ New client secret". Add a description(Name of the secret) for the secret and select “Never” for Expires. Click "Add". Once the client secret is created, copy its value, it need to be placed in the .env.

  15. Create a Bot Registration
    - Register a bot with Azure Bot Service, following the instructions [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart-registration?view=azure-bot-service-3.0).
    - Ensure that you've [enabled the Teams Channel](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0)
    - While registering the bot, use `https://<your_tunnel_domain>/api/messages` as the messaging endpoint.

16. To test facebook auth flow [create a facebookapp](FacebookAuth/README.md) and get client id and secret for facebook app.
    Now go to your bot channel registartion -> configuration -> Add OAuth connection string
   - Provide connection Name : for eg `facebookconnection`. You will use this name in your bot in the .env file.
   - Select service provider as `facebook`
   - Add clientid and secret of your facebook app that was created using Step 16.

### Setup code       

3. Clone the repository

    ```bash
    git clone https://github.com/OfficeDev/Microsoft-Teams-Samples.git
    ```
    
4. Open .env file from this path folder `samples/app-anonymous-users/nodejs/api/server` and update 
   - `{{Microsoft-App-id}}` - Generated from Step 1 (Application (client) ID)is the application app id
   - `{{MicrosoftAppPassword}}` - Generated from Step 1.14, also referred to as Client secret
   - `{{TenantId}}` - Generated from Step 1(Directory (tenant) ID) is the tenant id
   - `{{FacebookAppId}} and {{FacebookAppPassword}}`- Generated from step 16.
   - `{{domain-name}}` - Your domain name. E.g. if you are using ngrok it would be `https://1234.ngrok-free.app` then your domain-name will be `1234.ngrok-free.app` and if you are using dev tunnels then your domain will be like: `12345.devtunnels.ms`.

5. Open .env file from this path folder `samples/app-anonymous-users/nodejs/ClientApp` and update 
   - `{{Microsoft-App-id}}` - Generated from Step 1 (Application (client) ID)is the application app id
   - `{{FacebookAppId}}`- Generated from step 16.
     
6. We have two different solutions to run, so follow below steps:
 
- In a terminal, navigate to `samples/app-anonymous-users/nodejs/api` folder, Open your local terminal and run the below command to install node modules. You can do the same in Visual studio code terminal by opening the project in Visual studio code

    ```bash
    npm install
    ```

    ```bash
    npm start
    ```
- The server will start running on 3000 port

- In a different terminal, navigate to `samples/app-anonymous-users/nodejs/ClientApp` folder, Open your local terminal and run the below command to install node modules. You can do the same in Visual studio code terminal by opening the project in Visual studio code 

    ```bash
    npm install
    ```

    ```bash
    npm start
    ```
- The client will start running on 3978 port

**Note:**
-   If you are facing any issue in your app,  [please uncomment this line](https://github.com/OfficeDev/Microsoft-Teams-Samples/blob/main/samples/app-anonymous-users/nodejs/api/server/index.js#L154) and put your debugger for local debug.

7. __*This step is specific to Teams.*__

- **Edit** the `manifest.json` contained in the  `appManifest` folder to replace your Microsoft App Id `<<YOUR-MICROSOFT-APP-ID>>` (that was created when you registered your bot earlier) *everywhere* you see the place holder string `<<YOUR-MICROSOFT-APP-ID>>` (depending on the scenario the Microsoft App Id may occur multiple times in the `manifest.json`)

- **Edit** the `manifest.json` for `{{domain-name}}` with base Url domain. E.g. if you are using ngrok it would be `https://1234.ngrok-free.app` then your domain-name will be `1234.ngrok-free.app` and if you are using dev tunnels then your domain will be like: `12345.devtunnels.ms`.

- **Zip** up the contents of the `appManifest` folder to create a `manifest.zip` (Make sure that zip file does not contains any subfolder otherwise you will get error while uploading your .zip package)

- **Upload** the `manifest.zip` to Teams (In Teams Apps/Manage your apps click "Upload an app". Browse to and Open the .zip file. At the next dialog, click the Add button.)

- Add the app to team/groupChat scope (Supported scopes). 

**Note:**
-   If you are facing any issue in your app,  [please uncomment this line](https://github.com/OfficeDev/Microsoft-Teams-Samples/blob/main/samples/app-anonymous-users/nodejs/api/server/index.js#L156) and put your debugger for local debug.

## Running the sample

You can interact with Teams Tab meeting sidepanel.

**Install app:**

![InstallApp ](Images/InstallApp.png)

**Add to a meeting:**

![AddMeetingInstall ](Images/AddMeetingInstall.png)

**Select meeting:**

![AddToMeeting ](Images/AddToMeeting.png)

**Add app in a meeting tab:**

![MeetingTab ](Images/MeetingTab.png)

**Select vote:**

![Vote0 ](Images/Vote0.png)

**Vote UI:**

![VotePage1 ](Images/VotePage1.png)

**Submit vote:**

![SubmitVote2 ](Images/SubmitVote2.png)

**Select CreateConversation:**

![CreateConversation3 ](Images/CreateConversation3.png)

**All message have been send and user count:**

![SendConver7 ](Images/SendConver7.png)

**Send user conversation mmessage:**

![SendAllUserConver5 ](Images/SendAllUserConver5.png)

**Click the "Share Invite" button, copy the URL, open the URL in a new tab, and set up a guest account:**

![CopyMeetingLink8 ](Images/CopyMeetingLink8.png)

**Accept guest user:**

![AccepetGuestUser10 ](Images/AccepetGuestUser10.png)

**CreateConversation guest user:**

![CreateConverAnoUser11 ](Images/CreateConverAnoUser11.png)

**Add app:**

![AddApp12 ](Images/AddApp12.png)

**Share to stage view:**

![SharePage14 ](Images/SharePage14.png)

**Click share to stage view:**

![UserOneSharing15 ](Images/UserOneSharing15.png)

**Screen visible anonymous users:**

![AnoUserSharing16 ](Images/AnoUserSharing16.png)

**Click sign-in button**

![Clicksigninbutton ](Images/Clicksigninbutton.png)

**Click sign-in button anonymous users:**

![ClicksigninbuttonAnoUser ](Images/ClicksigninbuttonAnoUser.png)

**Anonymous user success page:**

![AnoUsersigninucess ](Images/AnoUsersigninucess.png)

**Click submit vote button:**

![SubmitVote ](Images/SubmitVote.png)

**Anonymous user showing the count:**

![AnoUserpage ](Images/AnoUserpage.png)

**Click submit vote button anonymous users:**

![submitVoteAno ](Images/submitVoteAno.png)

**User showing the count:**

![Resultscreen ](Images/Resultscreen.png)

**Anonymous user showing the count:**

![Resultscreenano ](Images/Resultscreenano.png)

**Remove guest user:**

![RemoveGuestuser21 ](Images/RemoveGuestuser21.png)

**Confirm message:**

![RemovePop22 ](Images/RemovePop22.png)

**The anonymous user was removed from team:**

![SuccessRemovedMessage23 ](Images/SuccessRemovedMessage23.png)

**Remove tenant user:**

![RemoveNormalUser24 ](Images/RemoveNormalUser24.png)

**The tenant user was removed from team:**

![UserRemoverSuccess25 ](Images/UserRemoverSuccess25.png)

**Add users:**

![Adduser26 ](Images/Adduser26.png)

**Welcome to the team:**

![SuccessAddUser27 ](Images/SuccessAddUser27.png)


## Further reading

- [Build apps for anonymous users](https://learn.microsoft.com/en-us/microsoftteams/platform/apps-in-teams-meetings/build-apps-for-anonymous-user?branch=pr-en-us-7318&tabs=javascript)
- [Authentication basics](https://docs.microsoft.com/microsoftteams/platform/concepts/authentication/authentication)
- [Create facebook app for development](https://developers.facebook.com/docs/development/create-an-app/)


<img src="https://pnptelemetry.azurewebsites.net/microsoft-teams-samples/samples/app-anonymous-users-nodejs" />