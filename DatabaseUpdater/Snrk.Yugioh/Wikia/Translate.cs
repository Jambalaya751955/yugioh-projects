using System;

namespace DatabaseDownloader.Wikia_
{
    class Translate
    {
        static string[] common_r = new string[6] {
            "共通", "Common", "Common", "Commune", "Comune", "Común" };
        static string[] rare_r = new string[6] {
            "レア", "Rare", "Rare", "Rare", "Rara", "Rara" };
        static string[] superRare_r = new string[6] {
            "スーパーレア", "Super Rare", "Super Rare", "Super Rare", "Super Rara", "Super Rara" };
        static string[] ultraRare_r = new string[6] {
            "ウルトラレア", "Ultra Rare", "Ultra Rare", "Ultra Rare", "Ultra Rara", "Ultra Rara" };
        static string[] ultimateRare_r = new string[6] {
            "アルティメットレア", "Ultimate Rare", "Ultimate Rare", "Ultimate Rare", "Rara Ultimate", "Raro Definitivo" };
        static string[] secretRare_r = new string[6] {
            "シークレットレア", "Secret Rare", "Secret Rare", "Secret Rare", "Rara Segreta", "Rara Secreta" };
        static string[] ghostRare_r = new string[6] {
            "ゴーストレア", "Ghost Rare", "Ghost Rare", "Ghost Rare", "Rara Fantasma", "Rara Fantasma" };
        static string[] parallelRare_r = new string[6] {
            "パラレルレア", "Parallel Rare", "Parallel Rare", "Parallèle Rare", "Rara Parallela", "Rara Paralela" };
        static string[] goldRare_r = new string[6] {
            "ゴールドレア", "Gold Rare", "Gold Rare", "Gold Rare", "Ultra Rara Gold", "Rara Dorada" };
        static string[] starfoilRare_r = new string[6] {
            "星ォイルレア", "Starfoil Rare", "Starfoil Rare", "Starfoil", "Rara Starfoil", "Rara Starfoil" };
        //static string[] millenniumRare_r = new string[6] {
        //"ミレニアムレア", "Millennium Rare", "", "", "", "Rara Milenio" };

        public static string CardRarity(string str, string language)
        {
            if (String.IsNullOrEmpty(language))
                language = "en";

            language = language.ToLower();
            int index = GetIndex(language);
            string s = String.IsNullOrEmpty(str) ? "" : str.ToLower();

            if (index == -1) return str;

            if (s == "common") return common_r[index];
            else if (s == "rare") return rare_r[index];
            else if (s == "super rare") return superRare_r[index];
            else if (s == "ultra rare") return ultraRare_r[index];
            else if (s == "ultimate rare") return ultimateRare_r[index];
            else if (s == "secret rare") return secretRare_r[index];
            else if (s == "ghost rare") return ghostRare_r[index];
            else if (s == "parallel rare") return parallelRare_r[index];
            else if (s == "gold rare") return goldRare_r[index];
            else if (s == "starfoil rare") return starfoilRare_r[index];

            return str;
        }

        private static int GetIndex(string lang)
        {
            switch (lang)
            {
                case "ja": return 0;
                case "en": return 1;
                case "de": return 2;
                case "fr": return 3;
                case "it": return 4;
                case "es": return 5;
                default: return -1;
            }
        }
    }
}
