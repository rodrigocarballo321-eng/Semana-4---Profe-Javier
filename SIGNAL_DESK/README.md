# SIGNAL_DESK - Operational Incident & Ticket Management System

## Description
SIGNAL_DESK is a desktop application built with .NET MAUI designed to monitor, log, and manage operational tickets and system incidents in real time. It provides a centralized dashboard where system administrators and operators can track issues, filter events by priority level, update resolution status, and manage ticket workflows.

## Prerequisites & Requirements
- **.NET SDK** (.NET 9.0)
- **IDE:** Visual Studio 2022 (with .NET MAUI workload installed)
- **OS:** Windows 10/11

## Project Structure
```text
SIGNAL_DESK/
├── Models/         # Data models (Ticket.cs)
├── ViewModels/     # Presentation logic (MainViewModel.cs)
├── Views/          # UI pages (MainPage.xaml)
├── Platforms/      # Platform-specific code
├── Resources/      # Images, fonts, and styles
├── MauiProgram.cs  # Application startup configuration
└── README.md       # Project documentation