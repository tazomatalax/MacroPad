namespace RSoft.MacroPad.BLL;

/// <summary>
/// Tracks which macro pad products have been tested with this software.
/// </summary>
public static class TestedProducts
{
    /// <summary>
    /// Collection of tested vendor/product ID pairs.
    /// </summary>
    private static readonly HashSet<(ushort VendorId, ushort ProductId)> TestedDevices =
    [
        (4489, 34960)
    ];

    /// <summary>
    /// Checks if a product has been tested with this software.
    /// </summary>
    /// <param name="vendorId">The USB vendor ID.</param>
    /// <param name="productId">The USB product ID.</param>
    /// <returns>True if the product has been tested; otherwise, false.</returns>
    public static bool IsTested(ushort vendorId, ushort productId) =>
        TestedDevices.Contains((vendorId, productId));
}
