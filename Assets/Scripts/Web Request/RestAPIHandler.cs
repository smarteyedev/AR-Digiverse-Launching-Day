using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

namespace VRInnocent.RestAPI
{
    public abstract class RestAPIHandler : MonoBehaviour
    {
        public RestAPI restAPI;
        public abstract void OnSuccessResult(JObject result);
        public abstract void OnProtocolErr(JObject result);
        public abstract void DataProcessingErr(JObject result);
    }

    [Serializable]
    public class RowData
    {
        public Dictionary<string, string> baseData;

        public RowData(Dictionary<string, string> _baseData)
        {
            baseData = _baseData;
        }
    }

    [Serializable]
    public class RowDataCustom
    {
        public Dictionary<string, int> baseData;

        public RowDataCustom(Dictionary<string, int> _baseData)
        {
            baseData = _baseData;
        }
    }
}