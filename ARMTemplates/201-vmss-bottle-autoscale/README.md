# Autoscale demo app on Ubuntu 16.04

**NOTE:** This template is a copy of the Microsoft [201-vmss-bottle-autoscale](https://github.com/Azure/azure-quickstart-templates/tree/master/application-workloads/python/vmss-bottle-autoscale) Azure Quick Start template and is provided here only to support labs on the LODS platform.

Self-contained Ubuntu autoscale example which includes a Python Bottle server to do work. The VM Scale Set scales up when average CPU across all VMs > 60%, scales down when avg CPU < 30%.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FLODSContent%2FChallengeLabs_ArmResources%2Fmaster%2FARMTemplates%2F201-vmss-bottle-autoscale%2Fazuredeploy.json)
[![Visualize](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.svg?sanitize=true)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FLODSContent%2FChallengeLabs_ArmResources%2Fmaster%2FARMTemplates%2F201-vmss-bottle-autoscale%2Fazuredeploy.json)

- Deploy the scale set with an instance count of 1.
- After it's deployed look at the resource group public IP address resource (in portal or resources explorer). Get the IP or domain name.
- Browse to the website of `vm#0` (port 9000), which shows the current backend VM name.
- To start doing work on the first VM browse to `dns:9000/do_work`.
- After a few minutes the VM Scale Set capacity will increase. Note that the first scale out takes longer than subsequent scale outs while the autoscale pipeline gets initialized. For example, wait up to 30 minutes before you conclude there's a problem.
- You can stop doing work by browsing to `dns:9000/stop_work`.

To learn more about how to deploy the template, see the [quickstart](https://docs.microsoft.com/azure/virtual-machine-scale-sets/quick-create-template-linux) article.
