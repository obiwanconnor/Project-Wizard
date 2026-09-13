using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.EditorTools
{
    /// <summary>
    /// The kids write chest jokes in a spreadsheet, not the Inspector
    /// (GDD section 11: "write 20 of these — this is the best job").
    /// One line per row; blank lines are skipped.
    /// </summary>
    public static class FlavourCsvImporter
    {
        [MenuItem("Where Are My Keys/Import Flavour Text From CSV...")]
        public static void Import()
        {
            string csvPath = EditorUtility.OpenFilePanel("Flavour text CSV", Application.dataPath, "csv");
            if (string.IsNullOrEmpty(csvPath)) return;

            var lines = new List<string>();
            foreach (var row in File.ReadAllLines(csvPath))
            {
                string trimmed = row.Trim().Trim('"');
                if (!string.IsNullOrEmpty(trimmed)) lines.Add(trimmed);
            }

            string savePath = EditorUtility.SaveFilePanelInProject(
                "Save Flavour Table", "FlavourTable", "asset", "Where to save the imported table");
            if (string.IsNullOrEmpty(savePath)) return;

            var table = ScriptableObject.CreateInstance<FlavourTable>();
            table.lines = lines.ToArray();

            AssetDatabase.CreateAsset(table, savePath);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Imported", $"{lines.Count} lines imported into {Path.GetFileName(savePath)}.", "Nice");
        }
    }
}
