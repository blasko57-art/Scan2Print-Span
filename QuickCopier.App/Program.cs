using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Logging;
using QuickCopier.Core.Services;
using QuickCopier.Windows.Printing;
using QuickCopier.Windows.Scanning;

namespace QuickCopier.App;

static class Program
{
    [STAThread]
    static void Main()
    {
        // try catch for cases in which app does not start properly 
        try
        {
            // app starts here 
            // console write here will write when app actually starts
            // only for Development
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            // ----------------------------------------
            // Logging
            // ----------------------------------------

            var logProvider = new InMemoryLoggerProvider();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(logProvider);
            });

            services.AddSingleton(logProvider);

            // ----------------------------------------
            // Scanner
            // ----------------------------------------

            // Register the real scanner implementation.
            services.AddSingleton<WindowsScannerService>();

            // Expose it through the interface,
            // but put the logging decorator in front of it.
            services.AddSingleton<IScannerService>(serviceProvider =>
                new LoggingScannerService(
                    serviceProvider.GetRequiredService<WindowsScannerService>(),
                    serviceProvider.GetRequiredService<
                        ILogger<LoggingScannerService>>()));

            // ----------------------------------------
            // Printer
            // ----------------------------------------
            // different to add service compared to when no logging
            //services.AddSingleton<IPrinterService, WindowsPrinterService>();
            services.AddSingleton<WindowsPrinterService>();

            services.AddSingleton<IPrinterService>(serviceProvider =>
                new LoggingPrinterService(
                    serviceProvider.GetRequiredService<WindowsPrinterService>(),
                    serviceProvider.GetRequiredService<
                        ILogger<LoggingPrinterService>>()));

                        // ----------------------------------------
            // Copy
            // ----------------------------------------

            // Register the real CopyService separately.
            services.AddSingleton<CopyService>();

            // Expose the logging decorator as ICopyService.
            services.AddSingleton<ICopyService>(serviceProvider =>
                new LoggingCopyService(
                    serviceProvider.GetRequiredService<CopyService>(),
                    serviceProvider.GetRequiredService<
                        ILogger<LoggingCopyService>>()));

            // ----------------------------------------
            // Forms
            // ----------------------------------------

            services.AddTransient<Form1>();
            services.AddTransient<LogsForm>();

            // ----------------------------------------
            // Start application
            // ----------------------------------------

            using var serviceProvider =
                services.BuildServiceProvider();

            var form =
                serviceProvider.GetRequiredService<Form1>();

            Application.Run(form);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "QuickCopier Startup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}