terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.34.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = ">=2.31.0"
    }
  }
}

data "azurerm_client_config" "main" {

}

variable "key_vault_secrets" {
  type        = map(string)
  description = "Additional secrets to be added to the key vault"
  default     = {}
}

variable "resource_group_name" {
  type        = string
  description = "The name of the resource group in which to create the resources."
}

data "azurerm_resource_group" "main" {
  name = var.resource_group_name
}

variable "storage_account_name" {
  type        = string
  description = "The name of the storage account to be created for storing workflows and stuff."
}

variable "key_vault_name" {
  type        = string
  description = "Name of the key vault to store secrets."
}

variable "workload_identity_name" {
  type        = string
  description = "Name of the managed identity to be created for workload identity authentication to the cluster."
}

variable "workload_identity_service_accounts" {
  type        = map(string)
  description = "Map of service accounts to be created for workload identity authentication to the cluster. The key is the namespace and the value is the service account name."
}

variable "workload_identity_issuer_url" {
  type        = string
  description = "Issuer URL of the cluster's OIDC provider."
}

variable "log_analytics_workspace_name" {
  type = string
}

variable "application_insights_name" {
  type = string
}

variable "application_client_id" {
  type = string
}

variable "application_secret" {
  type = string
}

variable "service_principal_object_id" {
  type = string
}

variable "app_service" {
  type = object({
    plan_name    = string
    name         = string
    sku          = string
    app_settings = map(string)
  })
  default = null
}