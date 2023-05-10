using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Diagnostics;

namespace SaveSystem
{
    public interface ICloudStorageProvider
    {
        // Przykład implementacji zapisu chmurowego

        // Metody przyjmują argument typu byte[] z uwagi niezależnosc od używanej usługi chumorowej
        void UploadToCloud(string path, byte[] contents);
        byte[] LoadFromCloud(string path);
    }
    public class SaveStore
    {
        // Z uwagi na zastosowanie słownika, jako formy tworzenia zapisów, łatwiej jest w przyszłosci zmienić format danych z pliku JSON
        // Słownikiem jest zarówno lista zapisów jak i same zapisy, dzięki czemu mogą korzystać z tych samych metod
        private Dictionary<string, object> saveData = new Dictionary<string, object>();
        public void Store(string key, object value)
        {
            if (value == null || (value is string && string.IsNullOrEmpty(value.ToString())))
            {
                throw noValueEx;
            }
            else
            {
                saveData[key] = value;
            }
        }
        public object Get(string key)
        {
            if (saveData.ContainsKey(key))
            {
                return saveData[key];
            }
            else
            {
                return null;
                throw noKeyEx;
            }
        }

        public int Count(Dictionary<string, Dictionary<string, object>> dict)
        {
            return dict.Values.Sum(d => d.Count);
        }

        public void Save(string path, string format)
        {
            string choosedFormat = format.ToLower();
            if (choosedFormat == "json")
            {
                SaveToJSON(path);
            }
            else if (choosedFormat == "binary")
            {
                SaveToBinary(path);
            }
            else if (choosedFormat == "xml")
            {
                SaveToXML(path);
            }
            else throw wrongFormatEx;
        }

        public static SaveStore Load(string path, string format)
        {
            string choosedFormat = format.ToLower();
            SaveStore saveStore = new SaveStore();
            if (choosedFormat == "json")
            {
                saveStore = LoadFromJSON(path);
            }
            else if (choosedFormat == "binary")
            {
                saveStore = LoadFromBinary(path);
            }
            else if (choosedFormat == "xml")
            {
                saveStore = LoadFromXML(path);
            }
            else throw saveStore.wrongFormatEx;
            return saveStore;
        }
        protected internal void SaveToJSON(string path)
        {
            // Zapis do pliku JSON przegląda każda pare (klucz,wartosc) zawartą w zęwntrznym słowniku,
            // oraz do każdej pary odnajduje pary wewnętrznego słownika, by zapisać całość w listy zapisów
            SaveStore saves = this;
            Dictionary<string, object> savesToSerialize = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> save in saves)
            {
                Dictionary<string, object> saveToSerialize = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> element in (save.Value as SaveStore))
                {
                    saveToSerialize.Add(element.Key, element.Value);
                }
                savesToSerialize.Add(save.Key, saveToSerialize);
            }

            string jsonString = JsonConvert.SerializeObject(savesToSerialize, Formatting.Indented);
            File.WriteAllText(path, jsonString);
        }
        protected internal void SaveToBinary(string path)
        {

        }
        protected internal void SaveToXML(string path)
        {

        }

        protected internal static SaveStore LoadFromJSON(string path)
        {
            // Odczyt z pliku JSON przegląda każda pare (klucz,wartosc) zawartą w zęwntrznym słowniku,
            // oraz do każdej pary odnajduje pary wewnętrznego słownika, by zwrócić całość w formie listy zapisów
            SaveStore saveStore = new SaveStore();
            string json = File.ReadAllText(path);
            Dictionary<string, Dictionary<string, object>> dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
            if (dict != null)
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> entry in dict)
                {
                    SaveStore save = new SaveStore();
                    foreach (KeyValuePair<string, object> kvp in entry.Value)
                    {
                        save.Store(kvp.Key, kvp.Value);
                    }
                    saveStore.Store(entry.Key, save);
                }
            }
            return saveStore;
        }

        protected internal static SaveStore LoadFromBinary(string path)
        {
            return null;
        }

        protected internal static SaveStore LoadFromXML(string path)
        {
            return null;
        }


        public static SaveStore LoadFromCloud(ICloudStorageProvider cloudStorage)
        {
            // Koncept metody LoadFromCloud
            SaveStore saveStore = new SaveStore();
            var content = cloudStorage.LoadFromCloud("");
            var deserializedContent = Encoding.Default.GetString(content);
            saveStore.saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(deserializedContent);
            return saveStore;
        }
        public void SaveToCloud(ICloudStorageProvider cloudStorage)
        {
            // Koncept metody UploadToCloud
            var contents = JsonConvert.SerializeObject(saveData);
            var serialized_contents = Encoding.Default.GetBytes(contents);
            cloudStorage.UploadToCloud("", serialized_contents);

        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
{
            return saveData.GetEnumerator();
        }
        public IEnumerable<string> GetSaveNames()
        {
            return saveData.Keys;
        }
        public KeyNotFoundException noValueEx = new KeyNotFoundException("The value or values need to be inserted.");
        public KeyNotFoundException noKeyEx = new KeyNotFoundException("The key or keys does not exist in the save data.");
        public ArgumentException wrongFormatEx = new ArgumentException("You provided wrong format.");
    }

    public class CloudStorage : ICloudStorageProvider
    {
        public byte[] LoadFromCloud(string path)
        {
            return new byte[0];
        }

        public void UploadToCloud(string path, byte[] contents)
        {

        }
    }
}
