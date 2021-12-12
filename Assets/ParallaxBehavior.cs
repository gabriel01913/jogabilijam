using UnityEngine;

public class ParallaxBehavior : MonoBehaviour
{
  float startZ;
  Vector2 startPosition;

  public Transform targetObject;
  public Camera targetCamera;

  Vector2 delta => (Vector2)targetCamera.transform.position - startPosition;

  private void Start()
  {
    startZ = transform.position.z;
    startPosition = transform.position;
  }


  public void LateUpdate()
  {
    transform.position = delta + startPosition;
  }

}
