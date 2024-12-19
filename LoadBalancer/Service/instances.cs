using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadBalancer.Model;

namespace LoadBalancer.Service
{
    public class Instances
    {
            private List<MicroServiceInstance> _orderServiceInstances = new List<MicroServiceInstance>();
            private List<MicroServiceInstance> _inventoryServiceInstances = new List<MicroServiceInstance>();

            private Dictionary<string, int> _roundRobinIndex = new Dictionary<string, int>();

            public List<MicroServiceInstance> OrderServiceInstances
            {
                get { return _orderServiceInstances; }
                set { _orderServiceInstances = value; }
            }
            
            public List<MicroServiceInstance> InventoryServiceInstances
            {
                get { return _inventoryServiceInstances; }
                set { _inventoryServiceInstances = value; }
            }

            public Dictionary<string, int> RoundRobinIndex
            {
                get { return _roundRobinIndex; }
                set { _roundRobinIndex = value; }
            }

    }
}