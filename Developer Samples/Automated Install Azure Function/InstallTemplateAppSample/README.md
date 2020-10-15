---
page_type: sample
languages:
- csharp
products:
- Power BI
description: "Power BI material to get you started in creating & distributing template apps"
urlFragment: "automated-install-azure-function"
---

# Power BI Template App - Automated Install Sample

This developer sample encapsulates the setup of an [Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) to leverage [Power BI APIs](https://docs.microsoft.com/en-us/rest/api/power-bi/) for installing a Template App and configuring it for your users automatically.

## Prerequisites
To get started, you must have:

* You need to have your own [Azure Active Directory tenant](https://docs.microsoft.com/en-us/power-bi/developer/embedded/create-an-azure-active-directory-tenant) setup.
* A [service principal (app-only token)](https://docs.microsoft.com/en-us/power-bi/developer/embedded/embed-service-principal), registered in the above tenant.
* A parameterized [template app](https://docs.microsoft.com/en-us/power-bi/connect-data/service-template-apps-overview) that is ready to be installed by Power BI users. This template app should be available on AppSource.

    The template app should be created in the same tenant in which you register your application in Azure Active Directory (Azure AD).

    See [template app tips](https://docs.microsoft.com/en-us/power-bi/connect-data/service-template-apps-tips.md) or [Create a template app in Power BI](https://docs.microsoft.com/en-us/power-bi/connect-data/service-template-apps-create) for more information.

If you're not signed up for **Power BI Pro**, [sign up for a free trial](https://powerbi.microsoft.com/pricing/) before you begin.

## Set up your template apps automation development environment

Before you continue setting up your application, follow the [prerequisites](https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp) to develop an Azure Function along with Azure App Configuration. Create your App Configuration as described in the article above.

### Register an application in Azure Active Directory (Azure AD)

Create a [service principal](https://docs.microsoft.com/en-us/power-bi/developer/embedded/embed-service-principal).

Make sure to register the application as a **server-side web application** app. You register a server-side web application to create an application secret.

Save the *Application ID* (Client ID) and *Application secret* (Client Secret) for later steps.

You can go through the [Embedding setup tool](https://aka.ms/embedsetup/AppOwnsData), so you can quickly get started creating an app registration.

## Template App preparation

Before you start distributing your template app using automated install, make sure to publish this application to [Partner Center](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-power-bi-app-offer).

> [!Note]
> For testing purposes, you can always use automated install on applications you own to install in your own tenant. Users outside your tenant will not be able to install and configure these applications when using automated install APIs unless the app is publicly available in [Power BI Apps marketplace](https://app.powerbi.com/getdata/services).

Once you've prepared your application and its ready to be installed by your users, save the following information for the next steps:

1. *App ID*, *Package Key*, *Owner ID* as they appear in the [installation URL]() when the app was created.

    Same link can be retrieved using 'Get Link' in the app's [Release Management](https://docs.microsoft.com/en-us/power-bi/connect-data/service-template-apps-create#manage-the-template-app-release).

2. *Parameter Names* as they are defined in the app's dataset.
    Names are case-sensitive strings and can be retrieved in the Parameter Settings tab when [creating the app](https://docs.microsoft.com/en-us/power-bi/connect-data/service-template-apps-create#manage-the-template-app-release) or from the dataset settings in Power BI.

## Sample configuration

To run this sample, you would need to setup your Azure App Configuration with the values & keys as described below. Keys are defined in [Constants.cs](./InstallTemplateApp/Constants.cs).

<center>

| Configuration Key | Meaning           |
|---------------    |-------------------|
| TemplateAppInstall:Application:AppId | *AppId* from [install URL](#Template-App-Preparation) |
| TemplateAppInstall:Application:PackageKey | *PackageKey* from [install URL](#Template-App-Preparation) |
| TemplateAppInstall:Application:OwnerId | *OwnerId* from [install URL](#Template-App-Preparation) |
| TemplateAppInstall:ServicePrincipal:ClientId | *Application Id* of [service principal](#Register-an-application-in-Azure-Active-Directory-(Azure-AD)) |
| TemplateAppInstall:ServicePrincipal:ClientSecret | *Application Secret* of [service principal](#Register-an-application-in-Azure-Active-Directory-(Azure-AD)) |

</center>

## Test this function locally

Follow the steps as described in [Run the function locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio#run-the-function-locally) to start your function.

Configure your portal to issue a ```POST``` request to the url of the function (e.g. ```POST http://localhost:7071/api/install```). Request body should be a JSON object describing Key-Value pairs, where keys are *parameter names* (from [install URL](#Template-App-Preparation) and values are the desired values to set for each parameter in the template app.

>[!Note]
> Parameter values in production are to be deduced for each user by your portal's intended logic.

The desired flow should be:

1. Portal prepares the request, per user\session.
2. ```POST /api/install``` request issued to Azure Function.
3. If all is configured properly, browser should automatically redirect to Power BI and show automated install flow.
4. Upon installation, parameter values are set as configured in steps 1 & 2.

## Next Steps

Follow official [Azure Function documentation](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio#publish-the-project-to-azure) on how you can publish this project to Azure and get ready to integrate Template app automated install APIs to your product & testing it in production environments.