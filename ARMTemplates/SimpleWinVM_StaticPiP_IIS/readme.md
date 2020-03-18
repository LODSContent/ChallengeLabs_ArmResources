This repo contains an ARM template that you can use to deploy a Windows Azure VM with IIS installed. 
The default web page is customized to display the Azure VM name.

The VM is deployed with a static public IP (PiP) address. As a consequence, this template is a good candidate for use in labs that require 
a network load balancing configuration. 

**IMPORTANT: A static PiP address SKU is more expensive than a dynamic PiP address. Therefore, you should only use this
template if you need a static PiP in your lab.**
