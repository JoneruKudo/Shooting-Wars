using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginSystem : MonoBehaviour
{
    public GameObject accountLoginScreen;

    public GameObject userLoginScreen;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public GameObject registrationScreen;
    public TMP_InputField newUsernameInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField reenterPasswordInput;

    public GameObject loadingScreen;
    public TMP_Text loadingText;
}
