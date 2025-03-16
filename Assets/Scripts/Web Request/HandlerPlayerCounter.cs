using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VRInnocent.RestAPI;

namespace Smarteye.AR.WebRequest
{
    public class HandlerPlayerCounter : RestAPIHandler
    {
        Action onSuccessAction;

        public void SendPlayerData(Action onComplateAction)
        {
            onSuccessAction = onComplateAction;

            Dictionary<string, object> newPlayer = new Dictionary<string, object>
            {
                {"increment", 1}
            };

            RowDataObject dataObject = new RowDataObject(newPlayer);
            restAPI.PostActionCustom(dataObject.baseData, OnSuccessResult, OnProtocolErr, DataProcessingErr, "postdata");
        }

        public void SendPlayerData(string _playerName, float _playerTimer, int _playerTapCount, Action onComplateAction)
        {
            onSuccessAction = onComplateAction;

            Dictionary<string, object> newPlayer = new Dictionary<string, object>
            {
                {"increment", 1},
                {"playerName", _playerName},
                {"playerTimer", _playerTimer},
                {"playerTapCount", _playerTapCount}
            };

            RowDataObject dataObject = new RowDataObject(newPlayer);
            // restAPI.PostActionCustom(dataObject.baseData, OnSuccessResult, OnProtocolErr, DataProcessingErr, "postdata");
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
            onSuccessAction?.Invoke();

            Debug.Log($"success: {result}");
        }
    }
}
