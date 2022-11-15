using UnityEngine;
using System.Collections;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxBackground : MonoBehaviour
{

    private Transform cameraTransform;

    [SerializeField] private Vector2 parallaxEffect;

    [SerializeField] private bool repeatingXAxis = true;

    private float startPositionY, currentPositionX, currentPositionY, width;
    private float limitY = -1;

    private Vector3 lastCameraPosition;

    //To be used when only bottom sprite
    [Header("For Bottom Only Background Sprites ")]
    [SerializeField] private bool bottomOnlySprite;
    [HideInInspector, SerializeField] private float maxPositionY, minPositionY;
    


    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        currentPositionX = transform.position.x;

        startPositionY = transform.position.y;
        currentPositionY = startPositionY;

        width = GetComponent<SpriteRenderer>().bounds.size.x;

        StartCoroutine(SetYLimit());
    }

    public IEnumerator SetYLimit()
    {
        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        CinemachineVirtualCamera vcam = GameObject.FindGameObjectWithTag(Config.CINEMACHINE_CAMERA_TAG).GetComponent<CinemachineVirtualCamera>();

        limitY = ((GetComponent<SpriteRenderer>().size.y * GetComponent<SpriteRenderer>().transform.localScale.y) - (vcam.m_Lens.OrthographicSize * 2f)) / 2f;
    }

    private void LateUpdate()
    {
        if (!bottomOnlySprite && limitY == -1)
            return;

        float deltaXFromCamera = (cameraTransform.position.x * (1 - parallaxEffect.x));
        float deltaXPosition = cameraTransform.position.x * parallaxEffect.x;

        transform.position = new Vector3(currentPositionX + deltaXPosition, transform.position.y, transform.position.z);

        if (repeatingXAxis)
        {
            if (deltaXFromCamera > currentPositionX + width)
                currentPositionX += width;
            else if (deltaXFromCamera < currentPositionX - width)
                currentPositionX -= width;
        }


        float deltaMovementY = cameraTransform.position.y - lastCameraPosition.y;
        float parallaxMovementY = -deltaMovementY * parallaxEffect.y;

        lastCameraPosition = cameraTransform.position;

        currentPositionY = transform.position.y;
        float newPossiblePosition = currentPositionY + parallaxMovementY - cameraTransform.position.y;

        if (bottomOnlySprite)
        {
            if (newPossiblePosition < maxPositionY && newPossiblePosition > minPositionY)
            {
                transform.position = new Vector3(transform.position.x, currentPositionY + parallaxMovementY, transform.position.z);
            }
        }
        else
        {
            if (newPossiblePosition < limitY && newPossiblePosition > -limitY)
            {
                transform.position = new Vector3(transform.position.x, currentPositionY + parallaxMovementY, transform.position.z);
            }
        }
    }

    #region Editor

#if UNITY_EDITOR

    [CustomEditor(typeof(ParallaxBackground))]
    public class ParallaxEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ParallaxBackground parallaxBackground = (ParallaxBackground)target;

            if (parallaxBackground.bottomOnlySprite)
                DrawDetails(parallaxBackground);
        }

        private static void DrawDetails(ParallaxBackground parallaxBackground)
        {
            EditorGUILayout.LabelField("Max Position Y Axis");
            parallaxBackground.maxPositionY = EditorGUILayout.FloatField(parallaxBackground.maxPositionY);

            EditorGUILayout.LabelField("Min Position Y Axis");
            parallaxBackground.minPositionY = EditorGUILayout.FloatField(parallaxBackground.minPositionY);

            EditorUtility.SetDirty(parallaxBackground);
        }
    }

#endif

    #endregion
}
