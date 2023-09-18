resource "azurerm_key_vault" "main" {
  resource_group_name = data.azurerm_resource_group.main.name
  name                = var.key_vault_name
  location            = "canadacentral"
  sku_name            = "standard"
  tenant_id           = data.azurerm_client_config.main.tenant_id
  tags                = data.azurerm_resource_group.main.tags
}

###################
# KEY VAULT ACCESS POLICIES
###################
data "azuread_group" "cloudops" {
  object_id = "555-555-555-555" # cloud admins
}

resource "azurerm_key_vault_access_policy" "self" {
  key_vault_id       = azurerm_key_vault.main.id
  object_id          = data.azuread_group.cloudops.object_id
  tenant_id          = data.azurerm_client_config.main.tenant_id
  secret_permissions = ["Get", "List", "Set", "Delete", "Purge", "Recover"]
}

resource "azurerm_key_vault_access_policy" "workload_identity" {
  key_vault_id       = azurerm_key_vault.main.id
  object_id          = azurerm_user_assigned_identity.main.principal_id
  tenant_id          = data.azurerm_client_config.main.tenant_id
  secret_permissions = ["Get", "List"]
}

###################
# KEY VAULT SECRETS
###################
resource "azurerm_key_vault_secret" "main" {
  for_each = merge(
    {
      "AzureAd--ClientId"                     = var.application_client_id
      "AzureAd--ClientSecret"                 = var.application_secret
      "AzureAd--TenantId"                     = data.azurerm_client_config.main.tenant_id
      "ApplicationInsights--ConnectionString" = azurerm_application_insights.main.connection_string
      "LogWorkspaceId"                        = azurerm_log_analytics_workspace.main.workspace_id
      # "PersonalAccessToken" = set manually
      # "TeamsWebhookUrl" = set manually
    },
    var.key_vault_secrets
  )
  key_vault_id = azurerm_key_vault.main.id
  name         = each.key
  value        = each.value
  depends_on = [
    azurerm_key_vault_access_policy.self
  ]
}