using Android.App;
using Android.Content;

namespace MasterCompound.Utils
{
    public static class SharedPreferences
    {
        private static ISharedPreferences GetPrefs()
        {
            return Application.Context.GetSharedPreferences("MasterCompound", FileCreationMode.Private);
        }

        private static ISharedPreferencesEditor GetEditorPrefs()
        {
            var prefs = Application.Context.GetSharedPreferences("MasterCompound", FileCreationMode.Private);
            return prefs.Edit();
        }

        public static string GetString(string key, string defValue = "")
        {
            var prefs = GetPrefs();
            return prefs.GetString(key, defValue);
        }

        public static void SaveString(string key, string value)
        {
            var editor = GetEditorPrefs();
            editor.PutString(key, value);
            editor.Apply();
        }

        public static void RemoveKey(string key)
        {
            var editor = GetEditorPrefs();

            editor.Remove(key);
            editor.Apply();
        }
    }
}