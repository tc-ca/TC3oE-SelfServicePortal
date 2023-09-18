# Self Service Portal

## Running locally

```pwsh
dotnet watch run --project SelfServicePortal.Web
```

If you want to use the debugger, then you can launch through the VSCode run configuration.  
In the "Run and Debug" tab, use the ".NET Core Launch (web) (SelfServicePortal-Webapp)" run.

### Running tests

```pwsh
dotnet test
```

You can also run a specific test instead of the entire suite using

```pwsh
dotnet test --filter mytesthere
```

## Purpose

The Self Service Portal is a webapp with the goal of automating repetitive tasks for the cloud team.

With the growing scope of the cloud team's reponsibilities, some requests were being frequently submitted that followed consistent-enough patterns to be automated. In order to reduce the load on the team, the self service portal was created to facilitiate requirement gathering and change implementation. The forms in the tool tell the users exactly what information is required from them, while also making it simple for automatically performing whatever actions are required.

It was originally created around October 2021.

## Overview

Users can request a variety of changes to be made in the cloud using this tool. Once a request is submitted, a cloud team member will review it for approval. If approved, the tool will automatically perform the desired change.

There are a variety of changes (called `workflows` in the code) already implemented in the tool, and it is designed to be easily expandable as more needs are identified.

## Design

The application is built using .NET Core 6, which is multi-platform, with other teams at the org being familiar with dotnet development.

The user is presented with a form, which is then validated by the server before being converted into a sequence of Workflows to create a Workflow Request. Forms are for gathering information, Workflows are for organizing that information into the steps required to implement a change.

For example, the form for creating a resource group gets transformed into multiple Workflows as follows:

- Create a the resource group
- Create a security group
- Create a role assignment for the security group onto the resource group
- Create an Azure DevOps service connection

Some steps are conditional on the information the user enters, e.g., they may wish to reuse an existing security group instead of creating a new one. This is accomplished by simply omitting the Workflow from the Workflow Request.

With the Workflow Request constructed and saved in a table, now it must be approved by a cloud team member. To do this, an Approval Request is created. Approval Requests are separate from Workflow Requests to help separate the job of determining approval status from the job of actually executing the changes. Although Approval Requests include a summary of the Workflow Request (usually JSON in markdown), they don't have any influence beyond yielding an `approved` or `denied` result.

## Technical Walkthrough

1. User submits a workflow request on the website
1. App creates new entry in `approvalrequests` queue
1. App creates new entry in `workflows` table
1. PowerAutomate consumes entries in `approvalrequests` queue to generate MS Teams approvals (MS Teams approvals only work through PowerAutomate right now, otherwise this would have been done in code instead)
1. Cloud team member responds to MS Teams approval
1. PowerAutomate creates new entry in `approvalresponses` queue with approval outcome (approve/deny)
1. App consumes entries in `approvalresponses` queue, updates corresponding `workflows` table entry, performs workflow if approval result was `approved`

The configuration for which Azure storage account is used for approvals is defined in [appsettings.json](./appsettings.json).


## Adding a new Workflow

1. Create the web form by adding to [./Pages/Forms](./Pages/Forms)
1. Add localization for your new form in [./Resources/Pages/Forms](./Resources/Pages/Forms)
1. Define new workflow model in [./Models/Workflows](./Models/Workflows)
    - Implement the `Complete` method that describes what the workflow _does_ when approved.
1. Add the workflow to the list of known workflows in [./Models/Workflows/Workflow.cs](./Models/Workflows/Workflow.cs)
    - This is required to ensure the workflow can be deserialized properly after an approval result is obtained

## Service Princiapl Authentication

The service principal is used to authenticate the app for the operations it will be performing. Owner role assignments on subscriptions are used to give the service principal permission to create new resource groups. Right now, the workflow for requesting a new resource group allows the user to pick from all subscriptions the tool has access to.

The credential object is created in [./Startup.cs](./Startup.cs), pulling credentials from the Azure key vault that is loaded in [./Program.cs](./Program.cs) using the key vault defined in [./appsettings.json](./appsettings.json)

## User Authentication

The application uses `services.AddMicrosoftIdentityWebAppAuthentication` ([here](./Extensions/Services/Auth.cs)) to provide noninteractive SSO with the org tenant. "Admins" in the tool are defined in the Managed Application area of the Azure portal, where an Azure security group containing cloud team members has been added with the app role of "Admin". The Admin role currently only shows users additional information on the History page of the app, the list of approvers for workflows just pulls from the list of members of the approvers group rather than using the admin role.