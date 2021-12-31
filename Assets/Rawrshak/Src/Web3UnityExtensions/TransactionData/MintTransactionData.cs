using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json;

namespace Rawrshak
{
    [Serializable]
    public class MintTransactionData
    {
        // Mint Public Data
        public string to = String.Empty;
        public List<BigInteger> tokenIds = null;
        public List<BigInteger> amounts = null;
        public BigInteger nonce = 0;
        public string signer = String.Empty;
        public string signature = "0x";

        public string GenerateArgsForCreateContractData() {
            object[] mintData = {
                to,
                tokenIds,
                amounts,
                nonce, 
                signer,
                signature
            };
            object[][] results = { mintData };
            return JsonConvert.SerializeObject(results);
        }

        /******************************************************************************/
        /* The following code is for generating our own TransactionData, if necessary */
        /******************************************************************************/

        // // keccak256("mintBatch((address,uint256[],uint256[],uint256,address,bytes))") = 0x84547d25
        // private static string FUNCTION_SELECTOR = "0x84547d25";

        // public string GenerateTransactionData() {
        //     BigInteger bigIntTo = BigInteger.Parse("0" + to, NumberStyles.AllowHexSpecifier);
        //     BigInteger bigIntSigner = BigInteger.Parse("0" + signer, NumberStyles.AllowHexSpecifier);
        //     // Debug.Log(String.Format("Address: {0}", bigIntTo.ToString("X64")));

        //     BigInteger data1Offset = 32;
        //     BigInteger tokenIdsOffset = 32 * 6;
        //     BigInteger amountsOffset = tokenIdsOffset + 32*(tokenIds.Count + 1);
        //     BigInteger signatureOffset = amountsOffset + 32*(amounts.Count + 1);
        //     BigInteger signatureLength = 65;

        //     string tokenIdsResults = ConvertListOfBigIntToTransactionString(tokenIds);
        //     string amountsResults = ConvertListOfBigIntToTransactionString(amounts);
        //     string nonceResults = nonce.ToString("X64");

        //     // Todo: signature needs to flush signature with 31 bytes of 0 at the end
        //     string[] dataArray = { 
        //         FUNCTION_SELECTOR,
        //         data1Offset.ToString("X64"),
        //         bigIntTo.ToString("X64"),
        //         tokenIdsOffset.ToString("X64"),
        //         amountsOffset.ToString("X64"),
        //         nonceResults,
        //         bigIntSigner.ToString("X64"),
        //         signatureOffset.ToString("X64"),
        //         tokenIdsResults,
        //         amountsResults,
        //         signatureLength.ToString("X64"),
        //         signature,
        //     };

        //     // Data.HexToByteArray()
        //     return string.Concat(dataArray);
        // }

        // private string ConvertListOfBigIntToTransactionString(List<BigInteger> array) {
        //     BigInteger len = array.Count;
        //     Debug.Log("Length: " + len.ToString("X64"));

        //     string arrayString = len.ToString("X64");

        //     List<string> arrayStr = new List<string>();
        //     for (int i = 0; i < array.Count; i++) {
        //         Debug.Log(String.Format("[{0}]: {1}", i, array[i].ToString("X64")));
        //         arrayStr.Add(array[i].ToString("X64"));
        //     }

        //     string arrayConcat = String.Concat(arrayStr);
        //     arrayString = String.Concat(arrayString, arrayConcat);

        //     return arrayString;
        // }
    }
}