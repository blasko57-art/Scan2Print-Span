namespace QuickCopier.Core.Models;

public sealed class ScannerCapabilities
{
    public IReadOnlyList<int> DpiValues { get; init; } =
        Array.Empty<int>();

    public int BrightnessMin { get; init; }

    public int BrightnessMax { get; init; }

    public int BrightnessStep { get; init; }

    public int ContrastMin { get; init; }

    public int ContrastMax { get; init; }

    public int ContrastStep { get; init; }

    public IReadOnlyList<int> RotationValues { get; init; } =
        Array.Empty<int>();

    public IReadOnlyList<int> DataTypeValues { get; init; } =
        Array.Empty<int>();

    public bool HasFlatbed { get; init; }

    public bool HasFeeder { get; init; }
}
