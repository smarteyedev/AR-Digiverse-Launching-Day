using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRInnocent.RestAPI;

namespace Smarteye.AR.WebRequest
{
    public class HandlerPlayerCounter : RestAPIHandler
    {
        private void Start()
        {

        }

        void Update()
        {

        }

        public void SendPlayerData()
        {
            Dictionary<string, int> newPlayer = new Dictionary<string, int>
            {
                {"increment", 1}
            };

            RowDataCustom dataObject = new RowDataCustom(newPlayer);
            restAPI.PostActionCustom(dataObject.baseData, OnSuccessResult, OnProtocolErr, DataProcessingErr, "postdata");
        }

        public override void DataProcessingErr(JObject result)
        {
            Debug.Log($"processing err: {result}");
        }

        public override void OnProtocolErr(JObject result)
        {
            Debug.Log($"protocol err: {result}");
        }

        public override void OnSuccessResult(JObject result)
        {
            Debug.Log($"success: {result}");
        }
    }
}
