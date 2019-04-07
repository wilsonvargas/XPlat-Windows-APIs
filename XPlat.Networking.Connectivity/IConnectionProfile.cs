namespace XPlat.Networking.Connectivity
{
    /// <summary>Represents a network connection, which includes either the currently connected network or prior network connections. Provides information about the connection status and connectivity statistics.</summary>
    public interface IConnectionProfile
    {
        /// <summary>Gets the name of the connection profile.</summary>
        string ProfileName { get; }

        /// <summary>Gets the object representing the network adapter providing connectivity for the connection.</summary>
        INetworkAdapter NetworkAdapter { get; }

        /// <summary>Gets the network connectivity level for this connection. This value indicates what network resources, if any, are currently available.</summary>
        /// <returns>The level of network connectivity.</returns>
        NetworkConnectivityLevel GetNetworkConnectivityLevel();

        /// <summary>Gets a value that indicates the current number of signal bars displayed by the Windows UI for the connection.</summary>
        /// <returns>An integer value within a range of 0-5 that corresponds to the number of signal bars displayed by the UI.</returns>
        byte? GetSignalBars();
    }
}