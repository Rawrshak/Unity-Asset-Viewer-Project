using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json;

namespace Rawrshak
{
    [Serializable]
    public class BurnTransactionData
    {
        string account;
        List<BigInteger> tokenIds;
        List<BigInteger> amounts;
        
        public string GenerateArgsForCreateContractData() {
            object[] burnData = {
                account,
                tokenIds,
                amounts
            };
            object[][] results = { burnData };
            return JsonConvert.SerializeObject(results);
        }
    }
}