resource "azurerm_log_analytics_workspace" "main" {
  resource_group_name = data.azurerm_resource_group.main.name
  location            = "canadacentral"
  tags                = data.azurerm_resource_group.main.tags
  name                = var.log_analytics_workspace_name
}

resource "azurerm_application_insights" "main" {
  workspace_id        = azurerm_log_analytics_workspace.main.id
  resource_group_name = data.azurerm_resource_group.main.name
  location            = "canadacentral"
  tags                = data.azurerm_resource_group.main.tags
  name                = var.application_insights_name
  application_type    = "web"
}