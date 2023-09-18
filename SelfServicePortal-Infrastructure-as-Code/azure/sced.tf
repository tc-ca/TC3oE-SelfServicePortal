# This uses the module to deploy to SCED
module "dev" {
  source = "./modules/ssp-sced"
  providers = {
    azurerm = azurerm
    azuread = azuread
  }
  resource_group_name          = "my-SelfServicePortal-RGP"
  storage_account_name         = "myselfserviceportalstorageaccount"
  key_vault_name               = "myselfserviceportalkeyvault"
  log_analytics_workspace_name = "myselfserviceportallogs"
  application_insights_name    = "myselfserviceportalappinsights"
  workload_identity_name       = "myselfserviceportalworkloadidentity"
  # todo: use data ref to cluster to grab this
  workload_identity_issuer_url = "https://canadacentral.oic.prod-aks.azure.com/5555-5555-5555-5555/5555-5555-5555-5555"
  workload_identity_service_accounts = {
    "self-service-portal" = "my-workload-identity-selfserviceportal-service-account"
  }
  application_client_id       = azuread_application.main.application_id
  application_secret          = azuread_application_password.main.value
  service_principal_object_id = azuread_service_principal.main.object_id
  app_service = {
    name      = "myselfserviceportalapp"
    plan_name = "myselfserviceportalappplan"
    sku       = "B1"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development-AppService"
    }
  }
}

output "dev_mi_client_id" {
  value = module.dev.mi_client_id
}