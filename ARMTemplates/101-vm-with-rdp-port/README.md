# Create a VM with an RDP port
**NOTE:** This template is a copy of the Microsoft [101-vm-with-rdp-port](https://github.com/Azure/azure-quickstart-templates/tree/master/101-vm-with-rdp-port) Azure Quick Start template and is provided here only to support labs on the LODS platform.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FLODSContent%2FChallengeLabs_ArmResources%2Fmaster%2FARMTemplates%2F101-vm-with-rdp-port%2Fazuredeploy.json) 

[![Visualize](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.svg?sanitize=true)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FLODSContent%2FChallengeLabs_ArmResources%2Fmaster%2FARMTemplates%2F101-vm-with-rdp-port%2Fazuredeploy.json)

This sample template demonstrates how to create a NAT rule in loadbalancer to allow RDP to a vm.

You can connect to the vm using:

* mstsc.exe /v:&lt;publicDnsName&gt;.&lt;location&gt;.cloudapp.azure.com:&lt;rdpPort&gt;

This template deploys the following resources:
<ul><li>storage account</li><li>vnet</li><li>public ip</li><li>load balancer with a NAT rule for RDP</li><li>a virtual machine</li></ul>




