using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameNotesEditorWindow : EditorWindow
{
    private const string prefsKey = "GameNotes";

    private List<GameNote> notes = new();
    private VisualElement rightPane;
    private ListView notesListView;

    [SerializeField] private int selectedIndex = -1;
    
    [MenuItem("Dracon/Game Notes")]
    public static void ShowExample()
    {
        GetWindow<GameNotesEditorWindow>();
    }
    
    public void CreateGUI()
    {
        LoadNotes();
        VisualElement root = rootVisualElement;

        var toolbar = new Toolbar();
        var newBtn = new Button(clickEvent: NewNote)
        {
            name = "New",
            text = "New"
        };
        var deleteBtn = new Button(clickEvent: DeleteNote)
        {
            name = "Delete",
            text = "Delete"
        };
        
        toolbar.Add(newBtn);
        toolbar.Add(deleteBtn);
        var flexContainer = new VisualElement()
        {
            style =
            {
                flexGrow = 1
            }
        };
        
        var saveToDiskBtn = new Button(clickEvent: Backup)
        {
            name = "Save to disk",
            text = "Save to disk"
        };
        var loadFromDiskBtn = new Button(clickEvent: LoadBackup)
        {
            name = "Load from disk",
            text = "Load from disk"
        };
        var helpBtn = new Button(clickEvent: ShowHelpMessage)
        {
            name = "?",
            text = "?"
        };
        toolbar.Add(flexContainer);
        toolbar.Add(saveToDiskBtn);
        toolbar.Add(loadFromDiskBtn);
        toolbar.Add(helpBtn);

        root.Add(toolbar);
        
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        root.Add(splitView);

        notesListView = new ListView()
        {
            style =
            {
                paddingLeft = 10,
                paddingRight = 10,
                paddingTop = 10,
                paddingBottom = 10
            },
        };
        rightPane = new ScrollView(ScrollViewMode.Vertical)
        {
            style =
            {
                paddingLeft = 10,
                paddingRight = 10,
                paddingTop = 10,
                paddingBottom = 10
            }
        };
        
        splitView.Add(notesListView);
        splitView.Add(rightPane);
        
        notesListView.makeItem = () => new Label();
        notesListView.bindItem = (item, index) => { (item as Label).text = notes[index].Key; };
        notesListView.itemsSource = notes;
        notesListView.selectionChanged += OnNoteSelectionChanged;
        notesListView.selectedIndex = selectedIndex;
        notesListView.selectionChanged += _ => selectedIndex = notesListView.selectedIndex;
    }

    private void LoadNotes()
    {
        var json = EditorPrefs.GetString(prefsKey, "");
        notes = JsonConvert.DeserializeObject<List<GameNote>>(json);

        RefreshList();
    }

    private void SaveNotes()
    {
        var json = JsonConvert.SerializeObject(notes);
        EditorPrefs.SetString(prefsKey, json);
        LoadNotes();
    }

    private void NewNote()
    {
        NewNoteDetailsDialog.Init(() =>
        {
            var noteName = EditorPrefs.GetString("GameNotes_NewNote_Name", "");
            Debug.Log("New note name: " + noteName);
            if (string.IsNullOrEmpty(noteName)) return;
            notes.Add(new GameNote(){Key = noteName, Content = "Put your notes here!"});
            SaveNotes();
        });
    }

    private void DeleteNote()
    {
        if (!EditorUtility.DisplayDialog("Delete?", "Delete your selected note?", "Delete", "Cancel")) return;
        notes.RemoveAt(selectedIndex);
        if(notes.Count > 0) {
            selectedIndex = Mathf.Min(selectedIndex, notes.Count - 1);
        } else {
            selectedIndex = -1;
        }
        SaveNotes();
    }

    private void Backup()
    {
        var path = EditorUtility.SaveFilePanel("Save to disk", Application.dataPath, "game_notes.json", "json");
        if (path == null) return;
        var json = EditorPrefs.GetString(prefsKey, "");
        File.WriteAllText(path, json);
    }

    private void LoadBackup()
    {
        var confirmed = EditorUtility.DisplayDialog("Confirm", "This will overwrite your local notes, with no ability to undo. Are you sure?",
            "Overwrite", "Cancel");

        if (!confirmed) return;
        var path = EditorUtility.OpenFilePanel("Load from disk", Application.dataPath, "json");
        if (path == null) return;
        var text = File.ReadAllText(path);
        EditorPrefs.SetString(prefsKey, text);
        LoadNotes();
    }

    private void ShowHelpMessage()
    {
        EditorUtility.DisplayDialog("Help",
            "This is the Game Notes system! \nThis is a tool to help you save and organise thoughts regarding your game development. Its use is fairly simple.\n\n" +
            "First, create a note! Just click new, and enter the note name. Once you have done this, the note will appear in the list. From there you can select it, enter your content and thats it!" +
            "You can edit the content whenever you want, and it will automatically save every time.\n\n" +
            "Use caution though: by default these notes only save to your own machine. This is to avoid conflicts with your team.\n\n" +
            "This means if you change machines, or need to reset your project, the notes will be erased. To avoid this, use the 'save to disk' feature." +
            "This will ask you for a file location, and will save your notes as a .json file. From there, you can transfer them where ever you like, and load them using 'load from disk'.\n\n" +
            "Think of it like a checkpoint system! I hope you enjoy this tool, and that it comes in handy.", "Nice");
    }
    
    private void OnNoteSelectionChanged (IEnumerable<object> items)
    {
        rightPane.Clear();
        var enumerator = items.GetEnumerator();
        
        if (!enumerator.MoveNext()) return;
        if (enumerator.Current is not GameNote selected) return;

        var textArea = new TextField
        {
            multiline = true,
            value = selected.Content,
            verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible
        };
        textArea.RegisterValueChangedCallback(x =>
        {
            // set notes selected index to newValue
            notes[selectedIndex].Content = x.newValue;
            SaveNotes();
        });
        rightPane.Add(textArea);
    }

    private void RefreshList()
    {
        if (notesListView == null)
        {
            //Debug.LogError("Null list view");
            return;
        }

        notesListView.itemsSource = notes;
        notesListView.Rebuild();
        Repaint();
    }
}

public class GameNote
{
    public string Key;
    public string Content;
}

public class NewNoteDetailsDialog : EditorWindow
{
    private TextField nameField;
    private static NewNoteDetailsDialog window;
    private static Action onComplete;
    public static void Init(Action _onComplete)
    {
        onComplete = _onComplete;
        window = GetWindow<NewNoteDetailsDialog>(true);
        window.ShowModalUtility();
    }

    private void CreateGUI()
    {
        nameField = new TextField("Name: ");

        rootVisualElement.Add(nameField);

        var submitButton = new Button(clickEvent: Submit)
        {
            text = "Submit"
        };
        rootVisualElement.Add(submitButton);
    }

    private void Submit()
    {
        EditorPrefs.SetString("GameNotes_NewNote_Name", nameField.value);
        window.Close();
        onComplete?.Invoke();
    }
}