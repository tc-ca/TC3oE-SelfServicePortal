resource "azuread_group" "main" {
  display_name     = "SelfServicePortal-RBAC-Groups"
  security_enabled = true
  description      = "Managed by Terraform"
}

####################
## Approver group
####################
data "azuread_user" "approvers_members" {
  for_each = toset([
    "first.last@org.gc.ca",
  ])
  user_principal_name = each.key
}
data "azuread_user" "approvers_owners" {
  for_each = toset([
    "first.last@org.gc.ca",
  ])
  user_principal_name = each.key
}

resource "azuread_group" "approvers" {
  display_name     = "SelfServicePortal-Approvers"
  security_enabled = true
  owners           = [for x in data.azuread_user.approvers_owners : x.object_id]
  members          = [for x in data.azuread_user.approvers_members : x.object_id]
  description      = "Managed by Terraform"
}
resource "azuread_group_member" "approvers" {
  group_object_id  = azuread_group.main.object_id
  member_object_id = azuread_group.approvers.id
}

####################
## K8s operator group
####################
data "azuread_user" "ops_members" {
  for_each = toset([
    "first.last@org.gc.ca"
  ])
  user_principal_name = each.key
}
data "azuread_user" "ops_owners" {
  for_each = toset([
    "first.last@org.gc.ca",
  ])
  user_principal_name = each.key
}

resource "azuread_group" "ops" {
  display_name     = "SelfServicePortal-Operators"
  security_enabled = true
  owners           = [for x in data.azuread_user.ops_owners : x.object_id]
  members          = [for x in data.azuread_user.ops_members : x.object_id]
  description      = "Managed by Terraform"
}
resource "azuread_group_member" "ops" {
  group_object_id  = azuread_group.main.object_id
  member_object_id = azuread_group.ops.id
}
