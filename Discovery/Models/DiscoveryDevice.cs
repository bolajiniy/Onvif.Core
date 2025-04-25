using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Onvif.Core.Discovery.Models
{
	
    public class DiscoveryDevice : INotifyPropertyChanged
    {
        public IEnumerable<string> Types { get; internal set; }

        public IEnumerable<string> XAdresses { get; internal set; }
		public string Model { get; internal set; }
		public string Name { get; internal set; }
        public string Address { get; internal set; }

        public string Host
		{
			get => new Uri(XAdresses.FirstOrDefault()).Authority;
		}
		public string MessageID { set; get; }
        public string Raw { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals (object obj)
		{
            if (!(obj is DiscoveryDevice o))
            {
                return false;
            }
			if (MessageID == o.MessageID || MessageID.Split(':').Last() == o.MessageID.Split(':').Last())
			{
				return true;
			}

			if (o.Model != Model || o.Name != Name || o.Address.ToString() != Address.ToString()
				|| o.Types.Count() != Types.Count() || o.XAdresses.Count() != XAdresses.Count()) {
				return false;
			}
			for (var i = 0; i < Types.Count(); i++) {
				if (!Types.ElementAt(i).Equals (o.Types.ElementAt(i))) {
					return false;
				}
			}
			for (var i = 0; i < XAdresses.Count (); i++) {
				if (!XAdresses.ElementAt (i).Equals (o.XAdresses.ElementAt (i))) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode ()
		{
			var hash = 1;
			if (Types != null) {
				foreach (var type in Types) {
					hash += type.GetHashCode ();
				}
			}
			if (XAdresses != null) {
				foreach (var address in XAdresses) {
					hash += address.GetHashCode ();
				}
			}
			hash += (Model?.GetHashCode () + Name?.GetHashCode () + Address?.GetHashCode ()) ?? 0;
			return hash;
		}
	}
}
