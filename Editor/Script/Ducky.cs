using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Audio;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum OPTIONS1
{
    SceneView = 0,
    EditorView = 1,
}
public enum OPTIONS2
{
    Dynamic = 0,
    Kinematic = 1,
    Static = 2,
}
public enum OPTIONS3
{
    Kinematic = 0,
    Static = 1,
}

public class Ducky : EditorWindow
{
    [MenuItem("Tools/Ducky")]
    public static void Duck() => GetWindow<Ducky>();

    public AnimationLoader animload;
    SerializedObject so;
    SerializedProperty propsize;
    SerializedProperty propvolume;

    //HandleOptions
    public OPTIONS1 op1;
    public OPTIONS2 op2;
    public OPTIONS3 op3;

    bool animate;

    //AnimationsHandler
    public Texture2D[] animtex;
    public Texture2D[] oldanimtex;
    public int Frame = 0;
    float time = 0;

    //AnimationStates
    public float BedTime;
    int random = 1;
    bool flip;

    public float size;
    public float volume;

    Vector3 lastpos;
    Vector3 lastrot;
    Vector3 pose;

    Rect screenPos;
    Rect oldscreenPos;
    Rect Buttonpos;

    Rect DuckyRect;

    GameObject source;
    List<string> count = new List<string>();
    GameObject instsource;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propsize = so.FindProperty("size");
        propvolume = so.FindProperty("volume");
        Frame = 0;
        animload = (AnimationLoader)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Script/AnimationData.asset", typeof(AnimationLoader));

        source = (GameObject)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/DONOTDELETorRENAMEsnde.prefab", typeof(GameObject));
        animtex = animload.Idleling1;

