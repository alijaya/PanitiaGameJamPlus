using RS.Typing.Core;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] private WordObject _startWordObject;
    private WordObject[] wordObjects;
    private void Awake() {
        wordObjects = FindObjectsOfType<WordObject>();
        SetEnableWordObjects(false);
        _startWordObject.enabled = true;
    }

    public void SetEnableWordObjects(bool value) {
        foreach (var wordObject in wordObjects) {
            wordObject.enabled = value;
        }
    }
}
