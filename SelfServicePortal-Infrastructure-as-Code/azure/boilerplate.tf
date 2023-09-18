terraform {
  backend "azurerm" {
    resource_group_name  = "my-terraform-resource-group"
    storage_account_name = "my-terraform-storage-account"
    container_name       = "my-container"
    subscription_id      = "5555-5555-5555-5555"
    key                  = "selfserviceportal.tfstate"
  }
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

provider "azurerm" {
  features {}
  subscription_id = "5555-5555-5555-5555"
}

data "azurerm_client_config" "main" {}