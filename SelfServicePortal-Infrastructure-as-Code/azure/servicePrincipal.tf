resource "azuread_application" "main" {
  display_name = "myselfserviceportal"
  app_role {
    description  = "Allows for the approval and denial of workflow requests."
    display_name = "Admin"
    value        = "admin"
    allowed_member_types = [
      "Application",
      "User"
    ]
  }

  group_membership_claims = ["None"]
  owners = [
    for x in data.azuread_user.ops_owners :
    x.object_id
  ]

  # see https://registry.terraform.io/providers/hashicorp/azuread/latest/docs/resources/application#resource_app_id 
  # az ad sp show --id 00000003-0000-0000-c000-000000000000 --query "appRoles"
  # az ad sp show --id 00000002-0000-0000-c000-000000000000 --query "appRoles"
  # $graphId = az ad sp list --query "[?appDisplayName=='Microsoft Graph'].appId | [0]" --all
  # az ad sp show --id $graphId --query "appRoles[].{Value:value, Id:id}" --output table | Select-String Directory.ReadWrite.All 
  # https://simonagren.github.io/azcli-adscope/
  required_resource_access {
    resource_app_id = "00000002-0000-0000-c000-000000000000" # azure graph (legacy)

    resource_access {
      # Application.ReadWrite.All
      id   = "1cda74f2-2616-4834-b122-5cb1b07f8a59"
      type = "Role"
    }
    resource_access {
      # Application.ReadWrite.OwnedBy
      # Allows service principals to create other service principals
      # see: https://github.com/Azure/azure-cli/issues/12939
      id   = "824c81eb-e3f8-4ee6-8f6d-de7f50d565b7"
      type = "Role"
    }
    resource_access {
      # Directory.ReadWrite.All
      id   = "78c8a3c8-a07e-4b9e-af1b-b5ccab50a175"
      type = "Role"
    }
  }
  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000" # microsoft graph

    resource_access {
      # User.Read
      id   = "e1fe6dd8-ba31-4d61-89e7-88639da4683d"
      type = "Scope"
    }
    resource_access {
      # GroupMember.Read.All
      id   = "bc024368-1153-4739-b217-4326f2e966d0"
      type = "Scope"
    }

    resource_access {
      id   = "1bfefb4e-e0b5-418b-a88f-73c46d2cc8e9"
      type = "Role"
    }
    resource_access {
      # Directory.ReadWrite.All
      id   = "19dbc75e-c2e2-444c-a770-ec69d8559fc7"
      type = "Role"
    }
  }
  web {
    redirect_uris = [
      "http://localhost:5001/ssp-pls/signin-oidc",
      "https://cloud.org.gc.ca/ssp-pls/signin-oidc",
    ]
    implicit_grant {
      access_token_issuance_enabled = false
      id_token_issuance_enabled     = true
    }
  }
}

resource "azuread_service_principal" "main" {
  application_id = azuread_application.main.application_id
  owners         = azuread_application.main.owners
  description    = "Managed by Terraform"
}

resource "azuread_app_role_assignment" "main" {
  resource_object_id  = azuread_service_principal.main.object_id
  app_role_id         = [for x in azuread_application.main.app_role : x.id if x.value == "admin"][0]
  principal_object_id = azuread_group.approvers.object_id
}

resource "azuread_application_password" "main" {
  application_object_id = azuread_application.main.object_id
}

##################
# ROLE ASSIGNMENTS
##################
resource "azurerm_role_assignment" "main" {
  for_each = {
    SUB1 = "555-555-555-555"
    SUB2 = "555-555-555-555"
    SUB3 = "555-555-555-555"
    SUB4 = "555-555-555-555"
  }
  scope                = "/subscriptions/${each.value}"
  role_definition_name = "Owner"
  principal_id         = azuread_service_principal.main.id
}

