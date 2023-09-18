resource "azurerm_user_assigned_identity" "main" {
  resource_group_name = data.azurerm_resource_group.main.name
  location            = "canadacentral"
  name                = var.workload_identity_name
  tags                = data.azurerm_resource_group.main.tags
}

resource "azurerm_federated_identity_credential" "main" {
  for_each            = var.workload_identity_service_accounts
  resource_group_name = data.azurerm_resource_group.main.name
  parent_id           = azurerm_user_assigned_identity.main.id
  name                = each.key
  issuer              = var.workload_identity_issuer_url
  subject             = "system:serviceaccount:${each.key}:${each.value}"
  audience            = ["api://AzureADTokenExchange"]
}