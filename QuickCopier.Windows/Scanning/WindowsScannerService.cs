//using System.IO;
//using System.Linq;
//<ImplicitUsings>enable</ImplicitUsings>
// replaced them

// only  supports flatbed
// TODO add Feeder scanner (A scanner only exposes an ADF)
using NAPS2.Wia;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;
// Will probably use WIA
// that could make enum of scanners wiindows exposes
// later ,aybe Windows COM interface
namespace QuickCopier.Windows.Scanning;


public class WindowsScannerService : IScannerService
{
    // Get  ALL WIA devices currently visible to Windows.
    // Later  filter specifically for scanners.



    /* Standard WIA device properties unused now, 

    WIA_DIP_DEV_ID    = 2
WIA_DIP_DEV_NAME  = 7
WIA_DIP_DEV_TYPE  = 5
    private const int WiaDeviceType = 5;
    private const int WiaDeviceId = 2;
    private const int WiaDeviceName = 7;
    // WIA device type 1 = scanner
    private const int WiaDeviceTypeProperty = 5;
    private const int ScannerDeviceType = 1; */
    // not trully  asynchronous, alsmost everything nside is synchronous (but not important for now)
    public Task<IReadOnlyList<ScannerDevice>> GetScannersAsync()
    {
        var scanners = new List<ScannerDevice>();

        using var deviceManager = new WiaDeviceManager();

        foreach (var deviceInfo in deviceManager.GetDeviceInfos())
        {
            using (deviceInfo)
            {

                scanners.Add(new ScannerDevice
                {
                    Id = deviceInfo.Id(),
                    Name = deviceInfo.Name()
                });
            }
        }

        return Task.FromResult<IReadOnlyList<ScannerDevice>>(scanners);
    }


    // Scans one page from the selected scanner.
    // simple for now
    public Task<ScannedDocument> ScanAsync(
        ScannerDevice scanner,
        ScanSettings settings)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        ArgumentNullException.ThrowIfNull(settings);

        using var deviceManager = new WiaDeviceManager();

        // Find the physical WIA device selected by the user.
        using var device = deviceManager.FindDevice(scanner.Id);

        using var flatbed = device.FindSubItem("Flatbed");

        // replaced custom code with wia code (and should throws exceptions by itself)
        // no need to enumerate all sub-items (done by WIA)
        if (flatbed is null)
        {
            throw new InvalidOperationException(
                "The scanner does not provide a Flatbed item.");
        }

        using var transfer = flatbed.StartTransfer();

        if (transfer is null)
        {
            throw new InvalidOperationException(
                "The scanner could not start the transfer.");
        }

        MemoryStream? scannedData = null;

// if scanning multiple pages, it will silently replace them...
        try
        {
            transfer.PageScanned += (_, e) =>
            {

                // The stream belongs to the scan event.
                // We copy its contents into our own MemoryStream
                // because ScannedDocument must own the scanned data.
                using var input = e.Stream;

                input.Position = 0;

                // new local var, data, This stream will become the ScannedDocument's owned data.
                var data = new MemoryStream();

                input.CopyTo(data);

                data.Position = 0;

                scannedData = data;
            };

            var success = transfer.Download();

            if (!success)
            {
                throw new InvalidOperationException(
                    "Scanner transfer was cancelled.");
            }
            // for future, t to prevent scanning of multiple pages ( only one page scanning allowed now )
            /*          if (pagesNum > 1)
            {
                throw new InvalidOperationException(
                    "This shouldnt have happen The scanner returned multiple pages, but only one page is supported.");
            }
*/
            if (scannedData is null)
            {
                throw new InvalidOperationException(
                    "Scanner did not return any image data.");
            }

            scannedData.Position = 0;

            // ScannedDocument now owns the MemoryStream.
            // No file is created; scanned image data remains in RAM.
            var document = new ScannedDocument(scannedData);

            // Ownership was transferred to ScannedDocument.
            // The finally block must not dispose this stream.
            scannedData = null;

            return Task.FromResult(document);
        }
        finally
        {
            // Dispose the stream if ownership was not transferred.
            scannedData?.Dispose();
        }
    }


}