using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using System.Linq;

// thank ChatGPT
[ScriptedImporter(1, "wordbank")]
public class WordBankImporter : ScriptedImporter
{
    public bool RemoveEmptyWord = true;
    public bool ToLowerCase = true;
    public bool Sort = true;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Read the contents of the text file and store them in a string array
        var lines = File.ReadAllLines(ctx.assetPath);

        // Remove empty line
        IEnumerable<string> filteredLines = lines;
        if (RemoveEmptyWord)
        {
            filteredLines = lines.Where(line => !string.IsNullOrWhiteSpace(line));
        }
        if (ToLowerCase)
        {
            filteredLines = lines.Select(line => line.ToLower());
        }
        if (Sort)
        {
            filteredLines = lines.OrderBy(line => line);
        }

        // Create a new WordBankSO ScriptableObject
        WordBankSO wordBankAsset = ScriptableObject.CreateInstance<WordBankSO>();

        // Set the lines property of the WordBankSO ScriptableObject
        wordBankAsset.Words = filteredLines.ToList();

        // add to ctx
        ctx.AddObjectToAsset("WordBank", wordBankAsset);
    }
}
