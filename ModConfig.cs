using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.IO;

namespace ModConfiguration {
    public class ModConfig {
        private readonly Dictionary<string, ModOption> _options = new Dictionary<string, ModOption>();
        private string _fileName = "";
        private Preferences _preferences;

        /// <summary>
        /// The number of options in the configuration file.
        /// </summary>
        public int Count {
            get { return _options.Count; }
        }

        /// <summary>
        /// The name of the configuration file (without extension).
        /// </summary>
        public string FileName {
            get { return _fileName; }
            set {
                if(!_fileName.Equals(value) && !string.IsNullOrWhiteSpace(value)) {
                    _fileName = value;
                    _preferences = new Preferences(FilePath);
                }
            }
        }

        /// <summary>
        /// Where the configuration file will be saved.
        /// </summary>
        public string FilePath {
            get { return Path.Combine(Main.SavePath, "Mod Configs", FileName + ".json"); }
        }

        /// <summary>
        /// Create a new ModConfig. The file is not created until you call <see cref="Save"/> for the first time.
        /// </summary>
        /// <param name="fileName">filename (without extension)</param>
        public ModConfig(string fileName) {
            FileName = fileName;
        }

        /// <summary>
        /// Add a new configuration option.
        /// </summary>
        public void Add(string name, object defaultValue) {
            if(!_options.ContainsKey(name)) {
                _options.Add(name, new ModOption(name, defaultValue));
            }
        }

        /// <summary>
        /// Add a new configuration option.
        /// </summary>
        public void Add(ModOption option) {
            if(!_options.ContainsKey(option.Name)) {
                _options.Add(option.Name, option);
            }
        }

        /// <summary>
        /// Add new configuration options.
        /// </summary>
        public void Add(params ModOption[] options) {
            foreach(ModOption option in options) {
                Add(option);
            }
        }

        /// <summary>
        /// Remove a configuration option.
        /// </summary>
        public void Remove(string name) {
            if(_options.ContainsKey(name)) {
                _options.Remove(name);
            }
        }

        /// <summary>
        /// Remove a configuration option.
        /// </summary>
        public void Remove(ModOption option) {
            if(_options.ContainsKey(option.Name)) {
                _options.Remove(option.Name);
            }
        }

        /// <summary>
        /// Get the value of a configuration option.
        /// </summary>
        public object Get(string name) {
            return (_options.ContainsKey(name) ? _options[name].Value : null);
        }

        /// <summary>
        /// Gets all of the configuration options.
        /// </summary>
        public List<ModOption> GetAll() {
            return _options.Values.ToList();
        }

        /// <summary>
        /// Gets all the configuration options of a specified type.
        /// </summary>
        public List<ModOption<T>> OfType<T>() {
            return _options.Values.Where(o => o.Value is T).ToList().ConvertAll(o => (ModOption<T>)o);
        }

        /// <summary>
        /// Set the value of a configuration option.
        /// </summary>
        public void Set(string name, object value) {
            _options[name].Value = value;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        public IOrderedEnumerable<ModOption> OrderBy<T>(Func<ModOption, T> keySelector) {
            return _options.Values.ToList().OrderBy(keySelector);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by using a specified comparer.
        /// </summary>
        public IOrderedEnumerable<ModOption> OrderBy<T>(Func<ModOption, T> keySelector, IComparer<T> comparer) {
            return _options.Values.ToList().OrderBy(keySelector, comparer);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public IOrderedEnumerable<ModOption> OrderByDescending<T>(Func<ModOption, T> keySelector) {
            return _options.Values.ToList().OrderByDescending(keySelector);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order by using a specified comparer.
        /// </summary>
        public IOrderedEnumerable<ModOption> OrderByDescending<T>(Func<ModOption, T> keySelector, IComparer<T> comparer) {
            return _options.Values.ToList().OrderByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Perform an action on each ModOption in the configuration file.
        /// </summary>
        public void ForEach(Action<ModOption> action) {
            _options.Values.ToList().ForEach(action);
        }

        /// <summary>
        /// Load the configuration from <see cref="FilePath"/>, or write to a new file if it doesn't exist.
        /// </summary>
        public void Load() {
            Read();

            var prefKeys = _preferences.GetAllKeys();

            if((prefKeys.Count > 0 && _options.Keys.Count > 0) &&
               (_options.Keys.Except(prefKeys).Any() || prefKeys.Except(_options.Keys).Any())) {
                int fileIndex = 0;
                string newFile = FilePath.Remove(FilePath.LastIndexOf('.')) + ".bak";

                while(File.Exists(newFile + fileIndex)) {
                    fileIndex++;
                }

                File.Copy(FilePath, newFile + fileIndex, true);
            }

            Save();
        }

        /// <summary>
        /// Read the configuration from <see cref="FilePath"/>.
        /// </summary>
        /// <returns>whether a configuration file exists</returns>
        private void Read() {
            if(_preferences.Load()) {
                foreach(ModOption opt in _options.Values) {
                    object value = _preferences.Get(opt.Name, opt.Value);

                    if(value is JObject) {
                        opt.Value = ((JObject)value).ToObject(opt.Value.GetType());
                    }
                    else if(value is JArray) {
                        opt.Value = ((JArray)value).ToObject(opt.Value.GetType());
                    }
                    else {
                        opt.Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Write the configuration file.
        /// </summary>
        public void Save() {
            Save(_options.Values.ToList());
        }

        /// <summary>
        /// Sort the configuration options according to a key, then save the configuration file.
        /// </summary>
        public void SortSave<T>(Func<ModOption, T> keySelector) {
            Save(OrderBy(keySelector).ToList());
        }

        /// <summary>
        /// Sort the configuration options by using a specified comparer, then save the configuration file.
        /// </summary>
        public void SortSave<T>(Func<ModOption, T> keySelector, IComparer<T> comparer) {
            Save(OrderBy(keySelector, comparer).ToList());
        }

        /// <summary>
        /// Save the specified list of <see cref="ModOption"/>s.
        /// </summary>
        /// <param name="modOptions"></param>
        private void Save(List<ModOption> modOptions) {
            _preferences.Clear();

            foreach(ModOption opt in modOptions) {
                _preferences.Put(opt.Name, opt.Value);
            }

            _preferences.Save();
        }
    }

    public class ModOption {
        public string Name { get; set; }
        public object Value { get; set; }

        public ModOption(string name, object value) {
            Name = name;
            Value = value;
        }
    }

    public class ModOption<T> {
        public string Name { get; set; }
        public T Value { get; set; }

        public ModOption(string name, T value) {
            Name = name;
            Value = value;
        }

        public static explicit operator ModOption<T>(ModOption option) {
            if(option.Value is T) {
                return new ModOption<T>(option.Name, (T)option.Value);
            }
            else {
                return new ModOption<T>(option.Name, default(T));
            }
        }
    }
}
