# Sigger

> Important note: sigger is currently still under development and cannot yet be used as a product. 
> Since the interface declarations and interfaces are not yet sufficiently stable


Sigger is essentially a code generator that generates client code from a SignalR hub. 
A schema file is generated for the application, which then generates client code via an npm application.
The name is derived from swagger (see https://github.com/domaindrivendev/Swashbuckle.AspNetCore) but for signalR.

## What is Sigger

>
> **Important: Sigger just supports .net6 or higher**
>

Sigger consists of several parts: 

 - One is the backend part, which is responsible for parsing the SignalR hubs and generating the schema 
   files and also provides some auxiliary functions for the SignalR interfaces.
   
 - The second part is the client, which is responsible for generating the client-side code. 
   This part will gradually be extended by various code generators.
  
 - Sigger Extensions provide extension functions and services that are often required for Sigger applications. For example, a user-topic registry.

 - Sigger UI Provides an interface to test the Sigger interface. 
 
 > Since I mainly use Angular, the first step only provides a generator for Angular clients. However, 
 > I will make sure that it can be adapted as easily as possible for other generators.
 
## Getting started

 https://github.com/voggue/sigger/blob/main/doc/GettingStarted.md
