using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace VRInnocent.RestAPI
{
    public class Authentication : RestAPIHandler
    {
        public bool isForDebuging;
        private bool isLoginSection = true;

        [Header("Sign In Components")]
        public GameObject panelSignIn;
        public TMP_InputField signInUsername;
        public TMP_InputField signInPassword;

        [Header("Sign Up Components")]
        public GameObject panelSignUp;
        public TMP_InputField signUpEmail;
        public TMP_InputField signUpName;
        public TMP_InputField signUpPassword;
        public TMP_InputField signUpPasswordComfirmation;
        public TextMeshProUGUI feedbackText;

        [Header("UI Components")]
        public GameObject[] btnLoginActions;
        /* public GameObject successPopUp;
        public GameObject errPopUp; */
        public GameObject CanvasLogin;

        [Space(10)]
        [Header("Authentication Event")]
        [Space(10)]
        public UnityEvent OnLoginComplete;
        public UnityEvent OnLoginFailed;
        [Space(10)]
        public UnityEvent OnSignUpSuccess;
        public UnityEvent OnSignUpFailed;

        private void Start()
        {
            if (isForDebuging)
            {
                signInUsername.text = "taufiq01@gmail.com";
                signInPassword.text = "taufiq123";

                // Invoke("OnClickLogin", 1f);
            }

            //OnClickOpenPanelLogin(true);
        }

        public void OpenLoginPanel()
        {
            /* Debug.LogWarning($"data player is {PlayerManager.Instance.isDataPlayerNull()}");
            if (PlayerManager.Instance.isDataPlayerNull())
            {
                CanvasLogin.SetActive(true);
            }
            else
            {
                OnLoginComplete?.Invoke();
            } */
        }

        public void OnClickRegister()
        {
            if (string.IsNullOrEmpty(signUpPassword.text) || string.IsNullOrEmpty(signUpPasswordComfirmation.text)
            || string.IsNullOrEmpty(signUpName.text) || string.IsNullOrEmpty(signUpEmail.text))
            {
                Debug.LogWarning("please fill the blank");
                feedbackText.text = $"silahkan isi kolom yang kosong terlebih dahulu";
                return;
            }

            if (signUpPassword.text != signUpPasswordComfirmation.text)
            {
                Debug.LogWarning("comfirm your password");
                feedbackText.text = $"password yang anda masukkan tidak sesuai";
                return;
            }

            foreach (var item in btnLoginActions)
            {
                item.SetActive(false);
            }

            Dictionary<string, string> newPlayer = new Dictionary<string, string>
            {
                {"email", signUpEmail.text},
                {"name", signUpName.text},
                {"password", signUpPassword.text}
            };

            RowData dataObject = new RowData(newPlayer);
            restAPI.PostAction(dataObject.baseData, OnSuccessResult, OnProtocolErr, DataProcessingErr, "register");
        }

        public void OnClickLogin()
        {
            if (string.IsNullOrEmpty(signInUsername.text) || string.IsNullOrEmpty(signInPassword.text))
            {
                Debug.LogWarning("please fill the blank");
                return;
            }

            foreach (var item in btnLoginActions)
            {
                item.SetActive(false);
            }

            Dictionary<string, string> newPlayer = new Dictionary<string, string>
            {
                {"email", signInUsername.text},
                {"password", signInPassword.text}
            };

            RowData dataObject = new RowData(newPlayer);
            var test = JsonConvert.SerializeObject(dataObject);
            Debug.Log("Login to account:" + test);

            restAPI.PostAction(dataObject.baseData, OnSuccessResult, OnProtocolErr, DataProcessingErr, "login");
        }

        private void Update()
        {
            // if (!isLoginSection)
            // {
            //     if (!string.IsNullOrEmpty(signUpPassword.text))
            //     {
            //         signUpPasswordComfirmation.interactable = true;
            //     }
            //     else
            //     {
            //         signUpPasswordComfirmation.interactable = false;
            //         signUpPasswordComfirmation.text = "";
            //     }
            // }
            // else
            // {
            //     successPopUp.SetActive(false);
            //     errPopUp.SetActive(false);
            // }
        }

        /* public void OnClickGenderSelection(string str)
        {
            signUpGender = str;
            Debug.Log($"player is {str}");
        } */

        public void OnClickOpenPanelLogin(bool state)
        {
            isLoginSection = state;

            if (isLoginSection)
            {
                panelSignIn.SetActive(true);
                panelSignUp.SetActive(false);

                signInUsername.text = "";
                signInPassword.text = "";
            }
            else
            {
                panelSignIn.SetActive(false);
                panelSignUp.SetActive(true);

                if (isForDebuging)
                {
                    signUpEmail.text = "taufiq07@gmail.com";
                    signUpName.text = "taufiq 07";
                    signUpPassword.text = "taufiq123";
                    signUpPasswordComfirmation.text = "taufiq123";
                }
            }

            foreach (var item in btnLoginActions)
            {
                item.SetActive(true);
            }
        }

        public override void OnSuccessResult(JObject result)
        {
            if (isLoginSection)
            {
                RootLogin.Root returnData = result.ToObject<RootLogin.Root>();
                // RootLogin.loginResponses.Add(returnData.response);  // Add to static list
                Debug.Log($"player : {returnData.response.userName} has logged-in");

                /* PlayerManager.Instance.userEmail = returnData.response.userEmail;
                PlayerManager.Instance.dataPlayer = returnData.response; */

                OnLoginComplete?.Invoke();
            }
            else
            {
                // successPopUp.SetActive(true);
                // errPopUp.SetActive(false);
                RootRegister returnData = result.ToObject<RootRegister>();
                Debug.Log($"New player name: {returnData.name}.");

                //! will update if system required the access token
                RootLogin.Response tempResponse = new RootLogin.Response();
                tempResponse.userId = returnData.id;
                tempResponse.userName = returnData.name;
                tempResponse.userEmail = returnData.email;
                tempResponse.access_token = "";

                /* PlayerManager.Instance.userEmail = returnData.email;
                PlayerManager.Instance.dataPlayer = tempResponse; */

                OnSignUpSuccess?.Invoke();
            }
        }

        public override void OnProtocolErr(JObject result)
        {
            if (isLoginSection)
            {
                OnLoginFailed?.Invoke();

                foreach (var item in btnLoginActions)
                {
                    item.SetActive(true);
                }
            }
            else
            {
                OnSignUpFailed?.Invoke();
                feedbackText.text = $"anda tidak dapat membuat akun atau silahkan gunakan email lain";
            }
        }

        public override void DataProcessingErr(JObject result)
        {
            if (isLoginSection)
            {
                /* RootLogin returnData = result.ToObject<RootLogin>();
                Debug.LogError($"{returnData.message}"); */

                OnLoginFailed?.Invoke();
            }
            else
            {
                /* RootRegister returnData = result.ToObject<RootRegister>();
                Debug.LogError($"Message: {returnData.message}."); */

                feedbackText.text = $"anda tidak dapat membuat akun atau silahkan gunakan email lain";
                OnSignUpFailed?.Invoke();
            }
        }

        public class RootRegister
        {
            public string id { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string passwordHash { get; set; }
            public string role { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

        [System.Serializable]
        public class RootLogin
        {
            [System.Serializable]
            public class Response
            {
                public string userId;
                public string userName;
                public string userEmail;
                public string access_token;
            }

            [System.Serializable]
            public class Root
            {
                public Response response;
            }

            // Static list to hold responses
            // public static List<Response> loginResponses = new List<Response>();
        }
    }
}