        size = EditorPrefs.GetFloat("DUCKY_TOOL_size", 120);
        volume = EditorPrefs.GetFloat("DUCKY_TOOL_volume", 1);
        op1 = (OPTIONS1)EditorPrefs.GetInt("DUCKY_TOOL_op1", (int)op1);
        op2 = (OPTIONS2)EditorPrefs.GetInt("DUCKY_TOOL_op2", (int)op2);
        op3 = (OPTIONS3)EditorPrefs.GetInt("DUCKY_TOOL_op3", (int)op3);
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        EditorPrefs.SetFloat("DUCKY_TOOL_size", size);
        EditorPrefs.SetFloat("DUCKY_TOOL_volume", volume);
        EditorPrefs.SetInt("DUCKY_TOOL_op1", (int)op1);
        EditorPrefs.SetInt("DUCKY_TOOL_op2", (int)op2);
        EditorPrefs.SetInt("DUCKY_TOOL_op3", (int)op3);
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI()
    {
        if (!count.Contains(SceneManager.GetActiveScene().name))
        {
            instsource = Instantiate(source);
            instsource.hideFlags = HideFlags.HideInHierarchy;

            count.Add(SceneManager.GetActiveScene().name);
        }

        //DestroyImmediate(source);
        for (int i = 0; i < count.Count; i++)
        {   
            if(count[i] != SceneManager.GetActiveScene().name)
            {
                count.RemoveAt(i);
            }
        }
        so.Update();
        using(new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Ducky:", EditorStyles.boldLabel);
            GUILayout.Space(4);
            op1 = (OPTIONS1)EditorGUILayout.EnumPopup("Change View:", op1);
        }
        if (OPTIONS1.SceneView == op1)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                op2 = (OPTIONS2)EditorGUILayout.EnumPopup("Modes:", op2);
                if (OPTIONS2.Dynamic == op2)
                {
                    animate = true;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Size:");
                        propsize.floatValue = EditorGUILayout.Slider(propsize.floatValue, 50, 300);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Volume:");
                        propvolume.floatValue = EditorGUILayout.Slider(propvolume.floatValue, 0, 1);
                    }
                }
                else if (OPTIONS2.Kinematic == op2)
                {
                    animate = true;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Size:");
                        propsize.floatValue = EditorGUILayout.Slider(propsize.floatValue, 50, 300);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Volume:");
                        propvolume.floatValue = EditorGUILayout.Slider(propvolume.floatValue, 0, 1);
                    }
                }
                else
                {
                    animate = false;
                    animtex = animload.Idleling1;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Size:");
                        propsize.floatValue = EditorGUILayout.Slider(propsize.floatValue, 50, 300);
                    }
                }
                SceneView.RepaintAll();
            }
        }
        else
        {
            if (animtex == null)
                return;

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                op3 = (OPTIONS3)EditorGUILayout.EnumPopup("Modes:", op3);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Volume:");
                    propvolume.floatValue = EditorGUILayout.Slider(propvolume.floatValue, 0, 1);
                }
            }


            HandleEditorStates();
            GUI.color = new Color(1, 1, 1, 1);
            if (position.width <= position.height - 70)
            {
                DuckyRect = new Rect(-3, position.height / 2 - (position.width - 110) / 2, position.width - 20, position.width - 20);
                GUI.DrawTexture(new Rect(-3, position.height / 2 - (position.width - 110) / 2, position.width - 20, position.width - 20), PlayAnimation(animtex));
                GUI.color = new Color(1, 1, 1, 0);
            }
            else
            {
                DuckyRect = new Rect(position.width / 2 - (position.height - 70) / 2 - 3, 90, position.height - 90, position.height - 90);
                GUI.DrawTexture(new Rect(position.width / 2 - (position.height - 70) / 2 - 3, 90, position.height - 90, position.height - 90), PlayAnimation(animtex));
            }

            if (OPTIONS3.Kinematic == op3)
            {
                animate = true;
                Repaint();
            }
            else
            {
                animate = false;
                animtex = animload.Idleling1;
            }
        }

        so.ApplyModifiedProperties();
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        if (!count.Contains(SceneManager.GetActiveScene().name))
        {
            instsource = Instantiate(source);
            instsource.hideFlags = HideFlags.HideInHierarchy;

            count.Add(SceneManager.GetActiveScene().name);
        }

        if (animtex == null || OPTIONS1.EditorView == op1)
            return;
        oldanimtex = animtex;

        Handles.BeginGUI();
        //Calculating Physics
        if (OPTIONS2.Dynamic == op2)
        {
            Vector3 pos = sceneView.camera.transform.position;
            Vector3 rot = sceneView.camera.transform.forward;

            //Calculating Direction
            Vector3 newdir = pos - lastpos;
            float diffright = Mathf.Sign(Vector3.Dot((pos - lastpos), sceneView.camera.transform.right));
            float diffup = Mathf.Sign(Vector3.Dot((pos - lastpos), -sceneView.camera.transform.up));
            float dist = sceneView.cameraDistance;

            //Moving when Moving
            pose.x -= diffright * Mathf.Abs(sceneView.camera.transform.InverseTransformDirection(newdir).x) * 300 / dist;
            pose.y -= diffup * Mathf.Abs(sceneView.camera.transform.InverseTransformDirection(newdir).y) * 700 / dist;

            //Moving when Rotating
            Vector3 somerot = rot - lastrot;
            pose -= Vector3.right * somerot.magnitude * Mathf.Sign(Vector3.Dot(-sceneView.camera.transform.right, lastrot)) * 200;
            pose -= Vector3.up * somerot.magnitude * Mathf.Sign(Vector3.Dot(sceneView.camera.transform.up, lastrot)) * 250;

            pose.y += 1;
            pose.x = Mathf.Clamp(pose.x, 0, sceneView.camera.scaledPixelWidth / 2 - propsize.floatValue);
            pose.y = Mathf.Clamp(pose.y, 0, sceneView.camera.scaledPixelHeight / 2 - propsize.floatValue);

            screenPos = new Rect(flip == false ? pose.x : pose.x + propsize.floatValue, pose.y, flip == false ? propsize.floatValue : -propsize.floatValue, propsize.floatValue);
            Buttonpos = new Rect(pose.x + 20, pose.y + 10, propsize.floatValue - 40, propsize.floatValue - 20);
            if (screenPos.x < oldscreenPos.x - .5f)
            {
                flip = false;
            }
            else if (screenPos.x > oldscreenPos.x + .5f)
            {
                flip = true;
            }
        }
        else
        {
            screenPos = new Rect(propsize.floatValue, sceneView.position.height - propsize.floatValue - 30, -propsize.floatValue, propsize.floatValue);
            Buttonpos = new Rect(10, sceneView.position.height - propsize.floatValue - 20, propsize.floatValue - 40, propsize.floatValue - 20);
        }

        HandleSceneStates(sceneView);
        Frame = oldanimtex != animtex ? 0 : Frame;

        GUI.color = new Color(1, 1, 1, 1);
        GUI.DrawTexture(screenPos, PlayAnimation(animtex));

        Handles.EndGUI();
        lastpos = sceneView.camera.transform.position;
        lastrot = sceneView.camera.transform.forward;
        oldscreenPos = screenPos;
        sceneView.Repaint();
    }

    Texture2D PlayAnimation(Texture2D[] texture)
    {
        if (animate && OPTIONS2.Static != op2)
        {
            if ((float)EditorApplication.timeSinceStartup >= time)
            {
                time = (float)EditorApplication.timeSinceStartup + 1f / 24f;
                Frame++;

                if (Frame >= texture.Length)
                {
                    Frame = 0;
                }
                return texture[Frame];
            }
        }
        else
        {
            return texture[0];
        }
        int frane = Frame - 1;
        frane = Mathf.Clamp(frane, 0, texture.Length - 1);
        return texture[frane];
    }

    void HandleSceneStates(SceneView sceneView)
    {
        if (!animate || OPTIONS2.Static == op2)
            return;

        if (pose.y >= sceneView.camera.scaledPixelHeight / 2 - propsize.floatValue - 10)
        {
            BedTime--;
            if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseLeaveWindow)
            {
                BedTime = 0;
            }
            if (BedTime >= -15000)
            {
                GUI.color = new Color(1, 1, 1, 0);
                if (GUI.Button(Buttonpos, new GUIContent()))
                {
                    instsource.GetComponent<AudioSource>().volume = propvolume.floatValue;
                    instsource.GetComponent<AudioSource>().Play();
                    animtex = animload.Quack;
                    Frame = 0;
                }
                else
                {
                    if (animtex != animload.Quack)
                    {
                        if (Frame >= animtex.Length - 1)
                        {
                            random = Random.Range(0, 21);
                        }
                        animtex = random <= 12 ? animload.Idleling1 : random <= 13 ? animload.Shaking : animload.Idleling2;
                    }
                }

                if(animtex == animload.Quack && Frame >= 11)
                {
                    animtex = animload.Idleling1;
                }

            }
            else if (BedTime < -15000)
            {
                animtex = animload.Sleeping;
            }
        }
        else if (pose.y < sceneView.camera.scaledPixelHeight / 2 - propsize.floatValue - 10 && OPTIONS2.Dynamic == op2)
        {
            animtex = animload.Falling;
        }
    }
    void HandleEditorStates()
    {
        if (!animate)
            return;

        BedTime--;
        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseLeaveWindow)
        {
            BedTime = 0;
        }
        if (BedTime >= -15000)
        {
            GUI.color = new Color(1, 1, 1, 0);
            if (GUI.Button(new Rect(DuckyRect.x, DuckyRect.y, DuckyRect.width, DuckyRect.height), new GUIContent()))
            {
                instsource.GetComponent<AudioSource>().volume = propvolume.floatValue;
                instsource.GetComponent<AudioSource>().Play();
                animtex = animload.Quack;
                Frame = 0;
            }
            else
            {
                if (animtex != animload.Quack)
                {
                    if (Frame >= animtex.Length - 1)
                    {
                        random = Random.Range(0, 21);
                    }
                    animtex = random <= 12 ? animload.Idleling1 : random <= 13 ? animload.Shaking : animload.Idleling2;
                }
            }

            if (animtex == animload.Quack && Frame >= 11)
            {
                animtex = animload.Idleling1;
            }
        }
        else if (BedTime < -15000)
        {
            animtex = animload.Sleeping;
        }
    }
}