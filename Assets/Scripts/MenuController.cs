using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private InputField inputFieldNumFloor;

    [SerializeField]
    private InputField inputFieldNumElevator;

    const int kDefaultNumFloor = 2;
    const int kDefaultNumElevator = 1;

    const string kGameScene = "GameScene";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBtnStart()
    {
        bool isSuccessfulParse = uint.TryParse(inputFieldNumFloor.text, out uint numFloor);
        isSuccessfulParse = uint.TryParse(inputFieldNumElevator.text, out uint numElevator) && isSuccessfulParse;

        if (isSuccessfulParse == false)
        {
            // Input Error, use default set up
            numFloor = kDefaultNumFloor;
            numElevator = kDefaultNumElevator;
            Debug.LogWarning("Input Error, use default set up");
        }
        else
        {
            // validate input
            numFloor = numFloor > kDefaultNumFloor ? numFloor : kDefaultNumFloor;
            numElevator = numElevator > kDefaultNumElevator ? numElevator : kDefaultNumElevator;
        }

        GameConfig.SetInputData(numFloor, numElevator);
        Debug.LogFormat("Start game with config: {0} floors and {1} elevator", numFloor, numElevator);
        SceneManager.LoadScene(kGameScene);
    }
}
