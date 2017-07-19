using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Helpers
{
    class Translations
    {
        public static Dictionary<string, string> FlavorText = new Dictionary<string, string>() {
            { "ja", "説明" }, { "en", "Flavor Text" }, { "de", "Beschreibung" },
            { "fr", "Description" }, { "it", "Descrizione" }, { "es", "Descripción" }
        };

        public static Dictionary<string, string> CardText = new Dictionary<string, string>() {
            { "ja", "カードテキスト" }, { "en", "Card Text" }, { "de", "Kartentext" },
            { "fr", "Texte de Carte" }, { "it", "Testo Carta" }, { "es", "Texto de Carta" }
        };

        public static Dictionary<string, string> MonsterEffect = new Dictionary<string, string>() {
            { "ja", "モンスター効果" }, { "en", "Monster Effect" }, { "de", "Monstereffekt" },
            { "fr", "Effet de Monstre" }, { "it", "Effetto Mostro" }, { "es", "Efecto de Monstruo" }
        };
        
        public static Dictionary<string, string> PendulumEffect = new Dictionary<string, string>() {
            { "ja", "ペンデュラム効果" }, { "en", "Pendulum Effect" }, { "de", "Pendeleffekt" },
            { "fr", "Effet Pendule" }, { "it", "Effetto Pendulum" }, { "es", "Efecto de Péndulo" }
        };

        public static Dictionary<string, string> PendulumScale = new Dictionary<string, string>() {
            { "ja", "ペンデュラムスケール" }, { "en", "Pendulum Scale" }, { "de", "Pendelbereich" },
            { "fr", "Échelle Pendule" }, { "it", "Valore Pendulum" }, { "es", "Escala de Péndulo" }
        };

        public static Dictionary<string, string> Attribute = new Dictionary<string, string>() {
            { "ja", "属性" }, { "en", "Attribute" }, { "de", "Attribut" },
            { "fr", "Attribut" }, { "it", "Attributo" }, { "es", "Atributo" }
        };

        public static Dictionary<string, string> TypeSingular = new Dictionary<string, string>() {
            { "ja", "タイプ" }, { "en", "Type" }, { "de", "Typ" },
            { "fr", "Type" }, { "it", "Tipo" }, { "es", "Tipo" }
        };

        public static Dictionary<string, string> TypePlural = new Dictionary<string, string>() {
            { "ja", "タイプ" },  { "en", "Types" }, { "de", "Typen" },
            { "fr", "Types" }, { "it", "Tipi" }, { "es", "Tipos" }
        };
        
        public static Dictionary<string, string> Level = new Dictionary<string, string>() {
            { "ja", "レベル" }, { "en", "Level" }, { "de", "Level" },
            { "fr", "Niveau" }, { "it", "Livello" }, { "es", "Nivel" }
        };

        public static Dictionary<string, string> LimitationSingular = new Dictionary<string, string>() {
            { "ja", "制限" }, { "en", "Limitation" }, { "de", "Limitierung" },
            { "fr", "Limitation" }, { "it", "Limitazione" }, { "es", "Limitación" }
        };

        public static Dictionary<string, string> LimitationPlural = new Dictionary<string, string>() {
            { "ja", "制限事項" }, { "en", "Limitations" }, { "de", "Limitierungen" },
            { "fr", "Limites" }, { "it", "Limitazioni" }, { "es", "Limitaciones" }
        };
    }
}
