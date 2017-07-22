using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	GameObject cameraTarget;
    public Vector3 offsetPosition;
	public float rotateSpeed;
	float rotate;
	//public float offsetDistance;
	//public float offsetHeight;
	public float smoothing;
	Vector3 offset;
	bool following = true;
	Vector3 lastPosition;

	void Start()
	{
		cameraTarget = GameObject.FindGameObjectWithTag("Player");
		lastPosition = cameraTarget.transform.position + offsetPosition;
		offset = cameraTarget.transform.position + offsetPosition;
	}

	void Update()
	{
		if(Input.GetKey(KeyCode.F))
		{
			if(following)
			{
				following = false;
			} 
			else
			{
				following = true;
			}
		}

        rotate = Input.GetAxis("Mouse X");
		//if(Input.GetKey(KeyCode.Q))
		//{
		//	rotate = -1;
		//} 
		//else if(Input.GetKey(KeyCode.E))
		//{
		//	rotate = 1;
		//} 
		//else
		//{
		//	rotate = 0;
		//}
		if(following)
		{
			offset = Quaternion.AngleAxis(rotate * rotateSpeed, Vector3.up) * offset;
			transform.position = cameraTarget.transform.position + offset;
            transform.position = new Vector3(Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offset.x, smoothing * Time.deltaTime),
                Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offset.y, smoothing * Time.deltaTime), 0);
				//Mathf.Lerp(lastPosition.z, cameraTarget.transform.position.z + offset.z, smoothing * Time.deltaTime));
		} 
		else
		{
			transform.position = lastPosition; 
		}
		transform.LookAt(cameraTarget.transform.position);
	}

	void LateUpdate()
	{
		lastPosition = transform.position;
	}
}