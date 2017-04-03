﻿namespace XPlat.Storage
{
    using System;
    using System.Threading;

    /// <summary>
    /// Defines the application data layer.
    /// </summary>
    public sealed class ApplicationData : IApplicationData
    {
        private static readonly Lazy<ApplicationData> CurrentAppData =
            new Lazy<ApplicationData>(() => new ApplicationData(), LazyThreadSafetyMode.PublicationOnly);

        private readonly Lazy<IAppSettingsContainer> settings = new Lazy<IAppSettingsContainer>(
            CreateSettings,
            LazyThreadSafetyMode.PublicationOnly);

        private readonly Lazy<IStorageFolder> localFolder = new Lazy<IStorageFolder>(
            CreateLocalFolder,
            LazyThreadSafetyMode.PublicationOnly);

        private readonly Lazy<IStorageFolder> roamingFolder = new Lazy<IStorageFolder>(
            CreateRoamingFolder,
            LazyThreadSafetyMode.PublicationOnly);

        private readonly Lazy<IStorageFolder> temporaryFolder = new Lazy<IStorageFolder>(
            CreateTemporaryFolder,
            LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current instance of the <see cref="ApplicationData"/>.
        /// </summary>
        public static ApplicationData Current => CurrentAppData.Value;

        /// <summary>
        /// Gets the root folder for the application in the local data store.
        /// </summary>
        public IStorageFolder LocalFolder => this.localFolder.Value;

        /// <summary>
        /// Gets the settings container for the application in the local data store.
        /// </summary>
        public IAppSettingsContainer LocalSettings => this.settings.Value;

        /// <summary>
        /// Gets the root folder for the application in the roaming data store.
        /// </summary>
        public IStorageFolder RoamingFolder => this.roamingFolder.Value;

        /// <summary>
        /// Gets the root folder for the application in the temporary data store.
        /// </summary>
        public IStorageFolder TemporaryFolder => this.temporaryFolder.Value;

        private static IAppSettingsContainer CreateSettings()
        {
            return new AppSettingsContainer();
        }

        private static IStorageFolder CreateLocalFolder()
        {
            return new StorageFolder(null, Windows.Storage.ApplicationData.Current.LocalFolder);
        }

        private static IStorageFolder CreateRoamingFolder()
        {
            return new StorageFolder(null, Windows.Storage.ApplicationData.Current.RoamingFolder);
        }

        private static IStorageFolder CreateTemporaryFolder()
        {
            return new StorageFolder(null, Windows.Storage.ApplicationData.Current.TemporaryFolder);
        }
    }
}