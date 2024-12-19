using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace InventoryService
{
    public class ReplicaIpFinder
    {
        public string GetOwnIpAddress()
        {
            string? newOwnIp = null;

            // Get all network interfaces and find the one that is most likely in use
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4 addresses only
                {
                    var rawOwnIp = ip.ToString();
                    newOwnIp = rawOwnIp.Trim('\"');
                    break; // Return the first valid IPv4 address
                }
            }

            if (newOwnIp == null)
            {
                throw new InvalidOperationException("No suitable network interface found for the replica.");
            }

            return newOwnIp;
        }
    }
}