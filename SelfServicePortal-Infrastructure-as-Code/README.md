# SelfServicePortal-Infrastructure-as-Code


**You need to manually configure two secrets in the key vault:**
- PersonalAccessToken
- TeamsWebhookUrl

## PersonalAccessToken

The application needs an Azure DevOps personal access token to enable it to create service connections, and to see which users belong to which projects.


It would be preferrable to be scoped to just the required endpoints, make sure to test the workflows if you decide to make this change.

## TeamsWebhookUrl

The application sends notifications to a configured Microsoft Teams channel.
This is configured manually by setting the `TeamsWebhookUrl` in the key vault.

To get the webhook url for a channel:
1. Channel > 3dots menu
1. Connectors
1. Incoming Webhook > configure
1. Enter name > create
1. Copy URL

To get the webhook url for a previously created connector:
1. Channel > 3dots menu
1. Connectors
1. Left nav "MANAGE" > Configured
1. Incoming Webhook > 1 Configured (dropdown)
1. MyConnection > Manage
1. Copy URL
