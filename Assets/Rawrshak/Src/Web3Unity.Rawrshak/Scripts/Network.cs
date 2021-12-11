using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Rawrshak
{
    [CreateAssetMenu(fileName="Network", menuName="Rawrshak/Create Network Object")]
    public class Network : SingletonScriptableObject<Network>
    {
        public string chain = "ethereum";
        public string network = "optimistic-kovan";
        public BigInteger chainId = 69;
        public string httpEndpoint = "https://kovan.optimism.io";
    }
}