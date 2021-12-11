using UnityEngine;
using System;

namespace Rawrshak
{
    [Serializable]
    public class WalletResponse
    {
        // public float balance;
        public string balance;
        public float fiatBalance;
        public string fiat;
        public string address;

        public static WalletResponse Parse(string jsonString)
        {
            return JsonUtility.FromJson<WalletResponse>(jsonString);
        }
    }
}