using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json;

namespace Rawrshak
{
    [Serializable]
    public class SafeBatchTransferFromTransactionData
    {
        string from = String.Empty;
        string to = String.Empty;
        string[] ids = null;
        string[] amounts = null;
        string bytes = String.Empty;
        
        public string GenerateArgsForCreateContractData() {
            object[] transferData = {
                from,
                to,
                ids,
                amounts,
                bytes
            };
            return JsonConvert.SerializeObject(transferData);
        }
    }
}