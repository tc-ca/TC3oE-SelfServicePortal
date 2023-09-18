resource "azurerm_service_plan" "appplan" {
  count               = var.app_service == null ? 0 : 1
  resource_group_name = data.azurerm_resource_group.main.name
  location            = "canadacentral"
  name                = var.app_service.plan_name
  tags                = data.azurerm_resource_group.main.tags
  sku_name            = var.app_service.sku
  os_type             = "Windows"
}

resource "azurerm_linux_web_app" "app" {
  count               = var.app_service == null ? 0 : 1
  resource_group_name = data.azurerm_resource_group.main.name
  location            = "canadacentral"
  name                = var.app_service.name
  tags                = data.azurerm_resource_group.main.tags
  service_plan_id     = azurerm_service_plan.appplan.0.id
  https_only          = true
  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.main.id
    ]
  }


  site_config {
    always_on = false
    # ftps_state        = "AllAllowed"
    use_32_bit_worker = false
  }

  app_settings = merge({
    APPINSIGHTS_INSTRUMENTATIONKEY             = azurerm_application_insights.main.instrumentation_key
    APPLICATIONINSIGHTS_CONNECTION_STRING      = azurerm_application_insights.main.connection_string
    ApplicationInsightsAgent_EXTENSION_VERSION = "~2"
  }, var.app_service.app_settings)
}