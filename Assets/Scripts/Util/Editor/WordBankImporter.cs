using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using System.Linq;
using UnidecodeSharpFork;

// thank ChatGPT
[ScriptedImporter(1, "wordbank")]
public class WordBankImporter : ScriptedImporter
{
    public bool ToUnidecode = true;
    public bool ToLowerCase = true;
    public bool RemoveEmptyWord = true;
    public bool Sort = true;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Read the contents of the text file and store them in a string array
        var lines = File.ReadAllLines(ctx.assetPath);

        // Remove empty line
        IEnumerable<string> filteredLines = lines;
        if (ToUnidecode)
        {
            filteredLines = filteredLines.Select(line => line.Unidecode());
        }
        if (ToLowerCase)
        {
            filteredLines = filteredLines.Select(line => line.ToLower());
        }
        if (RemoveEmptyWord)
        {
            filteredLines = filteredLines.Where(line => !string.IsNullOrWhiteSpace(line));
        }
        if (Sort)
        {
            // don't count spaces
            filteredLines = filteredLines.OrderBy(line => line.Count(c => c != ' ')).ThenBy(line => line);
        }

        // Create a new WordBankSO ScriptableObject
        WordBankSO wordBankAsset = ScriptableObject.CreateInstance<WordBankSO>();

        // Set the lines property of the WordBankSO ScriptableObject
        wordBankAsset.Words = filteredLines.ToList();

        // add to ctx
        ctx.AddObjectToAsset("WordBank", wordBankAsset);
    }
}
