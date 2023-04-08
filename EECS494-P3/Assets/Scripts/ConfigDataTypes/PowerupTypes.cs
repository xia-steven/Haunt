using System.Collections.Generic;
using Hub;
using JSON_Parsing;

namespace ConfigDataTypes {
    public class PurchaseableTypesData : Savable {
        public List<UpgradeData> types;
    }

    [System.Serializable]
    public class UpgradeData {
        public PurchaseableType type;
        public int cost;
        public float rate1;
        public float rate2;
        public float duration;
        public string description;
        public int maxOwnable = 1;
    }
}