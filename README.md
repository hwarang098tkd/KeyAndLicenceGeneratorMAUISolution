# Key And Licence Generator
## Introduction
The Key and Licence Generator is a software tool developed using .NET MAUI that enables users to generate public and private keys and create license files using these keys. It is designed to facilitate the management and distribution of digital certificates and licenses.

### Home Page
![Key and Licence Generator Image](/KeyAndLicenceGenerator/Resources/Images/1.png)

### Keys Page
![Key and Licence Generator Image](/KeyAndLicenceGenerator/Resources/Images/2.png)

### Licenes Page
![Key and Licence Generator Image](/KeyAndLicenceGenerator/Resources/Images/3.png)

## Usage
To use the application, run the executable created in the build process and navigate through the GUI to generate keys or licenses as required.

## Features
* Key Generation: Generate public and private keys.
* License File Creation: Use the generated keys to create license files.
* USB Device Integration: Save licenses directly to a USB drive.
* File Management: Manage generated keys and licenses through the application.
* Logging: Comprehensive logging of operations using Serilog.

## Dependencies
The project relies on the following dependencies:

* .NET MAUI for building cross-platform applications.
* CommunityToolkit.Mvvm for MVVM framework support.
* Serilog for logging.
* UsbDeviceLibrary for handling USB devices (Windows only).

## Configuration
Configuration settings can be adjusted in the appsettings.json file, where you can set paths, logging preferences, and other operational parameters.
