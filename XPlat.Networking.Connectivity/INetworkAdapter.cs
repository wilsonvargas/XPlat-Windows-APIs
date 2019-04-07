namespace XPlat.Networking.Connectivity
{
    /// <summary>Represents a network adapter.</summary>
    public interface INetworkAdapter
    {
        /// <summary>Gets a value indicating the network interface type as defined by the Internet Assigned Names Authority (IANA) for the NetworkAdapter.</summary>
        uint IanaInterfaceType { get; }
    }
}