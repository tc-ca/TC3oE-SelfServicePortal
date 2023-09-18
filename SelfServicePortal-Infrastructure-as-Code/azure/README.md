# Self Service Portal

Terraform deploys all the azure stuff, EXCEPT for some secrets in the key vault.

The secrets:

- `PersonalAccessToken` - An Azure DevOps PAT which is used to create service connections
- `TeamsWebhookUrl` - The webhook url for the teams channel to send notificaitons to
   1. Find the channel
   2. Three dots menu
   3. Connectors
   4. Incoming Webhook

Also not managed by Terraform is the PowerAutomate workflow that manages sending the approvals to MS Teams.