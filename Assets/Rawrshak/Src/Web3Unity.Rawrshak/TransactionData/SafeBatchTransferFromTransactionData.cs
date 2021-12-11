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
        string from;
        string to;
        string[] ids;
        string[] amounts;
        string bytes;
        
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