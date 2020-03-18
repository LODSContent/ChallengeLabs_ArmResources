# WinVM_IIS ARM Template

This repo contains an ARM template that you can use to deploy a Windows Azure VM with IIS installed. 
The default web page is customized to display the Azure VM name.

The VM can be deployed with a static (Standard SKU) or dynamic (Basic SKU) public IP (PiP) address, depending on the value you choose for the PiP SKU paramter. As a consequence, this template is a good candidate for use in labs that require a network load balancing configuration. 

**IMPORTANT: A static PiP address SKU is more expensive than a dynamic PiP address. Therefore, you should only use the Standard PiP SKU if  you need a genuinely need a static PiP in your lab.**

