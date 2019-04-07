namespace XPlat.Networking.Connectivity
{
    /// <summary>Defines the level of connectivity currently available.</summary>
    public enum NetworkConnectivityLevel
    {
        /// <summary>No connectivity.</summary>
        None,

        /// <summary>Local network access only.</summary>
        LocalAccess,

        /// <summary>Limited internet access.</summary>
        ConstrainedInternetAccess,

        /// <summary>Local and Internet access.</summary>
        InternetAccess,
    }
}