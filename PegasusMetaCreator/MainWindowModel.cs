using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace PegasusMetaCreator
{
    public class MainWindowModel
    {
        private MainWindowViewModel mainWindowViewModel;
        public MainWindowModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            loadAutoFillJson();
            LoadDefaultPathsJson();

            //saveSettingsJson();//remove this from here and make button for saving settings/options
            loadSettingsJson();
        }

        public void createPegasusMetadataFile() // can only be used when at least one game with a collection and file options available ? maybe game too
        {
            List<string> output = new List<string>();
            List<CollectionModel> collections = new List<CollectionModel>();
            List<string> collectionsTemp = new List<string>();

            foreach (var game in mainWindowViewModel.AddedExecutables)
            {
                foreach (var item in game) //add collections
                    if (item.Name == "collection" && !collectionsTemp.Contains(item.Content))
                    {
                        collectionsTemp.Add(item.Content);
                        collections.Add(new CollectionModel() { collection = item.Content });
                    }

                foreach (var collection in collections) //add games to collections
                {
                    string tempFile = string.Empty;
                    bool shouldBeAdded = false;
                    foreach (var item in game)
                    {
                        if(item.Name == "file")
                            tempFile = item.Content;
                        if(item.Name == "collection" && item.Content == collection.collection)
                            shouldBeAdded = true;
                    }
                    if(shouldBeAdded)
                        collection.files.Add(tempFile);
                }
            }
            
            foreach (var collection in collections)
            {
                output.Add(string.Empty);
                output.Add("collection: " + collection.collection);
                output.Add("launch: {file.path}");
                output.Add("files:");

                foreach (var file in collection.files)
                {
                    var cd2 = Directory.GetParent(mainWindowViewModel.DefaultMetadataPath);
                    Environment.CurrentDirectory = AssemblyDirectory;// cd2.FullName;
                    var p = Path.GetFullPath(mainWindowViewModel.DefaultMetadataPath);
                    var x = Path.GetDirectoryName(p);

                    Environment.CurrentDirectory = AssemblyDirectory;
                    var cd = Directory.GetParent(mainWindowViewModel.DefaultProgramPath + @"\dummy.exe");
                    Environment.CurrentDirectory = cd.FullName;
                    var y = Path.GetFullPath(file);

                    output.Add("  " + Path.GetRelativePath(x, y));
                }
            }

            foreach (var game in mainWindowViewModel.AddedExecutables) //i think collection should be at the start of every game
            {
                output.Add(string.Empty);
                List<string> otherOptions = new List<string>();
                foreach (var item in game)
                {
                    if(item.Name == "description")
                    {
                        if(item.Content != null)
                            item.Content = item.Content.Replace("\n", "\n  ");
                        output.Add(item.Name + ": " + item.Content);
                    }
                    else if (item.Name != "collection" && item.Name == "game")
                        output.Add(item.Name + ": " + item.Content);
                    else if(item.Name != "collection")
                        otherOptions.Add(item.Name + ": " + item.Content);//output.Add(item.Name + ": " + item.Content);

                }
                output.AddRange(otherOptions);
            }

            Environment.CurrentDirectory = AssemblyDirectory;
            File.WriteAllLines(Directory.GetParent(mainWindowViewModel.DefaultMetadataPath) + "\\" + Path.GetFileName(mainWindowViewModel.DefaultMetadataPath), output);
        }

        public static string AssemblyDirectory
        {
            get
            {
                UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
                return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            }
        }

        internal void SaveDefaultPathsJson()
        {
            DefaultPathsModel paths = new DefaultPathsModel() 
            { 
                DefaultMetadataPath = mainWindowViewModel.DefaultMetadataPath, 
                DefaultProgramPath = mainWindowViewModel.DefaultProgramPath 
            };

            File.WriteAllText("defaultpaths.json", JsonConvert.SerializeObject(paths));
        }

        internal void LoadDefaultPathsJson()
        {
            DefaultPathsModel paths = new DefaultPathsModel();
            if (File.Exists("defaultpaths.json"))
            {
                paths = JsonConvert.DeserializeObject<DefaultPathsModel>(File.ReadAllText("defaultpaths.json"));
                mainWindowViewModel.DefaultMetadataPath = paths.DefaultMetadataPath;
                mainWindowViewModel.DefaultProgramPath = paths.DefaultProgramPath;
            }
        }

        public void applyAutoFillRules(string selectedApplication)
        {
            foreach (var rule in mainWindowViewModel.AutoFill)
                for (int i = 0; i < mainWindowViewModel.Settings.Count; i++)
                    if (mainWindowViewModel.Settings[i].Name == rule.SettingName)
                    {
                        Match match = Regex.Match(selectedApplication, rule.RegexFilter);
                        if (rule.UseRegexFullMatch)
                        {
                            mainWindowViewModel.Settings.Insert(i, new SettingsModel(mainWindowViewModel.Settings[i].FieldType, mainWindowViewModel.Settings[i].Name, mainWindowViewModel)
                            {
                                Content = (string.IsNullOrEmpty(rule.AppendFront) ? string.Empty : rule.AppendFront) + match.Value + (string.IsNullOrEmpty(rule.AppendEnd) ? string.Empty : rule.AppendEnd),
                                FieldType = mainWindowViewModel.Settings[i].FieldType,
                                Name = mainWindowViewModel.Settings[i].Name
                            });

                        }
                        else
                        {
                            
                            mainWindowViewModel.Settings.Insert(i, new SettingsModel(mainWindowViewModel.Settings[i].FieldType, mainWindowViewModel.Settings[i].Name, mainWindowViewModel)
                            {
                                Content = (string.IsNullOrEmpty(rule.AppendFront) ? string.Empty : rule.AppendFront) + match.Groups[rule.SelectedRegexGroupe].Value + (string.IsNullOrEmpty(rule.AppendEnd) ? string.Empty : rule.AppendEnd),
                                FieldType = mainWindowViewModel.Settings[i].FieldType,
                                Name = mainWindowViewModel.Settings[i].Name
                            });
                        }

                       
                        mainWindowViewModel.Settings.Remove(mainWindowViewModel.Settings[i+1]);
                        break; //weird workaround, but it works!

                    }
        }

        internal void ShowCorrespondingAddedExecutables(string path)
        {
            foreach (var items in mainWindowViewModel.AddedExecutables)
            {
                foreach (var item in items)
                {
                    if(item.Name == "file" && item.Content == path)
                    {
                        ObservableCollection<SettingsModel> settings = new ObservableCollection<SettingsModel>();
                        foreach (var setting in items)
                        {
                            
                            settings.Add(new SettingsModel(setting.FieldType, setting.Name, mainWindowViewModel) { 
                                Content = setting.Content,
                                FieldType = setting.FieldType,
                                Name = setting.Name
                            });
                        }
                        mainWindowViewModel.Settings = settings;
                    }
                }
            }
        }

        public void saveAutoFillJson()
        {
            File.WriteAllText("autofill.json", JsonConvert.SerializeObject(mainWindowViewModel.AutoFill));
        }


        private void loadAutoFillJson()
        {
            ObservableCollection<AutoFillModel> x = new ObservableCollection<AutoFillModel>();
            if (File.Exists("autofill.json"))
                x = JsonConvert.DeserializeObject<ObservableCollection<AutoFillModel>>(File.ReadAllText("autofill.json"));

            if (File.Exists("autofill.json"))
                foreach (var item in x)
                {
                    mainWindowViewModel.AutoFill.Add(new AutoFillModel(mainWindowViewModel)
                    {
                        SettingName = item.SettingName,
                        AppendFront = item.AppendFront,
                        RegexFilter = item.RegexFilter,
                        AppendEnd = item.AppendEnd,
                        UseRegexFullMatch = item.UseRegexFullMatch,
                        SelectedRegexGroupe = item.SelectedRegexGroupe
                    });
                }
        }

        public void saveSettingsJson()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(mainWindowViewModel.Settings));
        }
        class TempSettingsModel //workaround
        {
            public string Name { get; set; }
            public string FieldType { get; set; }
        }

        private void loadSettingsJson()
        {
            //if (File.Exists("settings.json"))
            //    mainWindowViewModel.Settings = JsonConvert.DeserializeObject<ObservableCollection<SettingsModel>>(File.ReadAllText("settings.json"));

            //workaround
            mainWindowViewModel.Settings = new ObservableCollection<SettingsModel>();
            if (File.Exists("settings.json"))
                foreach (var item in JsonConvert.DeserializeObject<List<TempSettingsModel>>(File.ReadAllText("settings.json")))
                    mainWindowViewModel.Settings.Add(new SettingsModel(item.FieldType, item.Name, mainWindowViewModel) { FieldType = item.FieldType, Name = item.Name });
        }

        public List<string> listAllExecutables(string path)
        {
            List<string> executables = new List<string>();
            foreach (string programm in Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories))
                executables.Add(Path.GetRelativePath(path, programm));
            return executables;
        }

    }
}
