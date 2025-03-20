using UnityEngine;
using TMPro;
using Smarteye.AR;

namespace Smarteye.Keyboard
{
    public class InputFieldController : MonoBehaviour
    {
        public static InputFieldController Instance;
        // [SerializeField] TextMeshProUGUI textBox;
        // [SerializeField] TextMeshProUGUI printBox;

        [SerializeField] private GameManager gameManager;

        [SerializeField] private GameObject virtualKeyboard;
        [SerializeField] private TMP_InputField playerInputField;

        private void Start()
        {
            Instance = this;
            // printBox.text = "";
            // textBox.text = "";
        }

        public void OpenKeyboard()
        {
            if (Application.isMobilePlatform)
            {
                virtualKeyboard.SetActive(true);
                Debug.Log("Running on a mobile device");
            }
            else
            {
                virtualKeyboard.SetActive(false);
                Debug.Log("Running on a non-mobile platform");
            }
        }

        public void DeleteLetter()
        {
            // if (textBox.text.Length != 0)
            // {
            //     textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
            // }

            if (playerInputField.text.Length != 0)
            {
                playerInputField.text = playerInputField.text.Remove(playerInputField.text.Length - 1, 1);
            }
        }

        public void AddLetter(string letter)
        {
            // textBox.text = textBox.text + letter;

            playerInputField.text = playerInputField.text + letter;
        }

        public void SubmitWord()
        {
            gameManager.SetPlayerName();

            // printBox.text = textBox.text;
            // textBox.text = "";
            // Debug.Log("Text submitted successfully!");
        }
    }
}