using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlgoLesson8.Editor
{
    [InitializeOnLoad]
    public static class AutomationFactorySceneBuilder
    {
        private const string ScenePath = "Assets/Lesson8/Scenes/Lesson08_AutomationFactory.unity";

        static AutomationFactorySceneBuilder()
        {
            EditorApplication.delayCall += CreateIfMissing;
        }

        [MenuItem("AlgoLab/Lesson 8/Rebuild Automation Factory")]
        public static void Rebuild() => Build(true);

        public static void CapturePreview()
        {
            EditorSceneManager.OpenScene(ScenePath);
            Camera camera = Camera.main;
            RenderTexture target = new RenderTexture(1280, 720, 24);
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            string previewPath = Path.Combine(Application.dataPath, "Lesson8", "Lesson8_AutomationFactory.png");
            File.WriteAllBytes(previewPath, image.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(image);
            Debug.Log("Lesson 8 preview captured.");
        }

        private static void CreateIfMissing()
        {
            if (!File.Exists(ScenePath)) Build(false);
        }

        private static void Build(bool open)
        {
            Directory.CreateDirectory("Assets/Lesson8/Scenes");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject environment = Root("01_ENVIRONMENT_Factory");
            CreateFloor(environment.transform);
            GameObject belts = Root("02_CONVEYORS");
            CreateBelts(belts.transform);
            GameObject machines = Root("03_MACHINES");
            CreateMachines(machines.transform);
            CreateCamera();
            CreateLights();
            Root("06_UI_RuntimeOverlay");
            Root("07_GAME_MANAGER").AddComponent<FactoryGame>();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (open) EditorSceneManager.OpenScene(ScenePath);
            Debug.Log("Lesson 8 scene created: " + ScenePath);
        }

        private static void CreateFloor(Transform parent)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Factory_Floor";
            floor.transform.SetParent(parent);
            floor.transform.localScale = new Vector3(1.45f, 1f, 1.05f);
            floor.GetComponent<Renderer>().material.color = new Color(0.055f, 0.075f, 0.09f);
        }

        private static void CreateBelts(Transform parent)
        {
            CreateBelt(parent, new Vector3(-5.2f, 0.16f, -2.3f), new Vector3(8.5f, 0.18f, 1.15f));
            CreateBelt(parent, new Vector3(-5.2f, 0.16f, 2.3f), new Vector3(8.5f, 0.18f, 1.15f));
            CreateBelt(parent, new Vector3(0.6f, 0.16f, 0f), new Vector3(1.15f, 0.18f, 5.2f));
            CreateBelt(parent, new Vector3(4.7f, 0.16f, 0f), new Vector3(3.1f, 0.18f, 1.15f));
        }

        private static void CreateBelt(Transform parent, Vector3 position, Vector3 scale)
        {
            GameObject belt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            belt.name = "Conveyor";
            belt.transform.SetParent(parent);
            belt.transform.position = position;
            belt.transform.localScale = scale;
            belt.GetComponent<Renderer>().material.color = new Color(0.12f, 0.18f, 0.22f);
        }

        private static void CreateMachines(Transform parent)
        {
            CreateMachine(parent, "IronMine", "철 광산", 1.4f, new Vector3(-7.2f, 0.8f, -2.3f), new Color(0.35f, 0.55f, 0.72f));
            CreateMachine(parent, "Smelter", "용광로", 2.2f, new Vector3(-3.7f, 0.8f, -2.3f), new Color(1f, 0.35f, 0.1f));
            CreateMachine(parent, "GearPress", "기어 프레스", 1.7f, new Vector3(-0.3f, 0.8f, -2.3f), new Color(1f, 0.65f, 0.12f));
            CreateMachine(parent, "CopperMine", "구리 광산", 1.5f, new Vector3(-7.2f, 0.8f, 2.3f), new Color(0.8f, 0.38f, 0.16f));
            CreateMachine(parent, "WireMill", "전선 가공기", 2.6f, new Vector3(-3.7f, 0.8f, 2.3f), new Color(0.2f, 0.82f, 0.52f));
            CreateMachine(parent, "Assembler", "조립기", 3.4f, new Vector3(0.6f, 0.8f, 0f), new Color(0.55f, 0.38f, 1f));
            CreateMachine(parent, "Shipping", "출하장", 1.1f, new Vector3(6.2f, 0.8f, 0f), new Color(0.25f, 0.62f, 1f));
        }

        private static void CreateMachine(Transform parent, string id, string label, float seconds, Vector3 position, Color color)
        {
            GameObject machine = GameObject.CreatePrimitive(PrimitiveType.Cube);
            machine.transform.SetParent(parent);
            machine.transform.position = position;
            machine.transform.localScale = new Vector3(1.65f, 1.6f, 1.65f);
            machine.AddComponent<FactoryMachine>().Initialize(id, $"{label}\n{seconds:0.0}초", seconds, color);
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = Root("04_CAMERA_Main");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 15.5f, -15.5f);
            cameraObject.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.backgroundColor = new Color(0.015f, 0.025f, 0.035f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.fieldOfView = 52f;
        }

        private static void CreateLights()
        {
            GameObject root = Root("05_LIGHTING");
            GameObject lightObject = new GameObject("Factory_DirectionalLight");
            lightObject.transform.SetParent(root.transform);
            lightObject.transform.rotation = Quaternion.Euler(52f, -28f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.45f;
            light.color = new Color(0.82f, 0.9f, 1f);
        }

        private static GameObject Root(string name) => new GameObject(name);
    }
}
