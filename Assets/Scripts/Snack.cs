using UnityEngine;

public class Snack
{
    public CubePoint Position;

    public GameObject[] Prefabs;

    private Cube Cube;

    private GameObject SnackGameObject;

    public Snack(GameObject[] prefabs, Cube cube)
    {
        Prefabs = prefabs;
        Cube = cube;
    }
    
    public void AssignNewPosition(CubePoint[] pointsToAvoid)
    {
        RemoveContent();
        
        Position = Cube.GetRandomPoint(Cube.Dimension, pointsToAvoid);
        InstantiateContent();
    }

    private void RemoveContent()
    {
        MonoBehaviour.Destroy(SnackGameObject);
    }

    private void InstantiateContent()
    {
        GameObject prefab = Prefabs[Random.Range(0, Prefabs.Length)];
        
        // Position
        Vector3 positionInCube = Position.SideCoordinate.GetPositionInCube(Cube.Dimension, Cube.Scale); 
        Quaternion rotationInCube = Position.SideCoordinate.GetRotationInCube();
            
        // position of the center of a field
        Vector3 positionInSide = Position.FieldCoordinate.GetPositionInCubeSide(Cube.Scale);
        
        // TODO get the right Offsets when adding the right prefabs
        Vector3 snackOffset = new Vector3(0, prefab.transform.localScale.y / 2, 0);
        Vector3 snackPosition = positionInCube + (rotationInCube * (positionInSide + snackOffset));
        

        // Instantiate prefab on cube
        SnackGameObject = InstantiateManager.Instance.InstantiateGameObject(snackPosition, rotationInCube, prefab);
        SnackGameObject!.transform.parent = RotationManager.Instance.transform;

        // we need to change localPosition and the localRotation of the prefab.
        // Otherwise the snack would spawn somewhere else on the cube without the right rotation.
        SnackGameObject.transform.localPosition = snackPosition;
        SnackGameObject.transform.localRotation = rotationInCube;
    }
}
