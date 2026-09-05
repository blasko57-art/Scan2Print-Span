namespace QuickCopier.Core.Models;

public class ScannerDevice
{
    // Id will eventually be the identifier Windows gives us.
    //Name is what we'll show the user.

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    // set; or innit; 
    // probably set;
    // wantedto use init so that scanner identyty shouldnt normally change after we created object
    // but apparently that might break on some scanners i think
    public override string ToString()
    {
        // so that ComboBox  will return just the name of scanner
        // not something ike QuickCopier.Core.Models.ScannerDevice

        return Name;
    }
}
