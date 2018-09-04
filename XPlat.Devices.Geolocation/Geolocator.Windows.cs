﻿// <copyright file="Geolocator.Windows.cs" company="James Croft">
// Copyright (c) James Croft. All rights reserved.
// </copyright>

#if WINDOWS_UWP
namespace XPlat.Devices.Geolocation
{
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using XPlat.Devices.Geolocation.Extensions;
    using XPlat.Threading.Tasks;

    public class Geolocator : IGeolocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Geolocator"/> class.
        /// </summary>
        public Geolocator()
        {
            this.Locator.StatusChanged += this.Locator_OnStatusChanged;
        }

        /// <summary>Raised when the location is updated.</summary>
        public event XPlat.Foundation.TypedEventHandler<IGeolocator, PositionChangedEventArgs> PositionChanged;

        /// <summary>Raised when the ability of the Geolocator to provide updated location changes.</summary>
        public event XPlat.Foundation.TypedEventHandler<IGeolocator, StatusChangedEventArgs> StatusChanged;

        /// <summary>Gets the originating Windows Geolocator instance.</summary>
        public Windows.Devices.Geolocation.Geolocator Locator { get; } = new Windows.Devices.Geolocation.Geolocator();

        /// <summary>Gets the last known position recorded by the Geolocator.</summary>
        public Geoposition LastKnownPosition { get; private set; }

        /// <summary>Gets or sets the requested minimum time interval between location updates, in milliseconds. If your application requires updates infrequently, set this value so that location services can conserve power by calculating location only when needed.</summary>
        public uint ReportInterval
        {
            get => this.Locator.ReportInterval;
            set => this.Locator.ReportInterval = value;
        }

        /// <summary>Gets or sets the distance of movement, in meters, relative to the coordinate from the last PositionChanged event, that is required for the Geolocator to raise a PositionChanged event.</summary>
        public double MovementThreshold
        {
            get => this.Locator.MovementThreshold;
            set => this.Locator.MovementThreshold = value;
        }

        /// <summary>Gets the status that indicates the ability of the Geolocator to provide location updates.</summary>
        public PositionStatus LocationStatus { get; private set; }

        /// <summary>Gets or sets the accuracy level at which the Geolocator provides location updates.</summary>
        public PositionAccuracy DesiredAccuracy
        {
            get => this.Locator.DesiredAccuracy == Windows.Devices.Geolocation.PositionAccuracy.Default
                           ? PositionAccuracy.Default
                           : PositionAccuracy.High;
            set => this.Locator.DesiredAccuracy = value == PositionAccuracy.Default
                                                   ? Windows.Devices.Geolocation.PositionAccuracy.Default
                                                   : Windows.Devices.Geolocation.PositionAccuracy.High;
        }

        /// <summary>Gets or sets the desired accuracy in meters for data returned from the location service.</summary>
        public uint DesiredAccuracyInMeters
        {
            get => this.Locator.DesiredAccuracyInMeters ?? 0;
            set => this.Locator.DesiredAccuracyInMeters = value;
        }

        /// <summary>Starts an asynchronous operation to retrieve the current location of the device.</summary>
        /// <returns>An asynchronous operation that, upon completion, returns a Geoposition marking the found location.</returns>
        public async Task<Geoposition> GetGeopositionAsync()
        {
            Windows.Devices.Geolocation.Geoposition pos = await this.Locator.GetGeopositionAsync();
            return new Geoposition(pos);
        }

        /// <summary>Starts an asynchronous operation to retrieve the current location of the device.</summary>
        /// <returns>An asynchronous operation that, upon completion, returns a Geoposition marking the found location.</returns>
        public Task<Geoposition> GetGeopositionAsync(TimeSpan maximumAge, TimeSpan timeout)
        {
            // Setting the timeout on the Windows API to a year as an exception is thrown when it times out.
            IAsyncOperation<Windows.Devices.Geolocation.Geoposition> getPositionTask = this.Locator.GetGeopositionAsync(maximumAge, TimeSpan.FromDays(365));

            // Creating a specific timeout task to handle this in a nicer way.
            TimeoutTask timeoutTask = new TimeoutTask(timeout, getPositionTask.Cancel);

            TaskCompletionSource<Geoposition> tcs = new TaskCompletionSource<Geoposition>();

            getPositionTask.Completed = (op, s) =>
            {
                timeoutTask.Cancel();

                switch (s)
                {
                    case AsyncStatus.Canceled:
                        tcs.SetCanceled();
                        break;
                    case AsyncStatus.Completed:
                        this.LastKnownPosition = new Geoposition(op.GetResults());
                        tcs.SetResult(this.LastKnownPosition);
                        break;
                    case AsyncStatus.Error:
                        tcs.SetException(op.ErrorCode);
                        break;
                }
            };

            return tcs.Task;
        }

        /// <summary>Requests permission to access location data.</summary>
        /// <returns>A GeolocationAccessStatus that indicates if permission to location data has been granted.</returns>
        public async Task<GeolocationAccessStatus> RequestAccessAsync()
        {
            Windows.Devices.Geolocation.GeolocationAccessStatus accessStatus;

            if (this.Locator != null)
            {
                this.Locator.PositionChanged -= this.Locator_OnPositionChanged;
            }

            try
            {
                accessStatus = await Windows.Devices.Geolocation.Geolocator.RequestAccessAsync();
            }
            catch (Exception)
            {
                accessStatus = Windows.Devices.Geolocation.GeolocationAccessStatus.Denied;
            }

            if (accessStatus == Windows.Devices.Geolocation.GeolocationAccessStatus.Allowed && this.Locator != null)
            {
                this.Locator.PositionChanged += this.Locator_OnPositionChanged;
            }

            return accessStatus.ToInternalGeolocationAccessStatus();
        }

        private void Locator_OnPositionChanged(
            Windows.Devices.Geolocation.Geolocator sender,
            Windows.Devices.Geolocation.PositionChangedEventArgs args)
        {
            this.PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.Position));
        }

        private void Locator_OnStatusChanged(
            Windows.Devices.Geolocation.Geolocator sender,
            Windows.Devices.Geolocation.StatusChangedEventArgs args)
        {
            this.LocationStatus = args.Status.ToInternalPositionStatus();
            this.StatusChanged?.Invoke(this, new StatusChangedEventArgs(this.LocationStatus));
        }
    }
}
#endif