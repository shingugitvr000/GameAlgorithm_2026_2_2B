using UnityEngine;

namespace AlgoLesson8
{
    public sealed class FactoryMachine : MonoBehaviour
    {
        [SerializeField] private string machineId;
        [SerializeField] private string displayName;
        [SerializeField] private float secondsPerItem;
        private Renderer bodyRenderer;
        private Color baseColor;

        public string MachineId => machineId;
        public string DisplayName => displayName;
        public float SecondsPerItem => secondsPerItem;

        public void Initialize(string id, string label, float seconds, Color color)
        {
            machineId = id;
            displayName = label;
            secondsPerItem = seconds;
            name = $"Machine_{id}";
            bodyRenderer = GetComponent<Renderer>();
            baseColor = color;
            bodyRenderer.material.color = color;
            CreateLabel(label);
        }

        public MachineSnapshot CreateSnapshot() =>
            new MachineSnapshot(machineId, displayName, secondsPerItem, gameObject.activeInHierarchy);

        public void SetWorking(bool working)
        {
            if (bodyRenderer != null)
                bodyRenderer.material.color = working ? Color.Lerp(baseColor, Color.white, 0.55f) : baseColor;
        }

        private void CreateLabel(string labelText)
        {
            GameObject labelObject = new GameObject("WorldLabel");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = labelText;
            label.characterSize = 0.085f;
            label.fontSize = 28;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = Color.white;
        }

        private void LateUpdate()
        {
            if (Camera.main == null) return;
            foreach (TextMesh text in GetComponentsInChildren<TextMesh>())
                text.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
