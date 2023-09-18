resource "azurerm_storage_account" "main" {
  resource_group_name             = data.azurerm_resource_group.main.name
  name                            = var.storage_account_name
  location                        = "canadacentral"
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  account_kind                    = "StorageV2"
  tags                            = data.azurerm_resource_group.main.tags
  allow_nested_items_to_be_public = false
}

resource "azurerm_storage_table" "workflows" {
  storage_account_name = azurerm_storage_account.main.name
  name                 = "workflows"
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_storage_queue" "approvalrequests" {
  storage_account_name = azurerm_storage_account.main.name
  name                 = "approvalrequests"
}

resource "azurerm_storage_queue" "approvalresponses" {
  storage_account_name = azurerm_storage_account.main.name
  name                 = "approvalresponses"
}

# allow service principal to manage data
resource "azurerm_role_assignment" "queuer" {
  scope                = azurerm_storage_account.main.id
  principal_id         = var.service_principal_object_id
  role_definition_name = "Storage Queue Data Contributor"
}
resource "azurerm_role_assignment" "tabler" {
  scope                = azurerm_storage_account.main.id
  principal_id         = var.service_principal_object_id
  role_definition_name = "Storage Table Data Contributor"
}