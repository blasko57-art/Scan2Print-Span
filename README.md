QuickCopier

QuickCopier is a Windows desktop application for copying documents through connected scanners and printers on different devices.

The project is built with C# and .NET 10 and focuses on clean architecture, separation of responsibilities, dependency injection, logging, testability, and Windows hardware integration.

Features
Detect and select connected scanners
Detect and select connected printers
Copy documents directly from scanner to printer
Keep scanned documents in application memory only
Inspect scanner capabilities
View application logs and status through the UI
In-memory application logging
Dependency injection
Automated unit tests
Static code analysis
Current Workflow

The main application supports three operations.

Scan
Select a scanner.
Press SCAN.
The document is scanned and stored in application memory.
The document becomes available for printing.
Print
Scan a document first.
Select a printer.
Press PRINT.
The scanned document is sent to the selected printer.
The document is released from memory after printing.
Copy
Select a scanner.
Select a printer.
Press COPY.
The application directly scans the document and sends it to the selected printer.

Scanned documents are kept in application memory only and are not persisted to disk.

Architecture

The solution is divided into separate projects according to responsibility:

QuickCopier
│
├── QuickCopier.App
│   ├── UI
│   ├── State
│   └── Workflows
│
├── QuickCopier.Core
│   ├── Interfaces
│   ├── Models
│   ├── Services
│   └── Logging
│
├── QuickCopier.Windows
│   ├── Scanning
│   └── Printing
│
└── QuickCopier.Tests

QuickCopier.App

Contains the Windows Forms application, UI components, application state, and high-level document workflows.

QuickCopier.Core

Contains application-independent models, interfaces, services, and logging abstractions.

The Core project defines the application's contracts without depending on Windows-specific implementations.

QuickCopier.Windows

Contains Windows-specific hardware integration.

Scanner functionality currently uses WIA through the NAPS2.Wia library.

This project contains implementations such as:

WindowsScannerService
WindowsScannerCapabilities
WindowsPrinterService
QuickCopier.Tests

Contains automated tests for Core services, models, logging decorators, and service behavior.

The project uses xUnit.

Dependency Injection

The application uses Microsoft.Extensions.DependencyInjection to register services and UI components.

The architecture separates concrete Windows implementations from the interfaces consumed by the rest of the application.

Logging decorators are also registered through dependency injection.

For example:

IScannerService
      │
      ▼
LoggingScannerService
      │
      ▼
WindowsScannerService


The same pattern is used for printing and copying.

This allows logging and other cross-cutting concerns to remain separate from the underlying hardware implementation.

Logging

QuickCopier includes an in-memory logging system.

Application operations such as:

scanning
printing
copying
device discovery
successful operations
failures

can be recorded and displayed through the application's Logs window.

Logging is implemented separately from the underlying hardware services so that operational diagnostics do not have to be mixed into the hardware implementation itself.

Scanner Capabilities

The application includes a scanner inspection feature.

After selecting a scanner device, the Inspect button can be used to examine the capabilities reported by the connected scanner.

This is useful because scanner hardware can expose different capabilities depending on the device and driver.

Testing

The project contains automated tests covering:

Core services
Models
Logging
Error handling
Dependency injection
Resource disposal
Service behavior

The test project uses xUnit.

Code Quality

Development-time analyzers are configured centrally through Directory.Build.props.

Centralizing the analyzers avoids repeating the same configuration in every project file.

The project currently uses:

SonarAnalyzer
Meziantou.Analyzer
Roslynator.Analyzers
IDisposableAnalyzers
Microsoft .NET analyzers for tests
Technologies
C#
.NET 10
Windows Forms
Windows WIA
NAPS2.Wia
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Logging
xUnit
SonarAnalyzer
Meziantou.Analyzer
Roslynator
IDisposableAnalyzers
Requirements

QuickCopier currently targets Windows and has folowing requirements:

Windows
.NET 10
A compatible device for scanning functionality 
A configured Windows printer for printing functionality
Current Hardware Limitation

Hardware
Scanning is currently supported through flatbed scanners.
Feeder/multi-page scanning and Photo-Film scanning are planned for future versions.
You should not select same device for printing and scanning.

Support for other scanner types may require additional implementation depending on the hardware and driver.

Running the Application
Clone the repository.
Open the solution in Visual Studio.
Build the solution.
Set QuickCopier.App as the startup project.
Run the application.

A compatible scanner and configured printer are required for the hardware functionality to work.

Project Status

QuickCopier is an ongoing project.

The core scanning, printing, copying, logging, dependency injection, testing, and UI workflows are implemented.

Further improvements and additional scanner functionality are planned for future versions.