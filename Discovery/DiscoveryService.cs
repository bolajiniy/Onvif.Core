using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Onvif.Core.Discovery.Common;
using Onvif.Core.Discovery.Interfaces;
using Onvif.Core.Discovery.Models;

namespace Onvif.Core.Discovery
{
	public class DiscoveryService : IDiscoveryService
	{
		IWSDiscovery wsDiscovery;
		CancellationTokenSource cancellation;
		bool isRunning;

		public DiscoveryService()
		{
			DiscoveredDevices = new ObservableCollection<DiscoveryDevice>();
			wsDiscovery = new WSDiscovery();
		}

		public ObservableCollection<DiscoveryDevice> DiscoveredDevices { get; }

		public async Task Start()
		{
			if (isRunning)
			{
				throw new InvalidOperationException("The discovery is already running");
			}
			Console.WriteLine("Discovering.......");
			isRunning = true;
			cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WS_TIMEOUT * 2));
			try
			{
				var devicesDiscovered = await wsDiscovery.Discover(Constants.WS_TIMEOUT, cancellation.Token);
				//await Task.Delay(TimeSpan.FromSeconds(Constants.WS_TIMEOUT));

				Console.WriteLine("Discovered.......{0} devices", devicesDiscovered.Count());
				SyncDiscoveryDevices(devicesDiscovered);
			}
			finally
			{
				isRunning = false;
			}
		}

		public void Stop()
		{
			isRunning = false;
			cancellation?.Cancel();
		}

		void SyncDiscoveryDevices(IEnumerable<DiscoveryDevice> syncDevices)
		{
			var lostDevices = DiscoveredDevices.Where(d => !syncDevices.Any(s => s.Equals(d))).ToList();// DiscoveredDevices.Except(syncDevices);
			var newDevices = syncDevices.Where(d => !DiscoveredDevices.Any(s => s.Equals(d))).ToList(); //syncDevices.Except(DiscoveredDevices);
			var existingDevices = DiscoveredDevices.Where(d => syncDevices.Any(s => s.Equals(d))).ToList();

            if (lostDevices != null)
			{
				foreach (var lostDevice in lostDevices.ToList())
				{
					DiscoveredDevices.Remove(lostDevice);
				}
			}

			if (newDevices != null)
			{
				foreach (var newDevice in newDevices.ToList())
				{
					if (!DiscoveredDevices.Any(d => d.MessageID == newDevice.MessageID))
					{
						DiscoveredDevices.Add(newDevice);
					}
				}
			}

			foreach (var existingDevice in existingDevices.ToList())
			{
				var syncDevice = syncDevices.FirstOrDefault(a=>a.Equals(existingDevice));
				if(syncDevice != null)
				{
					existingDevice.Address = syncDevice.Address;
				}
			}
		}
	}
}
