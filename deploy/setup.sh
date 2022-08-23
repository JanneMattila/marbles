# Enable auto export
set -a

# All the variables for the deployment
subscription_name="AzureDev"
azuread_admin_group_contains="janne''s"

aks_name="thundernetes"
premium_storage_name="thundernetes00010"
premium_storage_share_name_nfs="nfs"
workspace_name="log-thundernetesworkspace"
vnet_name="vnet-thundernetes"
subnet_aks_name="snet-aks"
subnet_storage_name="snet-storage"
subnet_netapp_name="snet-netapp"
cluster_identity_name="id-thundernetes-cluster"
kubelet_identity_name="id-thundernetes-kubelet"
resource_group_name="rg-thundernetes"
location="swedencentral"

# Login and set correct context
az login -o table
az account set --subscription $subscription_name -o table

# Prepare extensions and providers
az extension add --upgrade --yes --name aks-preview

# Start deployment
az group create -l $location -n $resource_group_name -o table

azuread_admin_group_id=$(az ad group list --display-name $azuread_admin_group_contains --query [].id -o tsv)
echo $azuread_admin_group_id

workspace_id=$(az monitor log-analytics workspace create -g $resource_group_name -n $workspace_name --query id -o tsv)
echo $workspace_id

vnet_id=$(az network vnet create -g $resource_group_name --name $vnet_name \
  --address-prefix 10.0.0.0/8 \
  --query newVNet.id -o tsv)
echo $vnet_id

subnet_aks_id=$(az network vnet subnet create -g $resource_group_name --vnet-name $vnet_name \
  --name $subnet_aks_name --address-prefixes 10.2.0.0/24 \
  --query id -o tsv)
echo $subnet_aks_id

subnet_storage_id=$(az network vnet subnet create -g $resource_group_name --vnet-name $vnet_name \
  --name $subnet_storage_name --address-prefixes 10.3.0.0/24 \
  --query id -o tsv)
echo $subnet_storage_id

cluster_identity_json=$(az identity create --name $cluster_identity_name --resource-group $resource_group_name -o json)
kubelet_identity_json=$(az identity create --name $kubelet_identity_name --resource-group $resource_group_name -o json)
cluster_identity_id=$(echo $cluster_identity_json | jq -r .id)
kubelet_identity_id=$(echo $kubelet_identity_json | jq -r .id)
kubelet_identity_object_id=$(echo $kubelet_identity_json | jq -r .principalId)
echo $cluster_identity_id
echo $kubelet_identity_id
echo $kubelet_identity_object_id

az aks get-versions -l $location -o table

# Note: for public cluster you need to authorize your ip to use api
my_ip=$(curl --no-progress-meter https://api.ipify.org)
echo $my_ip

# Not used parameters:
# --zones 1 2 3

aks_json=$(az aks create -g $resource_group_name -n $aks_name \
 --max-pods 50 --network-plugin azure \
 --node-count 1 --enable-cluster-autoscaler --min-count 1 --max-count 3 \
 --node-osdisk-type Ephemeral \
 --node-vm-size Standard_D8ds_v4 \
 --kubernetes-version 1.24.3 \
 --enable-addons monitoring,azure-policy,azure-keyvault-secrets-provider \
 --enable-aad \
 --enable-azure-rbac \
 --disable-local-accounts \
 --aad-admin-group-object-ids $azuread_admin_group_id \
 --workspace-resource-id $workspace_id \
 --load-balancer-sku standard \
 --vnet-subnet-id $subnet_aks_id \
 --assign-identity $cluster_identity_id \
 --assign-kubelet-identity $kubelet_identity_id \
 --api-server-authorized-ip-ranges $my_ip \
 --enable-node-public-ip \
 -o json)

aks_node_resource_group_name=$(echo $aks_json | jq -r .nodeResourceGroup)
aks_node_resource_group_id=$(az group show --name $aks_node_resource_group_name --query id -o tsv)
echo $aks_node_resource_group_id

aks_nsg_name=$(az network nsg list --resource-group $aks_node_resource_group_name --query name -o tsv)
echo $aks_nsg_name

# Enable game server ports in our network security group
az network nsg rule create \
  --resource-group $aks_node_resource_group_name \
  --nsg-name $aks_nsg_name \
  --name AKSThundernetesGameServerRule \
  --access Allow \
  --protocol "*" \
  --direction Inbound \
  --priority 1000 \
  --source-port-range "*" \
  --destination-port-range 10000-12000

###################################################################
# Enable current ip
az aks update -g $resource_group_name -n $aks_name --api-server-authorized-ip-ranges $my_ip

# Clear all authorized ip ranges
az aks update -g $resource_group_name -n $aks_name --api-server-authorized-ip-ranges ""
###################################################################

sudo az aks install-cli
az aks get-credentials -n $aks_name -g $resource_group_name --overwrite-existing
kubelogin convert-kubeconfig -l azurecli

kubectl get nodes
kubectl get nodes -o wide

# Create namespace
kubectl apply -f namespace.yaml

# Wipe out the resources
az group delete --name $resource_group_name -y
