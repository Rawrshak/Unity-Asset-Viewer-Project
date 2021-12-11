using UnityEngine;
using System;

namespace Rawrshak
{
[Serializable]
    public class UploadResponse
    {
        public int id;
        public string name;
        public int folderId;
        public string transactionId;
        public int version;
        public Status status;

        public static UploadResponse Parse(string jsonString)
        {
            return JsonUtility.FromJson<UploadResponse>(jsonString);
        }
    }

    [Serializable]
    public class Status {
        public int status;
        public Confirmed confirmed;
    }

    [Serializable]
    public class Confirmed {
        public int block_height;
        public string block_indep_hash;
        public int number_of_confirmations;
    }
}