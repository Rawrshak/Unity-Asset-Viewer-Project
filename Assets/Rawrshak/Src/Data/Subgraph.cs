using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Rawrshak
{
    [CreateAssetMenu(fileName="Subgraph", menuName="Rawrshak/Create Subgraph Object")]
    public class Subgraph : SingletonScriptableObject<Subgraph>
    {
        // These are the deployed subgraph uri
        public string contentsSubgraphUri = "https://api.thegraph.com/subgraphs/name/gcbsumid/contents-optimistic-kovan";
        public string exchangeSubgraphUri = "https://api.thegraph.com/subgraphs/name/gcbsumid/exchange-optimistic-kovan";
    }
}