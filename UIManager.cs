using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    public TMP_InputField widthInput, heightInput, roadLengthInput, prefabLengthInput, stepsInput, seedInput, secondBranchInput, thirdBranchInput, densityInput, scaleInput, smallPropsInput, mediumPropsInput, hugePropsInput, propsLimitInput;
    public GameObject backgroundImage;

    public static UIManager Instance;
    private void Awake()
    {
        widthInput = GameObject.Find("Width Input").GetComponent<TMP_InputField>();
        heightInput = GameObject.Find("Height Input").GetComponent<TMP_InputField>();
        roadLengthInput = GameObject.Find("Road Length Input").GetComponent<TMP_InputField>();
        prefabLengthInput = GameObject.Find("Road Prefab Length Input").GetComponent<TMP_InputField>();
        stepsInput = GameObject.Find("Steps Input").GetComponent<TMP_InputField>();
        seedInput = GameObject.Find("Seed Input").GetComponent<TMP_InputField>();
        secondBranchInput = GameObject.Find("Second Branch Input").GetComponent<TMP_InputField>();
        thirdBranchInput = GameObject.Find("Third Branch Input").GetComponent<TMP_InputField>();
        densityInput = GameObject.Find("Density Input").GetComponent<TMP_InputField>();
        scaleInput = GameObject.Find("Scale Input").GetComponent<TMP_InputField>();
        smallPropsInput = GameObject.Find("Small Props Input").GetComponent<TMP_InputField>();
        mediumPropsInput = GameObject.Find("Medium Props Input").GetComponent<TMP_InputField>();
        hugePropsInput = GameObject.Find("Huge Props Input").GetComponent<TMP_InputField>();
        propsLimitInput = GameObject.Find("Props Limit Input").GetComponent<TMP_InputField>();
        backgroundImage = GameObject.Find("Background Image");
        Instance = this;
    }

    private void Start() 
    {
        widthInput.text = RandomWalk.Instance.width.ToString();
        heightInput.text = RandomWalk.Instance.height.ToString();
        roadLengthInput.text = RandomWalk.Instance.roadLength.ToString();
        prefabLengthInput.text = RandomWalk.Instance.roadPrefabLength.ToString();
        stepsInput.text = RandomWalk.Instance.steps.ToString();
        seedInput.text = RandomWalk.Instance.seed.ToString();
        secondBranchInput.text = RandomWalk.Instance.secondBranchPossibility.ToString();
        thirdBranchInput.text = RandomWalk.Instance.thirdBranchPossibility.ToString();
        densityInput.text = RandomWalk.Instance.densityOnCenter.ToString();
        scaleInput.text = RandomWalk.Instance.noiseScale.ToString();
        smallPropsInput.text = RandomWalk.Instance.smallPropsPossibility.ToString();
        mediumPropsInput.text = RandomWalk.Instance.mediumPropsPossibility.ToString();
        hugePropsInput.text = RandomWalk.Instance.hugePropsPossibility.ToString();
        propsLimitInput.text = RandomWalk.Instance.propsLimit.ToString();
    }

    private void Update()
    {
        RandomWalk.Instance.width = int.Parse(widthInput.text);
        RandomWalk.Instance.height = int.Parse(heightInput.text);
        RandomWalk.Instance.roadLength = int.Parse(roadLengthInput.text);
        RandomWalk.Instance.roadPrefabLength = int.Parse(prefabLengthInput.text);
        RandomWalk.Instance.steps = int.Parse(stepsInput.text);
        RandomWalk.Instance.seed = int.Parse(seedInput.text);
        RandomWalk.Instance.secondBranchPossibility = float.Parse(secondBranchInput.text);
        RandomWalk.Instance.thirdBranchPossibility = float.Parse(thirdBranchInput.text);
        RandomWalk.Instance.densityOnCenter = float.Parse(densityInput.text);
        RandomWalk.Instance.noiseScale = float.Parse(scaleInput.text);
        RandomWalk.Instance.smallPropsPossibility = float.Parse(smallPropsInput.text);
        RandomWalk.Instance.mediumPropsPossibility = float.Parse(mediumPropsInput.text);
        RandomWalk.Instance.hugePropsPossibility = float.Parse(hugePropsInput.text);
        RandomWalk.Instance.propsLimit = int.Parse(propsLimitInput.text);

        // If I key is pressed, toggle UI background image
        if (Input.GetKeyDown(KeyCode.I)) backgroundImage.SetActive(!backgroundImage.activeSelf);
    }

    public void OnGenerateButtonClicked()
    {
        PerlinNoiseRenderer.Instance.GenerateNoiseTexture();
        RandomWalk.Instance.GenerateRoads();
    }

}