using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;

namespace PegasusMetaCreator
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<SettingsModel> _Settings;
        public ObservableCollection<SettingsModel> Settings
        {
            get { return _Settings; }
            set { SetProperty(ref _Settings, value); }
        }

        private ObservableCollection<AutoFillModel> _AutoFill;
        public ObservableCollection<AutoFillModel> AutoFill
        {
            get { return _AutoFill; }
            set { SetProperty(ref _AutoFill, value); }
        }

        private List<string> _Executables;
        public List<string> Executables
        {
            get { return _Executables; }
            set { SetProperty(ref _Executables, value); }
        }
        private ObservableCollection<List<SettingsModel>> _AddedExecutables;
        public ObservableCollection<List<SettingsModel>> AddedExecutables
        {
            get { return _AddedExecutables; }
            set { SetProperty(ref _AddedExecutables, value); }
        }
        private ObservableCollection<string> _AddedExecutablesUI;
        public ObservableCollection<string> AddedExecutablesUI
        {
            get { return _AddedExecutablesUI; }
            set { SetProperty(ref _AddedExecutablesUI, value); }
        }
        private string _SelectedAddedExecutablesUI;
        public string SelectedAddedExecutablesUI
        {
            get { return _SelectedAddedExecutablesUI; }
            set 
            { 
                SetProperty(ref _SelectedAddedExecutablesUI, value);
                mainWindowModel.ShowCorrespondingAddedExecutables(value);
            }
        }
        private string _SelectedExecutable;
        public string SelectedExecutable
        {
            get { return _SelectedExecutable; }
            set 
            { 
                SetProperty(ref _SelectedExecutable, value);
                mainWindowModel.applyAutoFillRules(value);
            }
        }


        private string _DefaultProgramPath;
        public string DefaultProgramPath
        {
            get { return _DefaultProgramPath; }
            set 
            {
                SetProperty(ref _DefaultProgramPath, value);
                defaultPaths.DefaultProgramPath = value;
            }
        }
        private string _DefaultMetadataPath;
        public string DefaultMetadataPath
        {
            get { return _DefaultMetadataPath; }
            set 
            { 
                SetProperty(ref _DefaultMetadataPath, value);
                defaultPaths.DefaultMetadataPath = value;
            }
        }

        public List<string> FieldTypeOptions { get; set; }

        public string SelectedFieldType { get; set; }
        public string PropertyName { get; set; }

        //this block can be without setproperty i think
        public string SettingName { get; set; }
        public string AppendFront { get; set; }
        public string RegexFilter { get; set; }
        public string AppendEnd { get; set; }
        public bool UseRegexFullMatch { get; set; }
        public string SelectedRegexGroupe { get; set; }

        public ICommand AddAutoFilter { get; set; }
        public ICommand AddExecutables { get; set; }
        public ICommand SaveSettings { get; set; }
        public ICommand DeleteProgram { get; set; }
        public ICommand ExportMetafile { get; set; }
        public ICommand ImportMetafile { get; set; }
        public ICommand SaveDefaultPaths { get; set; }
        public ICommand AddPeroperty { get; set; }
        public ICommand ReloadExecutables { get; set; }

        public MainWindowModel mainWindowModel;
        private DefaultPathsModel defaultPaths = new DefaultPathsModel();
        public MainWindowViewModel()
        {
            FieldTypeOptions = new List<string>() { "textbox", "textbox-L", "textbox-XL" };

            //DefaultMetadataPath = @"..\metadata.pegasus.txt";
            DefaultMetadataPath = @"..\metadata.pegasus.txt";
            DefaultProgramPath = @"..\";
            SelectedFieldType = "textbox";

            AddAutoFilter = new DelegateCommand(addAutoFilter);
            AddExecutables = new DelegateCommand(addExecutables);
            SaveSettings = new DelegateCommand(saveSettings);
            DeleteProgram = new DelegateCommand(deleteProgram);
            ExportMetafile = new DelegateCommand(exportMetafile);
            ImportMetafile = new DelegateCommand(importMetafile);
            SaveDefaultPaths = new DelegateCommand(saveDefaultPaths);
            AddPeroperty = new DelegateCommand(addPeroperty);
            ReloadExecutables = new DelegateCommand(reloadExecutables);

            AutoFill = new ObservableCollection<AutoFillModel>();
            AddedExecutables = new ObservableCollection<List<SettingsModel>>();
            AddedExecutablesUI = new ObservableCollection<string>();


            mainWindowModel = new MainWindowModel(this);
            //Executables = mainWindowModel.listAllExecutables(@"C:\Users\Sebastian\Desktop\Racial Justice game bundle");
            Executables = mainWindowModel.listAllExecutables(DefaultProgramPath);
        }

        private void reloadExecutables()
        {
            Executables = mainWindowModel.listAllExecutables(DefaultProgramPath);
        }

        private void importMetafile() // need to find regex which also matches new lines for description
        {
            AddedExecutables = new ObservableCollection<List<SettingsModel>>();
            AddedExecutablesUI = new ObservableCollection<string>();

            List<string> metaData = new List<string>(); ;
            ObservableCollection<List<SettingsModel>> tempSettings = new ObservableCollection<List<SettingsModel>>();
            List<SettingsModel> test = new List<SettingsModel>();
            metaData = File.ReadAllText(DefaultMetadataPath).Split("\r\n\r\n").ToList();
            List<KeyValuePair<string, string>> collectionValues = new List<KeyValuePair<string, string>>();
            foreach (var data in metaData)
            {
                if (data.StartsWith("game: "))
                {
                    test = new List<SettingsModel>();
                    foreach (var property in Settings)
                    {
                        Match match = Regex.Match(data, "(?<=" + property.Name + @": )(.*)(?=)");
                        var testResult = match.Value;
                        test.Add(new SettingsModel(property.FieldType, property.Name, this) //change replace, waybee making problems with importing with multiline
                        {
                            Name = property.Name,
                            Content = testResult.Replace("\n", "").Replace("\r", ""),
                            FieldType = property.FieldType

                        });
                    }
                    tempSettings.Add(test);
                }
                else
                {
                    var x = data.Split("\r\n ");
                    Match match = Regex.Match(x[0], "(?<=collection: )(.*)(?=)");
                    for (int i = 1; i < x.Length; i++)
                    {
                        collectionValues.Add(new KeyValuePair<string, string>(match.Value, x[i]));
                    }
                }
            }
            foreach (var items in tempSettings) // foreach collection of games
            {
                string addToCollection = string.Empty;
                foreach (var item in items) //foreach game
                {
                    if(item.Name == "file")
                    {
                        //currentGame = item.Content;
                        foreach (var value in collectionValues) // check if collectionValues contains
                        {
                            if(value.Value == " " + item.Content)
                            {
                                addToCollection = value.Key.Replace("\r", "");
                            }
                        }
                    }
                }
                foreach (var item in items) //foreach game
                {
                    if (item.Name == "collection")
                    {
                        item.Content = addToCollection;
                    }
                }

            }
            //foreach (var data in metaData)
            //{

            //    if (data.StartsWith("\r\ncollection: "))
            //    {
            //        Match match = Regex.Match(data, @"(?<=\r\ncollection: )(.*)(?=[\s\S])");
            //        foreach (var item in tempSettings)
            //        {
            //            bool add = false; // when true then add collection to temp[i].Replace("  ", "")
            //            if (item.Name == "file")
            //            {
            //                var temp = data.Split("\r\n");
            //                for (int i = 4; i < temp.Length; i++)
            //                {
            //                    if (item.Content == temp[i].Replace("  ", ""))
            //                    {
            //                        add = true;
            //                    }
            //                }
            //            }
            //            if(item.Name == "collection")
            //            {
            //                ;
            //            }
            //        }
                    
            //    }
            //}
            AddedExecutables = tempSettings; // need to add it to AddedExecuteablesUI
            foreach (var items in AddedExecutables)
                foreach (var item in items)
                    if(item.Name == "file")
                        AddedExecutablesUI.Add(item.Content);

        }

        private void addPeroperty()
        {
            Settings.Add(new SettingsModel(SelectedFieldType, PropertyName, this) { Name = PropertyName, FieldType = SelectedFieldType });
            mainWindowModel.saveSettingsJson();
        }

        private void saveDefaultPaths()
        {
            mainWindowModel.SaveDefaultPathsJson();
            reloadExecutables();
        }

        private void exportMetafile()
        {
            mainWindowModel.createPegasusMetadataFile();
        }

        private void deleteProgram()
        {
            for (int j = 0; j < AddedExecutables.Count; j++)
                for (int i = 0; i < AddedExecutables[j].Count; i++)
                    if(AddedExecutables[j][i].Name == "file" && AddedExecutables[j][i].Content == SelectedAddedExecutablesUI)
                    {
                        AddedExecutablesUI.Remove(AddedExecutables[j][i].Content);
                        AddedExecutables.Remove(AddedExecutables[j]);
                        //SelectedAddedExecutablesUI = string.Empty; //maybe not needed
                        Settings = new ObservableCollection<SettingsModel>();
                        break;
                    }
        }

        private void saveSettings()
        {
            if(canChangeSettings())
                for (int j = 0; j < AddedExecutables.Count; j++)
                    for (int i = 0; i < AddedExecutables[j].Count; i++)
                        if (AddedExecutables[j][i].Name == "file" && AddedExecutables[j][i].Content == SelectedAddedExecutablesUI)
                        {
                            List<SettingsModel> settings = new List<SettingsModel>();
                            foreach (var setting in Settings)
                            {
                                settings.Add(setting);
                            }
                            AddedExecutables[j] = settings;
                            break;
                        }
        }

        private bool canChangeSettings()
        {
            string collection = string.Empty;
            string game = string.Empty;
            string file = string.Empty;

            foreach (var item in Settings)
            {
                if (item.Name == "collection")
                    collection = item.Content;
                else if (item.Name == "game")
                    game = item.Content;
                else if (item.Name == "file")
                    file = item.Content;

            }
            if (SelectedAddedExecutablesUI == game && !string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(collection))
                return true;
            return false;
        }

        private void addExecutables()// check if already in list
        {
            if (canAddExecutable())
            {
                bool fileIsFilled = false;
                foreach (var item in Settings)
                {
                    if (item.Name == "file" && !string.IsNullOrEmpty(item.Content))
                    {
                        AddedExecutablesUI.Add(item.Content);
                        fileIsFilled = true;
                    }
                }
                if (fileIsFilled)
                {
                    //AddedExecutables.Add(Settings);
                    List<SettingsModel> settings = new List<SettingsModel>();
                    foreach (var item in Settings)
                        settings.Add(new SettingsModel(item.FieldType, item.Name, this)
                        {
                            Content = item.Content,
                            FieldType = item.FieldType,
                            Name = item.Name
                        });

                    AddedExecutables.Add(settings);

                }
            }
            
        }

        private bool canAddExecutable()
        {
            string collection = string.Empty;
            string game = string.Empty;
            string file = string.Empty;

            foreach (var item in Settings)
            {
                if (item.Name == "collection")
                    collection = item.Content;
                else if (item.Name == "game")
                    game = item.Content;
                else if (item.Name == "file")
                    file = item.Content;

            }
            if (!string.IsNullOrEmpty(game) && !string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(collection))
                return true;
            return false;
        }

        public bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return value.All(char.IsNumber);
        }

        private void addAutoFilter()
        {
            if (IsNumeric(SelectedRegexGroupe))
            {
                AutoFill.Add(new AutoFillModel(this) { AppendEnd = AppendEnd, AppendFront = AppendFront, RegexFilter = RegexFilter, SettingName = SettingName, SelectedRegexGroupe = Convert.ToInt32(SelectedRegexGroupe), UseRegexFullMatch = UseRegexFullMatch });
                mainWindowModel.saveAutoFillJson();

            }
        }
    }
}
